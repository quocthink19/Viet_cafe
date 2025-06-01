using AutoMapper;
using System.Drawing.Imaging;
using Repository.IRepository;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Repository.UnitOfWork;
using Services.IServices;
using QRCoder;

namespace Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPromotionRepo _promotionRepo;
        private readonly IMapper _mapper;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IPromotionRepo promotionRepo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _promotionRepo =promotionRepo;
        }
        public async Task<OrderResponse> CreateOrder(Guid customerId, OrderRequest order)
        {
            double? finalPrice = 0;
            double? discountPrice = 0;

            var cart = await _unitOfWork.CartRepo.GetCartByCustomerId(customerId);
            Promotion promotion = null;

            if (!string.IsNullOrEmpty(order.Code))
            {
                promotion = await _promotionRepo.GetPromotionByCode(order.Code);

                if (promotion != null && promotion.DiscountPercent.HasValue)
                {
                    discountPrice = cart.TotalAmount * (promotion.DiscountPercent.Value / 100);
                    finalPrice = cart.TotalAmount - discountPrice;
                }
                else
                {
                    finalPrice = cart.TotalAmount;
                }
            }
            else
            {
                finalPrice = cart.TotalAmount;
            }
            var orderId = Guid.NewGuid();
            var item = _mapper.Map<List<OrderItem>>(cart.CartItems);
            foreach (var orderItem in item)
            {
                orderItem.Id = Guid.NewGuid();
                orderItem.OrderId = orderId;
            }

            var newOrder = new Order
            {
                Id = orderId,
                CustomerId = customerId,
                Payment = order.Paymemt,
                Status = Repository.Models.Enum.OrderStatus.NEW,
                PickUpTime = order.PickUpTime,
                OrderItems = item,
                TotalAmount = finalPrice,
                FinalPrice = finalPrice,
                DiscountPrice = discountPrice,
            };


            string qrContent = $"https://localhost:7207/Orders/update-by-qr/{newOrder.Id}";
            newOrder.QRcode = GenerateQrCodeBase64(qrContent);

            try
            {
                await _unitOfWork.OrderRepo.AddAsync(newOrder);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lưu đơn hàng:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }

            var response = _mapper.Map<OrderResponse>(newOrder);
            return response;
        }

        public async Task DeleteOrder(Guid Id)
        {
            var order = await GetOrderById(Id);
            await _unitOfWork.OrderRepo.DeleteAsync(Id);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<Order>> GetOrder()
        {
            var order = await _unitOfWork.OrderRepo.GetAll();
            return order;
        }

        public async Task<OrderResponse> GetOrderById(Guid Id)
        {
            var order =await _unitOfWork.OrderRepo.GetByIdAsync(Id);    
            if(order == null)
            {
                throw new Exception("không tìm thấy đơn hàng");
            }
            var orderes = _mapper.Map<OrderResponse>(order);
            return orderes;
        }

        public Task<OrderResponse> UpdateOrder(Guid Id, OrderRequest Order)
        {
            throw new NotImplementedException();
        }
        public async Task<OrderResponse> UpdateOrderByQR(Guid Id)
        {
            var order = await _unitOfWork.OrderRepo.GetByIdAsync(Id);
            if (order == null)
            {
                throw new Exception("không tìm thấy đơn hàng");
            }
            order.Status = Repository.Models.Enum.OrderStatus.COMPLETED;
            await _unitOfWork.OrderRepo.UpdateAsync(order);
            await _unitOfWork.SaveAsync();
            var res = _mapper.Map<OrderResponse>(order);
            return res;
        }
        private string GenerateQrCodeBase64(string content)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                   
                    byte[] qrCodeBytes = qrCode.GetGraphic(20);

                    
                    return $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
                }
            }
        }

    }
}


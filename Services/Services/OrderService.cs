using AutoMapper;
using System.Drawing.Imaging;
using Repository.IRepository;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Repository.UnitOfWork;
using Services.IServices;
using QRCoder;
using Repository.Models.Enum;

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

        public async Task<OrderResponse> CreateOrderWallet(Guid customerId, OrderRequest order)
        {
            double? finalPrice = 0;
            double? discountPrice = 0;

            var cart = await _unitOfWork.CartRepo.GetCartByCustomerId(customerId);
            var customer = await _unitOfWork.CustomerRepo.GetByIdAsync(customerId);
            Promotion promotion = null;
            
            int totalProductQuantity = cart.CartItems?.Sum(item => item.Quantity ?? 0) ?? 0;


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
            long orderId = DateTime.UtcNow.Ticks;
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
                Payment = Method.WALLET,
                Status = Repository.Models.Enum.OrderStatus.NEW,
                PickUpTime = order.PickUpTime,
                OrderItems = item,
                fullName = order.fullName,
                phoneNumber = order.phoneNumber,
                TotalAmount = finalPrice,
                FinalPrice = finalPrice,
                DiscountPrice = discountPrice,
            };

            if(customer.Wallet - (decimal)newOrder.FinalPrice >= 0)
            {
                customer.Wallet = customer.Wallet - (decimal)newOrder.FinalPrice;
            }else
            {
                throw new Exception("ví của bạn không đủ để thanh toán đơn hàng này ");
            }
            
            var orderLimit = await _unitOfWork.OrderSlotLimitRepo.GetSlotByTimeAsync(newOrder.PickUpTime);
            if (orderLimit != null)
            {
                var orderCount = await _unitOfWork.OrderRepo.GetOrdersCountAsync(orderLimit.StartedAt, orderLimit.EndTime);
                var cupCount = await _unitOfWork.OrderRepo.GetTotalCupsByPickUpTimeAsync(orderLimit.StartedAt, orderLimit.EndTime);

                if (orderLimit.MaxOrders <= orderCount || orderLimit.MaxCups <= cupCount)
                {
                    throw new Exception("Hiện tại đơn hàng đã quá tải, vui lòng đặt lại sau");
                }
            }

            string qrContent = $"https://localhost:7207/Order/update-by-qr/{newOrder.Id}";
            newOrder.QRcode = GenerateQrCodeBase64(qrContent);



            try
            {
                await _unitOfWork.CustomerRepo.UpdateAsync(customer);
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

        public async Task<OrderResponse> CreateOrder(Guid customerId, OrderRequest order)
        {
            double? finalPrice = 0;
            double? discountPrice = 0;

            // Lấy giỏ hàng của khách
            var cart = await _unitOfWork.CartRepo.GetCartByCustomerId(customerId);
            Promotion promotion = null;

            int totalProductQuantity = cart.CartItems?.Sum(item => item.Quantity ?? 0) ?? 0;

            // Tính giá theo promotion
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

           
            var orderItems = _mapper.Map<List<OrderItem>>(cart.CartItems);
            foreach (var orderItem in orderItems)
            {
                orderItem.Id = Guid.NewGuid();
            }
            
            var newOrder = new Order
            {
                
                CustomerId = customerId,
                Payment = Method.CASH,
                Status = Repository.Models.Enum.OrderStatus.NEW,
                PickUpTime = order.PickUpTime,
                OrderItems = orderItems,
                fullName = order.fullName,
                phoneNumber = order.phoneNumber,
                TotalAmount = finalPrice,
                FinalPrice = finalPrice,
                DiscountPrice = discountPrice,
                QRcode = ""
            };

           
            var orderLimit = await _unitOfWork.OrderSlotLimitRepo.GetSlotByTimeAsync(newOrder.PickUpTime);
            if (orderLimit != null)
            {
                var orderCount = await _unitOfWork.OrderRepo.GetOrdersCountAsync(orderLimit.StartedAt, orderLimit.EndTime);
                var cupCount = await _unitOfWork.OrderRepo.GetTotalCupsByPickUpTimeAsync(orderLimit.StartedAt, orderLimit.EndTime);

                if (orderLimit.MaxOrders <= orderCount || orderLimit.MaxCups <= cupCount)
                {
                    throw new Exception("Hiện tại đơn hàng đã quá tải, vui lòng đặt lại sau");
                }
            }

          
            try
            {
                await _unitOfWork.OrderRepo.AddAsync(newOrder);
                await _unitOfWork.SaveAsync();

              
                string qrContent = $"https://localhost:7207/Order/update-by-qr/{newOrder.Id}";
                newOrder.QRcode = GenerateQrCodeBase64(qrContent);

                
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



        public async Task DeleteOrder(long Id)
        {
            var order = await GetOrderById(Id);
            await _unitOfWork.OrderRepo.Delete(Id);
            await _unitOfWork.SaveAsync();  
        }

        public async Task<Customer> GetCustomerByOrderId(long orderId)
        {
            var customer = await _unitOfWork.OrderRepo.GetCustomerByOrderId(orderId);
            return customer;
        }

        public async Task<IEnumerable<Order>> GetOrder()
        {
            var order = await _unitOfWork.OrderRepo.GetAll();
            return order;
        }

        public async Task<OrderResponse> GetOrderById(long Id)
        {
            var order =await _unitOfWork.OrderRepo.GetById(Id);    
            if(order == null)
            {
                throw new Exception("không tìm thấy đơn hàng");
            }
            var orderes = _mapper.Map<OrderResponse>(order);
            return orderes;
        }

        public Task<OrderResponse> UpdateOrder(long Id, OrderRequest Order)
        {
            throw new NotImplementedException();
        }
        public async Task<OrderResponse> UpdateOrderByQR(long Id)
        {
            var order = await _unitOfWork.OrderRepo.GetById(Id);
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

        public async Task<OrderResponse> updateStatusOrder(long id, StatusOrderRequest status)
        {
            var order = await _unitOfWork.OrderRepo.GetById(id);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng");
            }

            var newStatus = status.Status;
            var currentStatus = order.Status;

            if (currentStatus == OrderStatus.CANCELLED)
            {
                throw new Exception("Đơn hàng đã bị hủy, không thể cập nhật trạng thái");
            }

            if (newStatus == OrderStatus.COMPLETED)
            {
                if (currentStatus == OrderStatus.NEW)
                {
                    throw new Exception("Đơn hàng chưa được xác nhận nên không thể chuyển trạng thái lên hoàn thành");
                }
                if (currentStatus == OrderStatus.PREPARING)
                {
                    throw new Exception("Đơn hàng chưa được chuẩn bị xong nên không thể chuyển trạng thái lên hoàn thành");
                }
            }
            if (newStatus == OrderStatus.READYFORPICKUP)
            {
                if (currentStatus == OrderStatus.NEW)
                {
                    throw new Exception("Đơn hàng chưa được xác nhận nên không thể chuyển sang trạng thái có thể nhận");
                }
                if (currentStatus == OrderStatus.CONFIRMED)
                {
                    throw new Exception("Đơn hàng chưa được chuẩn bị nên không thể chuyển sang trạng thái có thể nhận");
                }
            }
            if (newStatus == OrderStatus.CANCELLED && currentStatus == OrderStatus.COMPLETED)
            {
                throw new Exception("Đơn hàng đã hoàn thành nên không thể hủy");
            }

            order.Status = newStatus;
            await _unitOfWork.OrderRepo.UpdateAsync(order);
            await _unitOfWork.SaveAsync();
            return null;
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


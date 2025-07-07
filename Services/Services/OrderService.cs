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
using Microsoft.Extensions.Logging.Abstractions;
using Repository.Models.Filter;
using Repository.Repositories;

namespace Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPromotionRepo _promotionRepo;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IPromotionRepo promotionRepo, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _promotionRepo = promotionRepo;
            _emailService = emailService;

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
            var orderItems = _mapper.Map<List<OrderItem>>(cart.CartItems);
            foreach (var orderItem in orderItems)
            {
                orderItem.Id = Guid.NewGuid();
                
            }
            
            string code = UniqueCodeGenerator.GenerateCode();

            var newOrder = new Order
            {
                Code = code, 
                CustomerId = customerId,
                Payment = Method.WALLET,
                Status = Repository.Models.Enum.OrderStatus.NEW,
                PickUpTime = order.PickUpTime,
                OrderItems = orderItems,
                fullName = order.fullName,
                phoneNumber = order.phoneNumber,
                CreateAt = DateTime.Now,
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
            


            string qrContent = $"https://localhost:7207/Order/update-by-qr/{newOrder.Id}";
            newOrder.QRcode = GenerateQrCodeBase64(qrContent);



            try
            {
                await _unitOfWork.CustomerRepo.UpdateAsync(customer);
                await _unitOfWork.OrderRepo.AddAsync(newOrder);

                foreach (var cartItem in cart.CartItems)
                {
                    var productId = await _unitOfWork.ProductRepo.GetProductIdByCartItemIdAsync(cartItem.Id);
                    var quantity = cartItem.Quantity ?? 0;

                    if (productId != null && productId != Guid.Empty)
                    {
                        var product = await _unitOfWork.ProductRepo.GetByIdAsync(productId.Value);
                        if (product != null)
                        {
                            product.PurchaseCount = (product.PurchaseCount ?? 0) + quantity;
                            await _unitOfWork.ProductRepo.UpdateAsync(product);
                        }
                    }
                }

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

            long newOrderId = await GenerateUniqueRandomLongId();

            var orderItems = _mapper.Map<List<OrderItem>>(cart.CartItems);
            foreach (var orderItem in orderItems)
            {
                orderItem.Id = Guid.NewGuid();
                orderItem.OrderId = newOrderId;
            }
            string code = UniqueCodeGenerator.GenerateCode();
            var newOrder = new Order
            {
                Id = newOrderId,
                CustomerId = customerId,
                Code = code,
                Payment = Method.PAYOS,
                Status = Repository.Models.Enum.OrderStatus.NEW,
                PickUpTime = order.PickUpTime,
                OrderItems = orderItems,
                CreateAt = DateTime.Now,
                fullName = order.fullName,
                phoneNumber = order.phoneNumber,
                TotalAmount = finalPrice,
                FinalPrice = finalPrice,
                DiscountPrice = discountPrice,
                QRcode = ""
            };
          
          
            try
            {
                await _unitOfWork.OrderRepo.AddAsync(newOrder);
                foreach (var cartItem in cart.CartItems)
                {
                    var productId = await _unitOfWork.ProductRepo.GetProductIdByCartItemIdAsync(cartItem.Id);
                    var quantity = cartItem.Quantity ?? 0;

                    if (productId != null && productId != Guid.Empty)
                    {
                        var product = await _unitOfWork.ProductRepo.GetByIdAsync(productId.Value);
                        if (product != null)
                        {
                            product.PurchaseCount = (product.PurchaseCount ?? 0) + quantity;
                            await _unitOfWork.ProductRepo.UpdateAsync(product);
                        }
                    }
                }
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

        public async Task<IEnumerable<OrderResponse>> GetOrder()
        {
            var order = await _unitOfWork.OrderRepo.GetAll();
            return _mapper.Map<IEnumerable<OrderResponse>>(order);
             
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
            var customer = await _unitOfWork.OrderRepo.GetCustomerByOrderId(id);
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
                var subject = $"Đơn hàng {order.Code} của bạn đã sẵn sàng!";

                var body = $@"
                <!DOCTYPE html>
                <html>
                <body style='font-family: Arial, sans-serif; color: #333;'>
                  <h2>Kính gửi {customer.FullName},</h2>
                  <p>Cảm ơn bạn đã lựa chọn <b>Lượn Cafe</b>! 🎉</p>
                  <p>Đơn hàng <b>{order.Code}</b> của bạn đã được chuẩn bị xong và sẵn sàng để bạn đến nhận tại quán.</p>
                  <p>Hãy ghé qua để thưởng thức ly cà phê thơm ngon được chuẩn bị riêng cho bạn! ☕</p>
                  <p>Nếu có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email hoặc số điện thoại <b>0927363868</b>.</p>
                  <p>Trân trọng,<br><b>Lượn Cafe</b><br>22 Khổng Tử, P. Bình Thọ, Tp. Thủ đức | 0927363868 | <a href='https://www.instagram.com/luon_cafe/#'>Lượn Cafe</a></p>
                </body>
                </html>";

                await _emailService.SendEmail(customer.User.Email, subject, body);
            }
            if (newStatus == OrderStatus.CANCELLED && currentStatus == OrderStatus.COMPLETED)
            {
                throw new Exception("Đơn hàng đã hoàn thành nên không thể hủy");
            }
            if(newStatus == OrderStatus.CANCELLED)
            {
                customer.Wallet += (decimal)order.FinalPrice;
                foreach (var orderItem in order.OrderItems)
                {
                    
                    var quantity = orderItem.Quantity ?? 0;

                    if (orderItem.productId != null && orderItem.productId != Guid.Empty)
                    {
                        var product = await _unitOfWork.ProductRepo.GetByIdAsync(orderItem.productId);
                        if (product != null)
                        {
                            product.PurchaseCount = (product.PurchaseCount ?? 0) - quantity;
                            await _unitOfWork.ProductRepo.UpdateAsync(product);
                        }
                    }
                }
            }

            order.Status = newStatus;
            await _unitOfWork.CustomerRepo.UpdateAsync(customer);
            await _unitOfWork.OrderRepo.UpdateAsync(order);
            await _unitOfWork.SaveAsync();
            var res = _mapper.Map<OrderResponse>(order);
            return res;
        }

        private class UniqueCodeGenerator
        {
            private static readonly char[] chars =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

            public static string GenerateCode()
            {
                const string prefix = "VC-";
                int randomLength = 9 - prefix.Length;

                var random = new Random();
                var randomPart = new string(Enumerable.Repeat(chars, randomLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                return prefix + randomPart;
            }
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

        public async Task<IEnumerable<OrderResponse>> GetAllOrdersByCustomerId(Guid customerId)
        {
            var orders = await _unitOfWork.OrderRepo.GetAllOrdersByCustomerId(customerId);
            return _mapper.Map<IEnumerable<OrderResponse>>(orders);
        }

        public async  Task<string> OrderLimitNotification(DateTime dateTime)
        {
            TimeSpan time = dateTime.TimeOfDay;
            var (start, end) = RoundToHour(dateTime);
            var orderLimit = await _unitOfWork.OrderSlotLimitRepo.GetSlotByTimeAsync(time);
            if (orderLimit != null)
            {
                var orderCount = await _unitOfWork.OrderRepo.GetOrdersCountAsync(start, end);
                var cupCount = await _unitOfWork.OrderRepo.GetTotalCupsByPickUpTimeAsync(start, end);

                if (orderLimit.MaxOrders <= orderCount || orderLimit.MaxCups <= cupCount)
                {
                    throw new Exception("Hiện tại đơn hàng đã quá tải trong khung giờ bạn đặt, vui lòng đặt lại sau");
                }
            }
            return null;
        }
        public static (DateTime FloorHour, DateTime CeilHour) RoundToHour(DateTime dateTime)
        {
            
            DateTime floor = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, dateTime.Kind);

            
            DateTime ceil = dateTime.Minute == 0 && dateTime.Second == 0 && dateTime.Millisecond == 0
                ? floor
                : floor.AddHours(1);

            return (floor, ceil);
        }

        public async Task<PagedResult<Order>> GetFilteredOrdersAsync(OrderFilter filter)
        {
            var order = await _unitOfWork.OrderRepo.GetFilteredOrdersAsync(filter);
            
            return order;
        }
        public async Task<long> GenerateUniqueRandomLongId()
        {
            return long.Parse(DateTime.UtcNow.Ticks.ToString().Substring(3, 13));
        }
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Helper;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Services.IServices;
using Services.Services;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using Repository.UnitOfWork;
using static QRCoder.PayloadGenerator;
using static System.Net.WebRequestMethods;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly ICartService _cartService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;


        public PaymentController(IVnPayService vnPayService, IOrderService orderService, ICustomerService customerService, 
            ICartService cartService, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _vnPayService = vnPayService;
            _orderService = orderService;
            _customerService = customerService;
            _cartService = cartService;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        [Authorize]
        [HttpPost("payment-by-cash")]
        public async Task<ActionResult<OrderResponse>> CreateCashOrder([FromBody] OrderRequest dto)
        {
            var customer = await GetCurrentCustomer();
            var order = await _orderService.CreateOrder(customer.Id, dto);
            await _cartService.ClearCart(customer.Id);
            var respnose = new TResponse<OrderResponse>("Đơn hàng thanh toán tiền mặt đã được tạo thành công", order);
            return Ok(respnose);
        }
        [Authorize]
        [HttpPost("payment-by-wallet")]
        public async Task<ActionResult<OrderResponse>> PaymentByWallet([FromBody] OrderRequest dto) {
            var customer = await GetCurrentCustomer();
            var order = await _orderService.CreateOrderWallet(customer.Id, dto);
            await _cartService.ClearCart(customer.Id);
            var respnose = new TResponse<OrderResponse>("Đơn hàng thanh toán bằng ví đã được tạo thành công", order);
            return Ok(respnose);
    }
        
        [Authorize]
        [HttpPost("payment-by-vnpay")]
        public async  Task<ActionResult> CreatePaymentUrl([FromBody] OrderRequest dto)
        {
            try
            {
                var customer = await GetCurrentCustomer();
                var order = await _orderService.CreateOrder(customer.Id, dto);
                VnPayRequest vnPay = new VnPayRequest
                {
                    Amount = (double)order.TotalAmount,
                    CreatedDate = DateTime.UtcNow,
                    OrderId = order.Id
                };
                var paymentUrl = await  _vnPayService.CreatePaymentUrlAsync(HttpContext, vnPay);
                var respone = new TResponse<string>("link thanh toán", paymentUrl);
                return Ok(respone);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return BadRequest();
            }
        }
        [Authorize]
        [HttpPut("up-to-wallet")]
        public async Task<ActionResult> UpToWallet([FromBody] UpToWalletRequest dto)
        {
            var customer = await GetCurrentCustomer();
            var orderId = $"{customer.Id.ToString("N").Substring(0, 8)}-{DateTime.UtcNow.Ticks}";
            VnPayRequest vnPay = new VnPayRequest
            {
                Amount = (double)dto.Amount,
                CreatedDate = DateTime.UtcNow,
                /*OrderId = Guid.NewGuid(),*/
                CustomerId = customer.Id                
            };
            var paymentUrl = await _vnPayService.CreatePaymentUrlAsync(HttpContext, vnPay);
            var respone = new TResponse<string>("link thanh toán", paymentUrl);
            return Ok(respone);
        }
    

            [HttpGet("return")]
            public async Task<ActionResult> PaymentResponse()
            {
                try
                {
                 var vnpayRes = _vnPayService.PaymentExcute(Request.Query);
                string orderIdStr = Request.Query["vnp_TxnRef"];
                string vnpAmount = Request.Query["vnp_Amount"]; 
                Console.WriteLine($"Số tiền nhận từ VNPAY (vnp_Amount): {vnpAmount}");

                if (!string.IsNullOrEmpty(orderIdStr) && long.TryParse(orderIdStr, out long orderId))
                {
                    Console.WriteLine($"Order ID nhận từ VNPAY: {orderId}");
                    var customer = await _orderService.GetCustomerByOrderId(orderId);
                    if (customer != null)
                    {
                        await _cartService.ClearCart(customer.Id);
                    }

                }
                if (!vnpayRes.IsSuccess)
                    {
                        return Ok("giao dịch thành công");
                    }
                    return Ok(true);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                    return BadRequest("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
                }
            }
        [HttpGet("cancel")]
        public async Task<ActionResult> CancelPayment([FromQuery(Name = "OrderCode")] long orderId)
        {
            var payment = await _unitOfWork.PaymentRepo.GetByOrderId(orderId);
            if (payment == null)
            {
                return BadRequest("không tìm thấy thanh toán");
            }
            var order = await _unitOfWork.OrderRepo.GetById(orderId);
            if (order == null)
            {
                return BadRequest("không tìm thấy đơn hàng");
            }
            order.Status = Repository.Models.Enum.OrderStatus.CANCELLED;
            await _unitOfWork.OrderRepo.UpdateAsync(order);
            await _unitOfWork.SaveAsync();


            payment.Status = Repository.Models.Enum.PaymentStatus.FAILED;
            payment.Description += "cancel the payment";
            await _unitOfWork.PaymentRepo.UpdateAsync(payment);
            await _unitOfWork.SaveAsync();

            return Redirect($"https://luoncoffeeweb.vercel.app/payment-cancel");

        }
       

        [HttpGet("success")]
        public async Task<ActionResult> SuccessPayment([FromQuery(Name = "orderCode")] long code)
        {
           
            var order = await _unitOfWork.OrderRepo.GetById(code);
            if (order != null)
            {
                // Đơn hàng
                var payment = await _unitOfWork.PaymentRepo.GetByOrderId(code);
                if (payment == null) return BadRequest("Không tìm thấy thanh toán đơn hàng");
                if (payment.Status != Repository.Models.Enum.PaymentStatus.PAID)
                {
                    payment.Status = Repository.Models.Enum.PaymentStatus.PAID;
                    payment.Description += " - Success the payment.";
                    await _unitOfWork.PaymentRepo.UpdateAsync(payment);
                    await _unitOfWork.SaveAsync();
                }

                var customer = await _unitOfWork.OrderRepo.GetCustomerByOrderId(code);
                await _cartService.ClearCart(customer.Id);

                var subject = $"Thanh toán đơn hàng {order.Code} thành công";
                var body = $@"
        <!DOCTYPE html>
        <html lang=""vi"">
        <head>
          <meta charset=""UTF-8"">
          <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        </head>
        <body style=""font-family: Arial, sans-serif; color: #333; line-height: 1.6; max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9f9f9;"">
          <div style=""text-align: center; padding: 20px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
            <h2 style=""color: #2c3e50;"">Kính gửi {customer.FullName},</h2>
            <p style=""font-size: 16px;"">Cảm ơn bạn đã đặt hàng tại <strong>Lượn Cafe</strong>! 🎉</p>
            <p style=""font-size: 16px;"">Chúng tôi xin thông báo rằng <strong>đơn hàng {order.Code}</strong> của bạn đã được <span style=""color: #27ae60;"">thanh toán thành công</span>.</p>
            <p style=""font-size: 16px;"">Vui lòng chờ tin nhắn hoặc email tiếp theo từ chúng tôi để đến nhận ly cà phê thơm ngon của bạn! ☕</p>
            <p style=""font-size: 14px; color: #7f8c8d;"">Nếu bạn có bất kỳ câu hỏi nào, hãy liên hệ với chúng tôi qua email hoặc số điện thoại <strong>0927363868</strong>.</p>
            <div style=""margin-top: 20px; padding-top: 20px; border-top: 1px solid #eee;"">
              <p style=""font-size: 14px; color: #2c3e50;"">Trân trọng,<br><strong>Lượn Cafe</strong><br>22 Khổng Tử, P. Bình Thọ, Tp. Thủ đức | 0927363868 | <a href=""https://www.instagram.com/luon_cafe/#"" style=""color: #3498db; text-decoration: none;"">Lượn Cafe</a></p>
            </div>
          </div>
        </body>
        </html>";
                await _emailService.SendEmail(customer.User.Email, subject, body);
                return Redirect($"https://luoncoffeeweb.vercel.app/payment-success?orderCode={code}");
            }
            else
            {
                // Không phải Order, thử tìm TopUp
                var topup = await _unitOfWork.TopUpRepo.GetTopUpById(code);
                var customer = await _unitOfWork.CustomerRepo.GetCustomerById(topup.CustomerId);
                if (topup == null) return BadRequest("Không tìm thấy giao dịch nạp tiền này.");
                if (topup.Status != Repository.Models.Enum.PaymentStatus.PAID)
                {
                    topup.Status = Repository.Models.Enum.PaymentStatus.PAID;
                    topup.Description += " - Success topup.";
                    // Cộng tiền cho customer
                   
                    customer.Wallet += (decimal)topup.Amount;
                    await _unitOfWork.TopUpRepo.UpdateAsync(topup);
                    await _unitOfWork.CustomerRepo.UpdateAsync(customer);
                    await _unitOfWork.SaveAsync();
                }

                var subject = "Nạp tiền thành công vào tài khoản";
                var body = $@"
              <html>
            <body>
            <p>Xin chào {customer.FullName},</p>
            <p>Bạn đã nạp thành công số tiền <strong>{topup.Amount:N0} VND</strong> vào tài khoản của mình.</p>
            <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
            <br>
            <p>Lượn Cafe</p>
                </body>
              </html>";
                await _emailService.SendEmail(customer.User.Email, subject, body);

                return Redirect($"https://luoncoffeeweb.vercel.app/payment-success?orderCode={code}&type=topup");
            }
        }



        private async Task<Customer?> GetCurrentCustomer()
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(username)) return null;
                return await _customerService.GetCustomerByUsername(username);
            }
    }
    }

    
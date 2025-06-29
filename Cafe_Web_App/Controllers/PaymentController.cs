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

            return Ok(new { message = "thanh toán đơn hàng bị hủy" });

            }
        [HttpGet("success")]
        public async Task<ActionResult> SuccessPayment([FromQuery(Name = "orderCode")] long orderId)
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
            payment.Status = Repository.Models.Enum.PaymentStatus.PAID;
            payment.Description += " - Success the payment.";
            await _unitOfWork.PaymentRepo.UpdateAsync(payment);
            await _unitOfWork.SaveAsync();
            var customer = await _unitOfWork.OrderRepo.GetCustomerByOrderId(orderId);
            await _cartService.ClearCart(customer.Id);
            var body = $"Đơn hàng {order.Code} của bạn đã được thanh toán thành công vui lòng chờ tin nhắn thông báo đến nhận hàng của chúng tôi, Xin Cảm Ơn";
            await _emailService.SendEmail(customer.User.Email, "Thanh toán đơn hàng thành công ", body);

            return Ok(new { message = "thanh toán đơn hàng thành công" });
        }
        private async Task<Customer?> GetCurrentCustomer()
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(username)) return null;
                return await _customerService.GetCustomerByUsername(username);
            }
    }
    }

    
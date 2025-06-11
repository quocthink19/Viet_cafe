using Cafe_Web_App.Migrations;
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


        public PaymentController(IVnPayService vnPayService, IOrderService orderService, ICustomerService customerService, 
            ICartService cartService)
        {
            _vnPayService = vnPayService;
            _orderService = orderService;
            _customerService = customerService;
            _cartService = cartService;
         
        }

        [Authorize]
        [HttpPost("payment-by-wallet")]
        public async Task<ActionResult<OrderResponse>> PaymentByWallet([FromBody] OrderRequest dto) {
            var customer = await GetCurrentCustomer();
            var order = await _orderService.CreateOrderWallet(customer.Id, dto);
            var respnose = new TResponse<OrderResponse>("Đơn hàng thanh toán bằng ví đã được tạo thành công", order);
            return Ok(respnose);
    }
        
        [Authorize]
        [HttpPost("create")]
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
                OrderId = Guid.NewGuid(),
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

                if (!string.IsNullOrEmpty(orderIdStr) && Guid.TryParse(orderIdStr, out Guid orderId))
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

            private async Task<Customer?> GetCurrentCustomer()
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(username)) return null;
                return await _customerService.GetCustomerByUsername(username);
            }
    }
    }

    
using Cafe_Web_App.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Services.IServices;
using Services.Services;
using System.Security.Claims;

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

        public PaymentController(IVnPayService vnPayService, IOrderService orderService, ICustomerService customerService, ICartService cartService)
        {
            _vnPayService = vnPayService;
            _orderService = orderService;
            _customerService = customerService;
            _cartService = cartService;
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

            [HttpGet("return")]
            public async Task<ActionResult> PaymentResponse()
            {
                try
                {
                 var vnpayRes = _vnPayService.PaymentExcute(Request.Query);
                string orderIdStr = Request.Query["vnp_TxnRef"];
                
                if (!string.IsNullOrEmpty(orderIdStr) && Guid.TryParse(orderIdStr, out Guid orderId))
                {
                    Console.WriteLine($"Order ID nhận từ VNPAY: {orderId}");
                    var customer = await _orderService.GetCustomerByOrderId(orderId);
                    Console.WriteLine($"CustomerID: {customer.Id}");
                    await _cartService.ClearCart(customer.Id);
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

    
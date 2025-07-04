using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Net.payOS.Types;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Services.IServices;
using Services.Services;
using System.Security.Claims;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PayOSController : Controller
    {
        private readonly IPayOSService _payOSService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        public PayOSController(IOptions<PayOSSettings> payOSSettings, IPayOSService payOSService, IOrderService orderService, ICustomerService customerService)
        {
            _payOSService = payOSService;
            _orderService = orderService;
            _customerService = customerService;

        }

        [Authorize]
        [HttpPost("create-payment-url")]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] OrderRequest dto)
        {
            try
            {
                var customer = await GetCurrentCustomer();
                var order = await _orderService.CreateOrder(customer.Id, dto);
                var result = await _payOSService.CreatePaymentUrl(order.Id);
                return Ok(new { orderId = order.Id, PaymentUrl = result.checkoutUrl, PaymentId = result.paymentLinkId });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("create-top-up-payment-url")]
        public async Task<IActionResult> CreateTopUpPaymentUrl([FromBody] TopUpRequest dto)
        {
            try
            {
                // Lấy thông tin CustomerId từ JWT hoặc context (tuỳ app bạn implement)
                var customer = await GetCurrentCustomer(); // hoặc truyền Guid customerId nếu lấy được

                if (customer == null)
                {
                    return Unauthorized(new { Message = "Không xác định được người dùng" });
                }

                if (dto.Amount <= 0)
                {
                    return BadRequest(new { Message = "Số tiền nạp không hợp lệ" });
                }

                var result = await _payOSService.CreateTopUpPaymentUrl(dto.Amount, customer.Id);

                // Tuỳ bạn muốn trả về gì. Thường là trả paymentUrl hoặc object PaymentResult
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        /// <summary>
        /// Lấy thông tin thanh toán bằng orderCode
        /// </summary>
        /// 

        [HttpGet("info/{orderId}")]
        public async Task<IActionResult> GetPaymentInfo(int orderId)
        {
            try
            {
                var result = await _payOSService.GetPaymentInfo(orderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(string orderId, [FromBody] CancelRequest cancelRequest)
        {
            try
            {
                var result = await _payOSService.CancelOrder(orderId, cancelRequest.CancellationReason);
                return Ok(new { success = true, message = "Hủy link thanh toán thành công", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        private async Task<Customer?> GetCurrentCustomer()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username)) return null;
            return await _customerService.GetCustomerByUsername(username);
        }

        public class CancelRequest
        {
            public string CancellationReason { get; set; }
        }
    }
}


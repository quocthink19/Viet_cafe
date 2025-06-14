using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Net.payOS.Types;
using Repository.Models;
using Services.IServices;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PayOSController : Controller
    {
        private readonly IPayOSService _payOSService;
        private readonly IOrderService _orderService;
        public PayOSController(IOptions<PayOSSettings> payOSSettings, IPayOSService payOSService, IOrderService orderService)
        {
            _payOSService = payOSService;
            _orderService = orderService;
        }
       /* [HttpPost("webhook")]
        public async Task<IActionResult> WebhookHandler(WebhookType webhook)
        {
            try
            {
                await _orderService.ConfirmOrderPayment(webhook);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return BadRequest(new { success = false, message = ex.Message });
            }
        }*/
        /// <summary>
        /// Tạo link thanh toán từ orderId
        /// </summary>
        [HttpPost("create-payment-url/{orderId}")]
        public async Task<IActionResult> CreatePaymentUrl(long orderId)
        {
            try
            {
                var result = await _payOSService.CreatePaymentUrl(orderId);
                return Ok(new { orderId = orderId, PaymentUrl = result.checkoutUrl, PaymentId = result.paymentLinkId });
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

        public class CancelRequest
        {
            public string CancellationReason { get; set; }
        }
    }
}


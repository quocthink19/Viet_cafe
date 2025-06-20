using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Repository.Models.Enum;
using Services.IServices;
using Services.Services;
using System.Security.Claims;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, ICustomerService customerService)
        {
            _orderService = orderService;
            _customerService = customerService;
        }
        
        [Authorize]
        [HttpDelete("Id")]
        public async Task<ActionResult> DeleteOrder(long Id)
        {
            await _orderService.DeleteOrder(Id);
            return Ok(new { message = " đã xóa đơn hành thành công " });
        }
        [Authorize]
        [HttpPut("Id")]
        public async Task<ActionResult<OrderResponse>> UpdateStatusOrder(long Id,[FromBody] StatusOrderRequest status)
        {
            var order = await _orderService.updateStatusOrder(Id, status);
            var respnose = new TResponse<OrderResponse>("thay đổi trạng thái thành công", order);
            return Ok(respnose);
        }
        [Authorize]
        [HttpGet("get-orders-of-customer")]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetAllOrdersByCustomerId()
        {
            var customer = await GetCurrentCustomer();
            var orders = await _orderService.GetAllOrdersByCustomerId(customer.Id);
            var res = new TResponse<IEnumerable<OrderResponse>>("lấy danh sách tất đơn hàng thành công", orders);
            return Ok(res);
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders() {
            var orders = await _orderService.GetOrder();
            var res = new TResponse<IEnumerable<Order>>("lấy danh sách tất đơn hàng thành công", orders);
            return Ok(res);
        }
        [HttpPut("update-by-qr/{Id}")]
        public async Task<ActionResult> UpdateOrderByQR(long Id)
        {
            await _orderService.UpdateOrderByQR(Id);
            return Ok(new { message = "cập nhât trạng thái thành công" });
        }

        private async Task<Customer?> GetCurrentCustomer()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username)) return null;
            return await _customerService.GetCustomerByUsername(username);
        }
    }
}
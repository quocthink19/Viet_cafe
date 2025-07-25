﻿using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Repository.Models.Enum;
using Repository.Models.Filter;
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
        private readonly ITopUpService _topupService;

        public OrderController(IOrderService orderService, ICustomerService customerService, ITopUpService topupService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _topupService = topupService;
        }

        
        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredOrders([FromQuery] OrderFilter filter)
        {
            var result = await _orderService.GetFilteredOrdersAsync(filter);
            var res = new TResponse<PagedResult<Order>>("lấy danh sách đơn hàng thành công", result);
            return Ok(res);
        }

        [Authorize]
        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteOrder(long Id)
        {
            await _orderService.DeleteOrder(Id);
            return Ok(new { message = " đã xóa đơn hành thành công " });
        }
        [Authorize]
        [HttpGet("{Id}")]
        public async Task<ActionResult<OrderResponse>> GetOrderById(long Id)
        {
            var order = await _orderService.GetOrderById(Id);
            var res = new TResponse<OrderResponse>("lây order thành công", order);
            return Ok(res);
        }
        [Authorize]
        [HttpPut("{Id}")]
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
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrders() {
            var orders = await _orderService.GetOrder();
            var res = new TResponse<IEnumerable<OrderResponse>>("lấy danh sách tất đơn hàng thành công", orders);
            return Ok(res);
        }
        [HttpPut("update-by-qr/{Id}")]
        public async Task<ActionResult> UpdateOrderByQR(long Id)
        {
            await _orderService.UpdateOrderByQR(Id);
            return Ok(new { message = "cập nhât trạng thái thành công" });
        }
  
        [HttpGet("get-top-up/{id}")]
        public async Task<ActionResult<TResponse<TopUpResponse>>> GetToUpById(long id)
        {
            var topUp = await _topupService.GetTopUpById(id);
            var res = new TResponse<TopUpResponse>("lấy hoá đơn nạp tiền thành công", topUp);
            return Ok(res);
        }
        private async Task<Customer?> GetCurrentCustomer()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username)) return null;
            return await _customerService.GetCustomerByUsername(username);
        }
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.IRepository;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Repository.Models;
using Services.IServices;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderSlotLimitController : ControllerBase
    {
        private readonly IOrderSlotLimitService _orderSlotLimitService;
        public OrderSlotLimitController(IOrderSlotLimitService orderSlotLimitService)
        {
            _orderSlotLimitService = orderSlotLimitService;
        }
        [HttpGet]
        public async Task<ActionResult<TResponse<IEnumerable<OrderSlotLimit>>>> GetAll()
        {
            var result = await _orderSlotLimitService.GetOrderSlotLimit();
            return Ok(new TResponse<IEnumerable<OrderSlotLimit>>("Lấy danh sách thành công", result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TResponse<OrderSlotLimit>>> GetById(Guid id)
        {
            var result = await _orderSlotLimitService.GetOrderSlotLimitById(id);
            return Ok(new TResponse<OrderSlotLimit>("Lấy chi tiết thành công", result));
        }

        [HttpPost]
        public async Task<ActionResult<TResponse<OrderSlotLimit>>> Create(OrderSlotLimitRequest request)
        {
            var result = await _orderSlotLimitService.AddOrderSlotLimit(request);
            return Ok(new TResponse<OrderSlotLimit>("Tạo mới thành công", result));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TResponse<OrderSlotLimit>>> Update(Guid id, OrderSlotLimitRequest request)
        {
            var result = await _orderSlotLimitService.UpdateOrderSlotLimit(id, request);
            return Ok(new TResponse<OrderSlotLimit>("Cập nhật thành công", result));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<TResponse<string>>> Delete(Guid id)
        {
            await _orderSlotLimitService.DeleteOrderSlotLimit(id);
            return Ok(new TResponse<string>("Xóa thành công", "Xóa thành công"));
        }
    }
}

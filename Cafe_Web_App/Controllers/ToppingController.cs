using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Services.IServices;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToppingController : ControllerBase
    {
        private readonly IToppingService _service;

        public ToppingController(IToppingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<TResponse<IEnumerable<Topping>>>> GetAll()
        {
            var toppings = await _service.GetTopping();
            var response = new TResponse<IEnumerable<Topping>>("Lấy danh sách thành công", toppings);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TResponse<Topping>>> GetToppingById(Guid id)
        {
            var topping = await _service.GetToppingById(id);
            var response = new TResponse<Topping>("Lấy topping thành công", topping);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<TResponse<Topping>>> CreateTopping([FromBody] ToppingRequest topping)
        {
            var newTopping = await _service.AddTopping(topping);
            var response = new TResponse<Topping>("Tạo topping thành công", newTopping);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TResponse<Topping>>> UpdateTopping(Guid id, [FromBody] ToppingRequest topping)
        {
            var updateTopping = await _service.UpdateTopping(id, topping);
            var response = new TResponse<Topping>("Cập nhật topping thành công", updateTopping);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopping(Guid id)
        {
            await _service.DeleteTopping(id);
            return Ok(new { message = "Xóa topping thành công" });
        }
    }
}

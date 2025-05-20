using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Services.IServices;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SizeController : Controller
    {
        private readonly ISizeService _sizeService;
        public SizeController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Size>>> GetAll()
        {
            var sizes = await _sizeService.GetSize();
            var response = new TResponse<IEnumerable<Size>>("lấy danh sách thành công",sizes);
            return Ok(response);
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<Size>> GetSizeById(Guid Id)
        {
            var size = await _sizeService.GetSizeById(Id);
            var response = new TResponse<Size>("lấy size thành công", size);
            return Ok(response);
        }
        [HttpPost]
        public async Task<ActionResult<Size>> CreateSize([FromBody] SizeRequest sizeRequest)
        {
            var newSize = await _sizeService.AddSize(sizeRequest);
            var response = new TResponse<Size>("tạo size thành công", newSize);
            return Ok(response);
        }
        [HttpPut("{Id}")]
        public async Task<ActionResult<Size>> UpdateSize(Guid Id,[FromBody] SizeRequest sizeRequest)
        {
            var updateSize = await _sizeService.UpdateSize(Id,sizeRequest);
            var response = new TResponse<Size>("cập nhật sản phẩm thành công",updateSize);
            return Ok(response);
        }
        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteSize(Guid Id)
        {
            await _sizeService.DeleteSize(Id);
            return Ok(new {message = "xóa size thành công"});
        }
    }
}

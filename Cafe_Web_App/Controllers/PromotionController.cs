using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Services.IServices;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PromotionController : Controller
    {
        private readonly IPromotionService _service;
      public PromotionController(IPromotionService service) {
        _service = service;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promotion>>> GetAll()
        {
            var promotions = await _service.GetPromotion();
            var response = new TResponse<IEnumerable<Promotion>>("Lấy danh sách khuyến mãi thành công",promotions);
            return Ok(response);
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<Promotion>> GetById(Guid Id)
        {
            var promotion = await _service.GetPromotionById(Id);
            var response = new TResponse<Promotion>("Lấy khuyến mãi thành công", promotion);
            return Ok(response);
        }
        [HttpGet("get-by-code")]
        public async Task<ActionResult<Promotion>> GetPromotionByCode(string code)
        {
            var promotion = await _service.GetPromotionByCode(code);
            var response = new TResponse<Promotion>("Lấy mã khuyến mãi thành công", promotion);
            return Ok(response);
        }
        [HttpPost]
        public async Task<ActionResult<Promotion>> CreatePromotion(PromotionRequest request)
        {
            var promotion = await _service.AddPromotion(request);
            var response = new TResponse<Promotion>("tạo khuyến mãi thành công", promotion);
            return Ok(response);
        }
        [HttpPut("{Id}")]
        public async Task<ActionResult<Promotion>> UpdatePromotion(Guid Id,PromotionRequest request)
        {
            var promotion = await _service.UpdatePromotion(Id,request);
            var response = new TResponse<Promotion>("cập nhật khuyến mãi thành công", promotion);
            return Ok(response);
        }
        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeletePromotion(Guid Id)
        {
            await _service.DeletePromotion(Id);
            return Ok(new { message = "Xóa promtion thành công" });
        }
    }
}

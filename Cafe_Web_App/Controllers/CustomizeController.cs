using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Services.IServices;
using static Cafe_Web_App.Controllers.CategoryController;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomizeController : ControllerBase
    {
        private readonly ICustomizeService _service;
        public CustomizeController(ICustomizeService service)
        {
            _service = service;
        }
        [HttpPost]
        public async Task<ActionResult<TResponse<CustomizeResponse>>> CreateCustomize([FromBody] CustomizeRequest request)
        {
            var customize = await _service.AddCustomize(request);
            var response = new TResponse<CustomizeResponse>("Tạo customize thành công", customize);
            return Ok(response);
        }
        [HttpGet]
        public async Task<ActionResult<TResponse<IEnumerable<Customize>>>> GetAllCustomize()
        {
            var customizes = await _service.GetCustomize();
            var response = new TResponse<IEnumerable<Customize>>(" lấy danh sách customize thành công",customizes);
            return Ok(response);
        }
        [HttpGet("{Id}")] 
        public async Task<ActionResult<Customize>> GetCustomizeById(Guid Id)
        {
            var customize = await _service.GetCustomizeById(Id);
            var response = new TResponse<Customize>("lấy customize thành công", customize);
            return Ok(response);
        }
    }
}

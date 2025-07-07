using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Repository.Models.DTOs.Response;
using Services.IServices;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<TResponse<IEnumerable<Category>>>> GetAll()
        {
            var categories = await _service.GetCategory();
            var response = new TResponse<IEnumerable<Category>>("Lấy danh sách thành công", categories);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TResponse<Category>>> GetCategoryById(Guid id)
        {
            var category = await _service.GetCategoryById(id);
            var response = new TResponse<Category>("Lấy category thành công", category);
            return Ok(response);
        }

        public class CreateCategoryRequest
        {
            public string CategoryName { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult<TResponse<Category>>> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.CategoryName))
            {
                return BadRequest("CategoryName không được để trống");
            }

            var newCategory = await _service.AddCategory(request.CategoryName);
            var response = new TResponse<Category>("Tạo category thành công", newCategory);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<TResponse<string>>> DeleteCate(Guid id)
        {
            string mess = await _service.DeleteCate(id);
            if(mess == null) {

                return BadRequest("xóa sản phẩm thất bại");
            }
            var response = new TResponse<string>("message", mess);
            return Ok(response);
        }
        public class UpdateCategoryRequest
        {
            public string CategoryName { get; set; }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TResponse<Category>>> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.CategoryName))
            {
                return BadRequest("CategoryName không được để trống");
            }

            var updatedCategory = await _service.UpdateCategory(id, request.CategoryName);
            var response = new TResponse<Category>("Cập nhật thành công", updatedCategory);
            return Ok(response);
        }
    }
}

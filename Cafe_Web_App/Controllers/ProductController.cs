using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Repository.Models;
using Services.IServices;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<TResponse<IEnumerable<Product >>>> GetAll()
        {
            var Products = await _productService.GetProduct();
            var response = new TResponse<IEnumerable<Product>>("Lấy danh sách thành công", Products);
            return Ok(response);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<TResponse<Product>>> GetProductById(Guid Id)
        {
            var Product = await _productService.GetProductById(Id);
            var response = new TResponse<Product>("Lấy Product thành công", Product);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<TResponse<ProductResponse>>> CreateProduct([FromBody] ProductRequest Product)
        {
            var newProduct = await _productService.AddProduct(Product);
            var response = new TResponse<ProductResponse>("Tạo Product thành công", newProduct);
            return Ok(response);
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<TResponse<Product>>> UpdateProduct(Guid Id, [FromBody] ProductRequest Product)
        {
            var updateProduct = await _productService.UpdateProduct(Id, Product);
            var response = new TResponse<Product>("Cập nhật Product thành công", updateProduct);
            return Ok(response);
        }
        [HttpPut("Availlable/{Id}")]
        public async Task<ActionResult<TResponse<Product>>> UpdateAvaillableProduct(Guid Id)
        {
            var updateProduct = await _productService.UpdateAvaillableProduct(Id);
            var response = new TResponse<Product>("Cập nhật Product thành công", updateProduct);
            return Ok(response);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteProduct(Guid Id)
        {
            await _productService.DeleteProduct(Id);
            return Ok(new { message = "Xóa Product thành công" });
        }
    }
}

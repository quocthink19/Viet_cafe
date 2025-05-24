using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Services.IServices;
using System.Security.Claims;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ICustomerService _customerService;
        public CartController(ICartService cartService, ICustomerService customerService)
        {
            _cartService = cartService;
            _customerService = customerService;
        }
     [Authorize]
     [HttpPost("add-to-cart")]
    public async Task<ActionResult<Cart>> AddToCart([FromBody] AddToCartRequest customize)
        {
            var customer = await GetCurrentCustomer();
            var cart = await _cartService.AddtoCart(customer.Id, customize.CustomizeId);
            var response = new TResponse<Cart>("thêm sản phẩm vào giỏ hàng thành công", cart);
            return Ok(response);
        }
        [Authorize]
        [HttpGet("get-cart-by-customer")]
        public async Task<ActionResult<Cart>> GetCartByCustomer()
            {
            var customer = await GetCurrentCustomer();
            var cart = await _cartService.GetCartByCustomerId(customer.Id);
               var response = new TResponse<Cart>("lấy giỏ hàng thành công", cart);
                return Ok(response);
            }

        [Authorize]
        [HttpPost("clear-cart")] 
        public async Task<ActionResult<Cart>> ClearCart()
        {
            var customer = await GetCurrentCustomer();
            var cart = await _cartService.ClearCart(customer.Id);
            var response = new TResponse<Cart>("clear giỏ hàng thành công", cart);
            return Ok(response);
        }
        [Authorize]
        [HttpPut("delete-cart-item")]
        public async Task<ActionResult<Cart>> DeleteCartItem([FromBody] Guid CartItemId)
        {
            var customer = await GetCurrentCustomer();
            var cart = await _cartService.DeleteCartItem(customer.Id,CartItemId);
            var response = new TResponse<Cart>("xóa cart item thành công", cart);
            return Ok(response);
        }
        private async Task<Customer?> GetCurrentCustomer()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username)) return null;
            return await _customerService.GetCustomerByUsername(username);
        }
    }
    }

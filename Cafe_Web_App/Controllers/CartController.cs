using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly ICustomerService _customerService;
        public CartController(ICartService cartService, ICustomerService customerService, IMapper mapper)
        {
            _cartService = cartService;
            _customerService = customerService;
            _mapper = mapper;   
        }
        [Authorize]
        [HttpPost("add-customize-to-cart")]
        public async Task<ActionResult<CartResponse>> AddCustomizeToCart([FromBody] CustomizeRequest customize)
        {
            var customer = await GetCurrentCustomer();
            var cart = await _cartService.AddCustomizeToCart(customer.Id, customize);
            var response = new TResponse<CartResponse>("thêm sản phẩm vào giỏ hàng thành công", cart);
            return Ok(response);
        }

        [Authorize]
     [HttpPost("add-to-cart")]
    public async Task<ActionResult<CartResponse>> AddToCart([FromBody] AddToCartRequest customize)
        {
            var customer = await GetCurrentCustomer();
            var cart = await _cartService.AddtoCart(customer.Id, customize.CustomizeId);
            var response = new TResponse<CartResponse>("thêm sản phẩm vào giỏ hàng thành công", cart);
            return Ok(response);
        }
        [Authorize]
        [HttpGet("get-cart-by-customer")]
        public async Task<ActionResult<CartResponse>> GetCartByCustomer()
            {
            var customer = await GetCurrentCustomer();
            var cart = await _cartService.GetCartByCustomerId(customer.Id);
            var res = _mapper.Map<CartResponse>(cart);
               var response = new TResponse<CartResponse>("lấy giỏ hàng thành công", res);
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
        public async Task<ActionResult<CartResponse>> DeleteCartItem([FromBody] AddToCartRequest customize)
        {
            var customer = await GetCurrentCustomer();
            var cart = await _cartService.DeleteCartItem(customer.Id,customize.CustomizeId);
            var res = _mapper.Map<CartResponse>(cart);
            var response = new TResponse<CartResponse>("xóa cart item thành công", res);
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

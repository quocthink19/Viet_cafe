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
/*
        [Authorize]
     [HttpPost("add-to-cart")]
    public async Task<ActionResult<CartResponse>> AddToCart([FromBody] AddToCartRequest customize)
        {
            var customer = await GetCurrentCustomer();
            var cart = await _cartService.AddtoCart(customer.Id, customize.CustomizeId);
            var response = new TResponse<CartResponse>("thêm sản phẩm vào giỏ hàng thành công", cart);
            return Ok(response);
        }*/
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
        [HttpGet("{Id}")]
        public async Task<ActionResult<CartResponse>> GetCartById(Guid Id)
        {
           
            var cart = await _cartService.GetCartById(Id);
            var res = _mapper.Map<CartResponse>(cart);
            var response = new TResponse<CartResponse>("lấy giỏ hàng thành công", res);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("clear-cart")] 
        public async Task<ActionResult<CartResponse>> ClearCart()
        {
            var customer = await GetCurrentCustomer();
            var cart = await _cartService.ClearCart(customer.Id);
            var res = _mapper.Map<CartResponse>(cart);
            var response = new TResponse<Cart>("clear giỏ hàng thành công", cart);
            return Ok(response);
        }

        [Authorize]
        [HttpPut("update-cart-item")]
        public async Task<ActionResult<CartResponse>> DUpdateCartItem([FromBody] UpdateCartItemRequest cartItem)
        {
            var customer = await GetCurrentCustomer();
            var cart = await _cartService.UpdateCartItem(customer.Id,cartItem.CartItemId, cartItem.newQuantity );
            var res = _mapper.Map<CartResponse>(cart);
            var response = new TResponse<CartResponse>("cập nhật cart item thành công", res);
            return Ok(response);
        }
        [Authorize]
        [HttpDelete("remove-cart-item")]
        public async Task<ActionResult<CartResponse>> DeleteCartItem([FromBody] RemoveCartItemRequest cartItem)
        {
            var customer = await GetCurrentCustomer();
            var cart = await _cartService.RemoveCartItem(customer.Id, cartItem.CartItemId);
            var response = new TResponse<CartResponse>("xóa cart item thành công", cart);
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

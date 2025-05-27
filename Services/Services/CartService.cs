using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Repository.UnitOfWork;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace Services.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ISizeService _sizeService;
        private readonly ICustomizeService _customizeService;
        private readonly IToppingService _toppingService;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper, IProductService productService, ISizeService sizeService, ICustomizeService customizeService, IToppingService toppingService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _productService = productService;
            _sizeService = sizeService;
            _customizeService = customizeService;
            _toppingService = toppingService;
            
        }

        public async Task<CartResponse> AddCustomizeToCart(Guid customerId, CustomizeRequest customizeRequest)
        {
           await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingCustomize = await _customizeService.GetExistingCustomizeAsync(customizeRequest);
                Customize customizeToUse;

                if (existingCustomize != null)
                {
                    customizeToUse = existingCustomize;
                }else
                {
                    var size = await _sizeService.GetSizeById(customizeRequest.SizeId);
                    var product = await _productService.GetProductById(customizeRequest.ProductId);
                    var extra = +size.ExtraPrice;

                    var newCustomize = new Customize
                    {
                        Id = Guid.NewGuid(),
                        Milk = customizeRequest.Milk,
                        Ice = customizeRequest.Ice,
                        Sugar = customizeRequest.Sugar,
                        SizeId = customizeRequest.SizeId,
                        ProductId = customizeRequest.ProductId,
                        Extra = 0,
                        Price = 0
                    };
                    var toppingList = new List<CustomizeTopping>();

                    if (customizeRequest.CustomizeToppings?.Any() == true)
                    {
                        foreach (var topping in customizeRequest.CustomizeToppings)
                        {
                            if (topping == null || topping.ToppingId == Guid.Empty)
                                continue;

                            var toppingEntity = await _toppingService.GetToppingById(topping.ToppingId);
                            if (toppingEntity == null)
                                continue;

                            extra += toppingEntity.Price * topping.Quantity;

                            toppingList.Add(new CustomizeTopping
                            {
                                CustomizeId = newCustomize.Id,
                                ToppingId = topping.ToppingId,
                                Quantity = topping.Quantity
                            });
                        }
                    }

                    newCustomize.Extra = extra;
                    newCustomize.Price = product.Price + extra;
                    newCustomize.CustomizeToppings = toppingList;

                    await _unitOfWork.CustomizeRepo.AddAsync(newCustomize);
                    customizeToUse = newCustomize;
                }
                var cart = await GetCartByCustomerId(customerId);
                var existingItem = await _unitOfWork.CartRepo.GetCartItem(cart.Id, customizeToUse.Id);

                if (existingItem != null)
                {
                    existingItem.Quantity += 1;
                    cart.TotalAmount += customizeToUse.Price;
                }
                else
                {
                    var newItem = new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CustomizeId = customizeToUse.Id,
                        Quantity = 1,
                        UnitPrice = customizeToUse.Price,
                        CartId = cart.Id,
                    };
                    cart.TotalAmount += customizeToUse.Price;
                    await _unitOfWork.CartRepo.InsertCartItemAsync(newItem);
                }

                await _unitOfWork.CartRepo.UpdateAsync(cart);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();

                var res = _mapper.Map<CartResponse>(cart);
                return res;


            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception($"Lỗi khi thêm tuỳ chỉnh vào giỏ hàng: {ex.Message}", ex);
            }
        }
    

        public async Task<CartResponse> AddtoCart(Guid customerId, Guid customizeId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cart = await GetCartByCustomerId(customerId);
                var customize = await _unitOfWork.CustomizeRepo.GetById(customizeId);
                if (customize == null)
                {
                    throw new Exception("Không tìm thấy sản phẩm tuỳ chỉnh");
                }
                cart.TotalAmount = await _unitOfWork.CartRepo.CalculateTotalAmount(cart.Id);
                var existingItem = await _unitOfWork.CartRepo.GetCartItem(cart.Id, customizeId);
                if (existingItem != null)
                {
                    existingItem.Quantity += 1;
                    cart.TotalAmount += customize.Price;
                }
                else
                {
                    var newItem = new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CustomizeId = customizeId,
                        Quantity = 1,
                        UnitPrice = customize.Price,
                        CartId = cart.Id,
                    };
                    cart.TotalAmount += customize.Price;

                    await _unitOfWork.CartRepo.InsertCartItemAsync(newItem);
                }

               

                await _unitOfWork.CartRepo.UpdateAsync(cart);

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
                var res = _mapper.Map<CartResponse>(cart);

                return res;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await _unitOfWork.RollbackAsync();
                var message = $"Lỗi cập nhật đồng thời: {ex.Message}";
                throw new Exception(message, ex);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                var message = $"Lỗi hệ thống: {ex.Message}";
                throw new Exception(message, ex);
            }
        }

        public async Task<Cart> ClearCart(Guid customerId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cart = await GetCartByCustomerId(customerId);

                await _unitOfWork.CartRepo.DeleteAllCartItems(cart.Id);
                cart.TotalAmount = 0;
                await _unitOfWork.CartRepo.UpdateAsync(cart);

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
                return cart;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<Cart> DeleteCartItem(Guid customerId, Guid cartItemId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cart = await GetCartByCustomerId(customerId);
                var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
                if (item == null)
                {
                    throw new Exception("Không tìm thấy sản phẩm để xóa");
                }
                double? minus = 0;
                if (item.Quantity == 1)
                {
                    await _unitOfWork.CartRepo.DeleteCartItem(item);
                    minus = item.Customize.Price;
                }
                else
                {
                    item.Quantity -= 1;
                    minus = item.Customize.Price;
                }

                cart.TotalAmount = await _unitOfWork.CartRepo.CalculateTotalAmount(cart.Id);
                cart.TotalAmount -= minus;
                await _unitOfWork.CartRepo.UpdateAsync(cart);

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();

                return cart;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<Cart> GetCartByCustomerId(Guid customerId)
        {
            var cart = await _unitOfWork.CartRepo.GetCartByCustomerId(customerId);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    CartItems = new List<CartItem>(),
                    TotalAmount = 0
                };

                await _unitOfWork.CartRepo.AddAsync(cart);
                await _unitOfWork.SaveAsync();
            }
            var res = _mapper.Map<Cart>(cart);

            return res;
        }

        public async Task<Cart> GetCartById(Guid id)
        {
            var cart = await _unitOfWork.CartRepo.GetCartById(id);
            if (cart == null)
            {
                throw new Exception("Không tìm thấy giỏ hàng");
            }
            return cart;
        }

        public async Task<Cart> UpdateCart(Guid customerId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cart = await GetCartByCustomerId(customerId);
                cart.TotalAmount = await _unitOfWork.CartRepo.CalculateTotalAmount(cart.Id);
                await _unitOfWork.CartRepo.UpdateAsync(cart);

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();

                return cart;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}

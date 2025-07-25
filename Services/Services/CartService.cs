﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Repository.Helper;
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
                var filteredToppings = (customizeRequest.CustomizeToppings ?? new List<CustomizeToppingDto>())
                    .Where(t => t != null && t.ToppingId != Guid.Empty)
                    .ToList();

                customizeRequest.CustomizeToppings = filteredToppings;

                var existingCustomize = await _customizeService.GetExistingCustomizeAsync(customizeRequest);
                Customize customizeToUse;

                var size = await _sizeService.GetSizeById(customizeRequest.SizeId);
                var product = await _productService.GetProductById(customizeRequest.ProductId);
                var extra = +size.ExtraPrice;

                if (existingCustomize != null)
                {
                    customizeToUse = existingCustomize;
                }
                else
                {
                    var newCustomize = new Customize
                    {
                        Id = Guid.NewGuid(),
                        Note = customizeRequest.Note,
                        SizeId = customizeRequest.SizeId,
                        Size = size,
                        ProductId = customizeRequest.ProductId,
                        Product = product,
                        Extra = 0,
                        Price = 0,
                    };

                    var toppingList = new List<CustomizeTopping>();

                    if (filteredToppings.Any())
                    {
                        foreach (var topping in filteredToppings)
                        {
                            var toppingEntity = await _toppingService.GetToppingById(topping.ToppingId);

                            if (toppingEntity == null)
                                continue;

                            extra += toppingEntity.Price * topping.Quantity;

                            toppingList.Add(new CustomizeTopping
                            {
                                CustomizeId = newCustomize.Id,
                                ToppingId = topping.ToppingId,
                                Quantity = topping.Quantity,
                                Topping = toppingEntity
                            });
                        }
                    }

                    newCustomize.Extra = extra;
                    newCustomize.Price = product.Price + extra;
                    newCustomize.CustomizeToppings = toppingList;

                    await _unitOfWork.CustomizeRepo.AddAsync(newCustomize);
                    customizeToUse = newCustomize;
                }

                // In ra kiểm tra toppings (tùy chọn debug)
                foreach (var ct in customizeToUse.CustomizeToppings)
                {
                    Console.WriteLine($"check : {ct.ToppingId} - {ct.Topping?.Name}");
                }

                var cart = await GetCartByCustomerId(customerId);
                var existingItem = await _unitOfWork.CartRepo.GetCartItem(cart.Id, customizeToUse.Id);

                if (existingItem != null)
                {
                    existingItem.Quantity += customizeRequest.Quantity;
                    cart.TotalAmount += customizeToUse.Price * customizeRequest.Quantity;

                    existingItem.Description = CustomizeHelper.BuildDescription(customizeToUse);
                }
                else
                {
                    var newItem = new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CustomizeId = customizeToUse.Id,
                        Quantity = customizeRequest.Quantity,
                        UnitPrice = customizeToUse.Price,
                        CartId = cart.Id,
                        imageProduct = product.Image,
                        Customize = customizeToUse,
                        Description = CustomizeHelper.BuildDescription(customizeToUse)
                    };

                    cart.TotalAmount += customizeToUse.Price * customizeRequest.Quantity;
                    await _unitOfWork.CartRepo.InsertCartItemAsync(newItem);
                }

                await _unitOfWork.CartRepo.UpdateAsync(cart);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();

                var updateCart = await _unitOfWork.CartRepo.GetCartById(cart.Id);
                var res = _mapper.Map<CartResponse>(updateCart);

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
                    throw new KeyNotFoundException("Không tìm thấy sản phẩm tuỳ chỉnh");
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

         var cartItems = await _unitOfWork.CartRepo.GetCartItemsByCartId(cart.Id);

                foreach (var item in cartItems)
                {
                    if (item.CustomizeId != Guid.Empty)
                    {
                        await _unitOfWork.CustomizeRepo.DeleteAsync(item.CustomizeId);
                    }
                }

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

        public async Task<Cart> UpdateCartItem(Guid customerId, Guid cartItemId, int newQuanity)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cart = await GetCartByCustomerId(customerId);
                var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
                if (item == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy sản phẩm để xóa");
                }
                if (newQuanity <= 0)
                {
                    throw new ArgumentException("Số lượng phải luôn lớn hơn 0");
                }
                item.Quantity = newQuanity;
                await _unitOfWork.CartRepo.UpdateAsync(cart);

                cart.TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Customize.Price);

                if (cart.TotalAmount < 0)
                {
                    cart.TotalAmount = 0;
                }
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
                throw new KeyNotFoundException("Không tìm thấy giỏ hàng");
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

        public async Task<CartResponse> RemoveCartItem(Guid customerId, Guid CartItemId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cart = await GetCartByCustomerId(customerId);
                var item = cart.CartItems.FirstOrDefault(ci => ci.Id == CartItemId);
                if (item == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy sản phẩm để xóa");
                }
                await _unitOfWork.CartRepo.DeleteCartItem(item);


                cart.TotalAmount = await _unitOfWork.CartRepo.CalculateTotalAmount(cart.Id);

                if (cart.TotalAmount < 0)
                {
                    cart.TotalAmount = 0;
                }
                await _unitOfWork.CartRepo.UpdateAsync(cart);

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();

                var res = _mapper.Map<CartResponse>(cart);
                return res;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

    }
}

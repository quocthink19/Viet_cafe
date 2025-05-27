using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Repository.UnitOfWork;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class CustomizeService : ICustomizeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISizeService _sizeService;
        private readonly IProductService _productService;
        private readonly IToppingService _ToppingService;
        private readonly IMapper _mapper;
        public CustomizeService(IUnitOfWork unitOfWork, ISizeService sizeService, IProductService productService, IToppingService toppingService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _sizeService = sizeService;
            _productService = productService;
            _ToppingService = toppingService;
            _mapper = mapper;
        }
        public async Task<CustomizeResponse> AddCustomize(CustomizeRequest customize)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var size = await _sizeService.GetSizeById(customize.SizeId);
                var product = await _productService.GetProductById(customize.ProductId);
                var extra = +size.ExtraPrice;

                var newCustomize = new Customize
                {
                    Id = Guid.NewGuid(), 
                    Milk = customize.Milk,
                    Ice = customize.Ice,
                    Sugar = customize.Sugar,
                //  Temperature = customize.Temperature,
                    SizeId = customize.SizeId,
                    ProductId = customize.ProductId,
                    Extra = 0, 
                    Price = 0
                };

                var toppingList = new List<CustomizeTopping>();
                var toppings = new List<CustomizeToppingResponse>();

                if (customize.CustomizeToppings?.Any() == true)
                {
                    foreach (var topping in customize.CustomizeToppings)
                    {
                        if (topping == null || topping.ToppingId == Guid.Empty)
                            continue;

                        var toppingEntity = await _ToppingService.GetToppingById(topping.ToppingId);
                        if (toppingEntity == null)
                            continue;

                        extra += toppingEntity.Price * topping.Quantity;

                        toppingList.Add(new CustomizeTopping
                        {
                            CustomizeId = newCustomize.Id,
                            ToppingId = topping.ToppingId,
                            Quantity = topping.Quantity
                        });
                        toppings.Add(new CustomizeToppingResponse
                        {
                            Topping = toppingEntity.Name,
                            Quantity = topping.Quantity,
                        });
                    }
                }

                newCustomize.Extra = extra;
                newCustomize.Price = product.Price + extra;
                newCustomize.CustomizeToppings = toppingList;

                await _unitOfWork.CustomizeRepo.AddAsync(newCustomize);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitAsync();

                var customizeRes = _mapper.Map<CustomizeResponse>(newCustomize);

                return customizeRes;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteCustomize(Guid Id)
        {
            var customize = await GetCustomizeById(Id);
            await _unitOfWork.CustomizeRepo.DeleteAsync(Id);
        }

        public async Task<IEnumerable<Customize>> GetCustomize()
        {
            var customizeList = await _unitOfWork.CustomizeRepo.GetAll();
            return customizeList;
        }

        public async Task<Customize> GetCustomizeById(Guid Id)
        {
            var customize = await _unitOfWork.CustomizeRepo.GetById(Id);
            if(customize == null)
            {
                throw new Exception("Lấy customize thất bại");
            }
            return customize;
        }

        public async Task<Customize?> GetExistingCustomizeAsync(CustomizeRequest customize)
        {
            var existingCustomizes = await _unitOfWork.CustomizeRepo.GetQueryable()
                .Where(c =>
            c.ProductId == customize.ProductId &&
            c.SizeId == customize.SizeId &&
            c.Milk == customize.Milk &&
            c.Ice == customize.Ice &&
            c.Sugar == customize.Sugar
            ).Include(c => c.CustomizeToppings)
            .ToListAsync();
            foreach (var cus in existingCustomizes)
            {
                var toppingsEqual = true;
               
                if (cus.CustomizeToppings.Count != customize.CustomizeToppings?.Count)
                {
                    toppingsEqual = false;
                }
                else
                {
                    
                    foreach (var topping in customize.CustomizeToppings)
                    {
                        if (!cus.CustomizeToppings.Any(t =>
                            t.ToppingId == topping.ToppingId &&
                            t.Quantity == topping.Quantity))
                        {
                            toppingsEqual = false;
                            break;
                        }
                    }
                }

                if (toppingsEqual)
                {
                    return cus;
                }
            }

            return null;
        }

        public async Task<CustomizeResponse> UpdateCustomize(Guid Id, CustomizeRequest customizeRequest)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var customize = await GetCustomizeById(Id);

                
                customize.Milk = customizeRequest.Milk;
                customize.Ice = customizeRequest.Ice;
                customize.Sugar = customizeRequest.Sugar;
        //      customize.Temperature = customizeRequest.Temperature;
                customize.SizeId = customizeRequest.SizeId;
                customize.ProductId = customizeRequest.ProductId;

                
                var size = await _sizeService.GetSizeById(customizeRequest.SizeId);
                var product = await _productService.GetProductById(customizeRequest.ProductId);
                var extra = size.ExtraPrice;

                var toppingList = new List<CustomizeTopping>();
                var toppings = new List<CustomizeToppingResponse>();

               
                customize.CustomizeToppings?.Clear();

                if (customizeRequest.CustomizeToppings?.Any() == true)
                {
                    foreach (var topping in customizeRequest.CustomizeToppings)
                    {
                        if (topping == null || topping.ToppingId == Guid.Empty)
                            continue;

                        var toppingEntity = await _ToppingService.GetToppingById(topping.ToppingId);
                        if (toppingEntity == null)
                            continue;

                        extra += toppingEntity.Price * topping.Quantity;

                        toppingList.Add(new CustomizeTopping
                        {
                            CustomizeId = customize.Id,
                            ToppingId = topping.ToppingId,
                            Quantity = topping.Quantity
                        });
                        toppings.Add(new CustomizeToppingResponse
                        {
                            Topping = toppingEntity.Name,
                            Quantity = topping.Quantity
                        });
                    }
                }

                // Update price and extra
                customize.Extra = extra;
                customize.Price = product.Price + extra;
                customize.CustomizeToppings = toppingList;

                // Update in repository
                await _unitOfWork.CustomizeRepo.UpdateAsync(customize);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitAsync();

                var customizeRes = _mapper.Map<CustomizeResponse>(customize);

                return customizeRes;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}

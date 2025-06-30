using AutoMapper;
using Azure.Core;
using Microsoft.Identity.Client;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Repository.Models.Filter;
using Repository.UnitOfWork;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ProductResponse> AddProduct(ProductRequest newProduct)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var product = new Product{
                    Name = newProduct.Name,
                    Description = newProduct.Description,
                    Price = newProduct.Price,
                    Rating = 0,
                    PurchaseCount = 0,
                    Image = newProduct.Image,
                    CategoryId = newProduct.CategoryId,
                    IsAvaillable = true
                };
                await _unitOfWork.ProductRepo.AddAsync(product);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
                var productRes  = _mapper.Map<ProductResponse>(product);

                return productRes;
            } catch (Exception) {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteProduct(Guid Id)
        {
           await _unitOfWork.BeginTransactionAsync();
            try
            {
                var product = await GetProductById(Id);
               
                await _unitOfWork.ProductRepo.DeleteAsync(Id);

                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<Product>> GetBestSeller()
        {
            var product = await _unitOfWork.ProductRepo.GetBestSeller();
            return product;
        }

        public async  Task<PagedResult<Product>> GetFilteredProductsAsync(ProductFilter filter)
        {
            return await _unitOfWork.ProductRepo.GetAllFilteredProductsAsync(filter);
        }

        public async Task<IEnumerable<Product>> GetProduct()
        {
            var products = await _unitOfWork.ProductRepo.GetAll();
            return products;
        }

        public async Task<Product> GetProductById(Guid Id)
        {
            var product = await _unitOfWork.ProductRepo.GetById(Id);
            if (product == null)
            {
                throw new Exception("không tìm thấy sản phẩm");
            }
            return product;
        }

        public async Task<Product> UpdateAvaillableProduct(Guid Id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try {
                var existingProduct = await GetProductById(Id); 

                existingProduct.IsAvaillable = !existingProduct.IsAvaillable;

                await _unitOfWork.ProductRepo.UpdateAsync(existingProduct);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitAsync();
                return existingProduct;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<Product> UpdateProduct(Guid Id, ProductRequest updateProduct)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingProduct = await GetProductById(Id);
               existingProduct.Name = updateProduct.Name;
               existingProduct.Description = updateProduct.Description;
               existingProduct.Image = updateProduct.Image;
               existingProduct.Price = updateProduct.Price;
               existingProduct.CategoryId = updateProduct.CategoryId;

                await _unitOfWork.ProductRepo.UpdateAsync(existingProduct);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitAsync();
                return existingProduct;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}

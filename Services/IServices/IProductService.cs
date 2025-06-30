using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Repository.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IProductService 
    {
        Task<PagedResult<Product>> GetFilteredProductsAsync(ProductFilter filter);

        Task<ProductResponse> AddProduct(ProductRequest newProduct);
        Task DeleteProduct(Guid Id);
        Task<Product> UpdateProduct(Guid Id, ProductRequest updateProduct);
        Task<IEnumerable<Product>> GetProduct();
        Task<Product> GetProductById(Guid Id);
        Task<Product> UpdateAvaillableProduct(Guid Id);
        Task<IEnumerable<Product>> GetBestSeller();
    }
}

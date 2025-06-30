using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IProductService 
    {
        Task<ProductResponse> AddProduct(ProductRequest newProduct);
        Task DeleteProduct(Guid Id);
        Task<Product> UpdateProduct(Guid Id, ProductRequest updateProduct);
        Task<IEnumerable<Product>> GetProduct();
        Task<Product> GetProductById(Guid Id);
        Task<Product> UpdateAvaillableProduct(Guid Id);
        Task<IEnumerable<Product>> GetBestSeller();
    }
}

using Repository.Models;
using Repository.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IProductRepo : IRepository<Product>
    {
        Task<PagedResult<Product>> GetAllFilteredProductsAsync(ProductFilter filter);
        Task<IEnumerable<Product>> GetAll();
        Task<Product> GetById(Guid id);
        Task<IEnumerable<Product>> GetBestSeller();
        Task<Guid?> GetProductIdByCartItemIdAsync(Guid cartItemId);
        Task updatePurchaseCount(Guid Id, int quanity);
    }
}

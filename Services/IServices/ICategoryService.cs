using Repository.Models.DTOs.Request;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface ICategoryService 
    {
        Task<Category> AddCategory(string categoryName);
        Task DeleteCategory(Guid Id);
        Task<Category> UpdateCategory(Guid Id, string categoryName);
        Task<IEnumerable<Category>> GetCategory();
        Task<Category> GetCategoryById(Guid Id);
    }
}

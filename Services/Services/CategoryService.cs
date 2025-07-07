using Repository.IRepository;
using Repository.Models;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepo _repo;
        public CategoryService(ICategoryRepo repo)
        {
            _repo = repo;
        }

        public async Task<Category> AddCategory(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("Tên category không được để trống hoặc null.");
            }

            var category = new Category { Name = categoryName,
            isDelete = false };
            await _repo.Add(category);
            return category;
        }

        public async Task DeleteCategory(Guid id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Không tìm thấy category để xóa.");
            }

            await _repo.DeleteAsync(id);
        }

        public async Task<string> DeleteCate(Guid id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Không tìm thấy category để xóa.");
            }
            category.isDelete = true;
            await _repo.Update(category);
            return ("xóa category thành công");
        }

        public async Task<IEnumerable<Category>> GetCategory()
        {
            var categories = await _repo.GetAll();
            return categories;
        }

        public async Task<Category> GetCategoryById(Guid id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Không tìm thấy category.");
            }
            return category;
        }

        public async Task<Category> UpdateCategory(Guid id, string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("Tên category không được để trống hoặc null.");
            }

            var category = await _repo.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Không tìm thấy category để cập nhật.");
            }

            category.Name = categoryName;
            await _repo.Update(category);

            return category;
        }
    }
}

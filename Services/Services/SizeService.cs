using Repository.IRepository;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class SizeService : ISizeService
    {
        private readonly ISizeRepo _sizeRepo;
        public SizeService(ISizeRepo sizeRepo)
        {
            _sizeRepo = sizeRepo;
        }
        public async Task<Size> AddSize(SizeRequest size)
        {
            if (string.IsNullOrWhiteSpace(size.Name) || !size.Value.HasValue)
            {
                throw new ArgumentException("Tên và thể tích là bắt buộc.");
            }

            var newSize = new Size
            {
                Name = size.Name,
                Value = size.Value.Value,
            };

            await _sizeRepo.Add(newSize);
            return newSize;
        }

        public async Task DeleteSize(Guid id)
        {
            var size = await GetSizeById(id);
            await _sizeRepo.Delete(id);
        }

        public async Task<IEnumerable<Size>> GetSize()
        {
            var sizes = await _sizeRepo.GetAllAsync();
            return sizes ?? throw new Exception("Không thể lấy danh sách size.");
        }

        public async Task<Size> GetSizeById(Guid id)
        {
            var size = await _sizeRepo.GetByIdAsync(id);
            return size;
           /*  ?? throw new KeyNotFoundException($"Size với ID {id} không tồn tại.");*/
        }

        public async Task<Size> UpdateSize(Guid id, SizeRequest size)
        {
            var existingSize = await GetSizeById(id);

            if (!string.IsNullOrWhiteSpace(size.Name))
            {
                existingSize.Name = size.Name;
            }

            if (size.Value.HasValue)
            {
                existingSize.Value = size.Value.Value;
            }

            await _sizeRepo.Update(existingSize);
            return existingSize;
        }
    }
}
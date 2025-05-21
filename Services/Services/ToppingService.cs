using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repository.IRepository;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Services.IServices;

namespace Services.Services
{
    public class ToppingService : IToppingService
    {
        private readonly IToppingRepo _repo;

        public ToppingService(IToppingRepo repo)
        {
            _repo = repo;
        }

        public async Task<Topping> AddTopping(ToppingRequest topping)
        {
            if (string.IsNullOrWhiteSpace(topping.Name))
            {
                throw new ArgumentException("Tên topping không được để trống hoặc null.");
            }

         

            var newTopping = new Topping
            {
                Name = topping.Name,
                Price = topping.Price
            };

            await _repo.Add(newTopping);
            return newTopping;
        }

        public async Task DeleteTopping(Guid id)
        {
            var topping = await GetToppingById(id);
            await _repo.Delete(id);
        }

        public async Task<IEnumerable<Topping>> GetTopping()
        {
            var toppings = await _repo.GetAllAsync();
            if (toppings == null)
            {
                throw new Exception("Lấy danh sách topping thất bại");
            }
            return toppings;
        }

        public async Task<Topping> GetToppingById(Guid id)
        {
            var topping = await _repo.GetByIdAsync(id);
            if (topping == null)
            {
                throw new KeyNotFoundException("Không tìm thấy topping với Id này.");
            }
            return topping;
        }

        public async Task<Topping> UpdateTopping(Guid id, ToppingRequest topping)
        {
            if (string.IsNullOrWhiteSpace(topping.Name))
            {
                throw new ArgumentException("Tên topping không được để trống hoặc null.");
            }

            var updateTopping = await GetToppingById(id);

            updateTopping.Name = topping.Name;
            updateTopping.Price = topping.Price;

            await _repo.Update(updateTopping);
            return updateTopping;
        }
    }
}

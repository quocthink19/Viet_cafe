using Repository.Models;
using Repository.Models.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IToppingService
    {
        Task<Topping> AddTopping(ToppingRequest Topping);
        Task DeleteTopping(Guid Id);
        Task<Topping> UpdateTopping(Guid Id, ToppingRequest Topping);
        Task<IEnumerable<Topping>> GetTopping();
        Task<Topping> GetToppingById(Guid Id);
    }
}

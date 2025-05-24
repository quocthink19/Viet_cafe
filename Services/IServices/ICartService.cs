using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface ICartService
    {
        Task<Cart> GetCartByCustomerId(Guid CustomerId);
        Task<Cart> GetCartById(Guid Id);
        Task<Cart> ClearCart(Guid Id);
        Task<Cart> AddtoCart(Guid customerId,Guid CustomizeId);
        Task<Cart> UpdateCart(Guid customerId);
        Task<Cart> DeleteCartItem(Guid customerId, Guid CartItemId);
    }
}

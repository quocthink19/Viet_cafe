using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ICartRepo : IRepository<Cart> 
    {
        Task<Cart> GetCartByCustomerId(Guid customerId);
        Task<Cart?> GetCartById(Guid id);
        Task<CartItem> GetCartItem(Guid CartId, Guid CustomizeId);
        Task DeleteCartItem(CartItem item);

        Task InsertCartItemAsync(CartItem item);
        Task<double?> CalculateTotalAmount(Guid cartId);
        Task DeleteAllCartItems(Guid cartId);
    }
}

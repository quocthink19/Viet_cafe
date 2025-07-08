using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IWalletHistoryService
    {
        Task<IEnumerable<WalletHistory>> GetWalletHistoryByCustomerId(Guid customerId);
        Task<WalletHistory> GetWalletById(Guid id);
    }
}

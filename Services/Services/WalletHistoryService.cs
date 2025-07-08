using Repository.Models;
using Repository.UnitOfWork;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class WalletHistoryService : IWalletHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public WalletHistoryService(IUnitOfWork unitOfWork) {
        _unitOfWork = unitOfWork;
        }

        public async Task<WalletHistory> GetWalletById(Guid id)
        {
            var history = await _unitOfWork.WalletHistoryRepo.GetByIdAsync(id);
            if(history == null)
            {
                throw new KeyNotFoundException("không tìm thấy lịch sử nào");
            }
            return history;
        }

        public async Task<IEnumerable<WalletHistory>> GetWalletHistoryByCustomerId(Guid customerId)
        {
            var histories = await _unitOfWork.WalletHistoryRepo.GetByCustomerID(customerId);
            if(histories == null)
            {
                throw new ArgumentException("không có lịch sử nào ");
            }
            return histories;
        }
    }
}

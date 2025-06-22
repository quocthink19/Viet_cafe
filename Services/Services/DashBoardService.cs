using Repository.Models.DTOs.Response;
using Repository.UnitOfWork;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashBoardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DailyStatsResponse?> GetDailyStatsAsync(DateTime date)
        {
           var Daily = await _unitOfWork.OrderRepo.GetDailyStatsAsync(date);
            if (Daily == null) {
                throw new ArgumentException($"không tìm thấy thống kê của ngày {date} ");
            }
           return Daily;
        }

        public async Task<MonthlyStatsResponse?> GetMonthlyStatsAsync(int year, int month)
        {
            var monthly = await _unitOfWork.OrderRepo.GetMonthlyStatsAsync(year,month);
            if(monthly == null)
            {
                throw new ArgumentException($"không tim thấy thông kê của tháng {month} năm {year}");
            }
            return monthly;
        }

        public async Task<TodayStatsRessponse> GetTodayStatsAsync()
        {
           var today = await _unitOfWork.OrderRepo.GetTodayStatsAsync();
            if (today == null)
            {
                throw new ArgumentException("không tìm thấy thông kê của ngày hôm nay ");
            }
            return today;
        }
    }
}

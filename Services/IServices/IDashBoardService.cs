using Repository.Models.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IDashBoardService
    {
        Task<DailyStatsResponse?> GetDailyStatsAsync(DateTime date);
        Task<MonthlyStatsResponse?> GetMonthlyStatsAsync(int year, int month);
        Task<TodayStatsRessponse> GetTodayStatsAsync();
    }
}

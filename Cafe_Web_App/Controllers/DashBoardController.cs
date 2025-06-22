using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Services.IServices;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DashBoardController : ControllerBase
    {
        private readonly IDashBoardService _dashBoardService;
        public DashBoardController(IDashBoardService dashBoardService)
        {
            _dashBoardService = dashBoardService;
        }
        [Authorize]
        [HttpGet("today-stats")]
        public async Task<ActionResult<TResponse<TodayStatsRessponse>>> GetToday()
        {
            var today = await _dashBoardService.GetTodayStatsAsync();
            var res = new TResponse<TodayStatsRessponse>("lấy thông kê hôm nay thành công ", today);
            return Ok(res);
        }
        [Authorize]
        [HttpGet("any-date-stats")]
        public async Task<ActionResult<TResponse<DailyStatsResponse>>> GetAnyday([FromQuery] DailyRequest req)
        {
            var daily = await _dashBoardService.GetDailyStatsAsync(req.date);
            var res = new TResponse<DailyStatsResponse>($"lấy thông kê ngày {req.date} thành công", daily);
            return Ok(res);
        }

        [Authorize]
        [HttpGet("monthly-stats")]
        public async Task<ActionResult<TResponse<MonthlyStatsResponse>>> GetMonth([FromQuery] MothlyRequest req)
        {
            var monthly = await _dashBoardService.GetMonthlyStatsAsync(req.year , req.month);
            var res = new TResponse<MonthlyStatsResponse>($"lây thông kê tháng {req.month} năm {req.year} thành công", monthly);
            return Ok(res);
        }
    }
}

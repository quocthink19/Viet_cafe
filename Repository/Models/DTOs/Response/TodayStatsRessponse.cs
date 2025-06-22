using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Response
{
    public  class TodayStatsRessponse
    {
        public DateTime Date {  get; set; }
        public int TotalOrders { get; set; }
        public double TotalRevenue { get; set; }
    }
}

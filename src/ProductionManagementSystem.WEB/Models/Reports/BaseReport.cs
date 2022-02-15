using Microsoft.AspNetCore.Mvc.Rendering;
using ProductionManagementSystem.WEB.Models.Charts;

namespace ProductionManagementSystem.WEB.Models.Reports
{
    public class BaseReport
    {
        public SelectList Entities { get; set; }
        public BarChartData BarChart { get; set; }
    }
}

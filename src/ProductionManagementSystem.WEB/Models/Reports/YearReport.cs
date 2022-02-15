using Microsoft.AspNetCore.Mvc.Rendering;
using ProductionManagementSystem.WEB.Models.Charts;

namespace ProductionManagementSystem.WEB.Models.Reports
{
    public class YearReport : BaseReport
    {
        public SelectList Years { get; set; }
        public SelectList Entities { get; set; }
        public BarChartData BarChart { get; set; }
    }
}
using ProductionManagementSystem.WEB.Models.Charts;

namespace ProductionManagementSystem.WEB.Models.Reports
{
    public class MonthReport
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public BarChartData BarChart { get; set; }
    }
}
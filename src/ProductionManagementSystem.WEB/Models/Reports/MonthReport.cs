using Microsoft.AspNetCore.Mvc.Rendering;
using ProductionManagementSystem.WEB.Models.Charts;

namespace ProductionManagementSystem.WEB.Models.Reports
{
    public class MonthReport : BaseReport
    {
        public SelectList Years { get; set; }
        public SelectList Months { get; set; }
    }
}
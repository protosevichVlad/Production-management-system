using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.WEB.Models.Charts;
using ProductionManagementSystem.WEB.Models.Reports;
using ProductionManagementSystem.WEB.Services;

namespace ProductionManagementSystem.WEB.Controllers
{
    [Authorize(Roles=RoleEnum.Admin)]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> MontageMonthReport()
        {
            var montageMonthReport = await _reportService.GetMontageMonthReportAsync(2021, 12);
            return View(new MonthReport()
            {
                BarChart = ChartService.ElementDiffToBarChart(await  _reportService.GroupByDateAsync(montageMonthReport, "dd.MM"))
            });
        }
    }
}
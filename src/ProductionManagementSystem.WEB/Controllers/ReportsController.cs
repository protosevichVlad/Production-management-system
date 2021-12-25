using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.WEB.Models.Charts;
using ProductionManagementSystem.WEB.Models.Reports;
using ProductionManagementSystem.WEB.Services;

namespace ProductionManagementSystem.WEB.Controllers
{
    public class MonthStructure
    {
        public int Key { get; set; }
        public string Text { get; set; }
    }
    
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

        public async Task<IActionResult> MontageMonthReport(int? month, int? year, int? montageId)
        {
            if (!month.HasValue) month = DateTime.Now.Month;
            if (!year.HasValue) year = DateTime.Now.Year;

            if (month < 1 || month > 12) month = DateTime.Now.Month;
            if (year < 2020) year = DateTime.Now.Year;
            
            var montageMonthReport = await _reportService.GetMontageMonthReportAsync(year.Value, month.Value, montageId);
            return View(new MonthReport()
            {
                Months = GetMonths(month),
                Years = GetYears(year),
                BarChart = ChartService.ElementDiffToBarChart(await  _reportService.GroupByDateAsync(montageMonthReport, "dd.MM"))
            });
        }
        
        public async Task<IActionResult> MontageYearReport(int? year, int? montageId)
        {
            if (!year.HasValue) year = DateTime.Now.Year;

            if (year < 2020) year = DateTime.Now.Year;
            
            var montageMonthReport = await _reportService.GetMontageYearReportAsync(year.Value, montageId);
            return View(new YearReport()
            {
                Years = GetYears(year),
                BarChart = ChartService.ElementDiffToBarChart(await  _reportService.GroupByDateAsync(montageMonthReport, "MMMM"))
            });
        }

        private SelectList GetMonths(int? selected=null)
        {
            List<MonthStructure> result = new List<MonthStructure>();
            for (int i = 1; i <= 12; i++)
            {
                result.Add(new MonthStructure()
                {
                    Key = i, 
                    Text = new DateTime(2020, i, 1).ToString("MMMM")
                });
            }
            return new SelectList(result, "Key", "Text", selected);
        }
        
        private SelectList GetYears(int? selected=null)
        {
            List<int> result = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++)
            {
                result.Add(i);
            }
            return new SelectList(result, selected);
        }
    }
}
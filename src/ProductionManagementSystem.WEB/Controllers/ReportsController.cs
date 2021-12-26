using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductionManagementSystem.Core.Models.ElementsDifference;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Services;
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
        private readonly IMontageService _montageService;
        private readonly IDesignService _designService;
        private readonly IDeviceService _deviceService;

        public ReportsController(IReportService reportService, IMontageService montageService, IDesignService designService, IDeviceService deviceService)
        {
            _reportService = reportService;
            _montageService = montageService;
            _designService = designService;
            _deviceService = deviceService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> MontageMonthReport(int? month, int? year, int entityId=-1)
        {
            if (!month.HasValue) month = DateTime.Now.Month;
            if (!year.HasValue) year = DateTime.Now.Year;

            if (month < 1 || month > 12) month = DateTime.Now.Month;
            if (year < 2020) year = DateTime.Now.Year;
            
            return View("MonthReport", await GetMonthReport(ElementType.Montage, year.Value, month.Value, entityId));
        }
        
        public async Task<IActionResult> MontageYearReport(int? year, int entityId=-1)
        {
            if (!year.HasValue) year = DateTime.Now.Year;

            if (year < 2020) year = DateTime.Now.Year;
            
            return View("YearReport", await GetYearReport(ElementType.Montage, year.Value, entityId));
        }
        
        public async  Task<IActionResult> MontagePeriodReport(DateTime? from, DateTime? to, int entityId=-1, string groupBy="dd.MM")
        {
            if (!from.HasValue) from = DateTime.Now.AddMonths(-1);
            if (!to.HasValue) to = DateTime.Now;
            if (from > to) return View("PeriodReport");

            return View("PeriodReport", await GetPeriodReport(ElementType.Montage, from.Value, to.Value, entityId, groupBy));
        }
        
        public async Task<IActionResult> DesignMonthReport(int? month, int? year, int entityId=-1)
        {
            if (!month.HasValue) month = DateTime.Now.Month;
            if (!year.HasValue) year = DateTime.Now.Year;

            if (month < 1 || month > 12) month = DateTime.Now.Month;
            if (year < 2020) year = DateTime.Now.Year;
            
            return View("MonthReport", await GetMonthReport(ElementType.Design, year.Value, month.Value, entityId));
        }
        
        public async Task<IActionResult> DesignYearReport(int? year, int entityId=-1)
        {
            if (!year.HasValue) year = DateTime.Now.Year;

            if (year < 2020) year = DateTime.Now.Year;
            
            return View("YearReport", await GetYearReport(ElementType.Design, year.Value, entityId));
        }
        
        public async Task<IActionResult> DesignPeriodReport(DateTime? from, DateTime? to, int entityId=-1, string groupBy="dd.MM")
        {
            if (!from.HasValue) from = DateTime.Now.AddMonths(-1);
            if (!to.HasValue) to = DateTime.Now;
            if (from > to) return View("PeriodReport");

            return View("PeriodReport", await GetPeriodReport(ElementType.Design, from.Value, to.Value, entityId, groupBy));
        }
        
        public async Task<IActionResult> DeviceMonthReport(int? month, int? year, int entityId=-1)
        {
            if (!month.HasValue) month = DateTime.Now.Month;
            if (!year.HasValue) year = DateTime.Now.Year;

            if (month < 1 || month > 12) month = DateTime.Now.Month;
            if (year < 2020) year = DateTime.Now.Year;
            
            return View("MonthReport", await GetMonthReport(ElementType.Device, year.Value, month.Value, entityId));
        }
        
        public async Task<IActionResult> DeviceYearReport(int? year, int entityId=-1)
        {
            if (!year.HasValue) year = DateTime.Now.Year;

            if (year < 2020) year = DateTime.Now.Year;
            
            return View("YearReport", await GetYearReport(ElementType.Device, year.Value, entityId));
        }
        
        public async Task<IActionResult> DevicePeriodReport(DateTime? from, DateTime? to, int entityId=-1, string groupBy="dd.MM")
        {
            if (!from.HasValue) from = DateTime.Now.AddMonths(-1);
            if (!to.HasValue) to = DateTime.Now;
            if (from > to) return View("PeriodReport");

            return View("PeriodReport", await GetPeriodReport(ElementType.Device, from.Value, to.Value, entityId, groupBy));
        }

        private async Task<MonthReport> GetMonthReport(ElementType type, int year, int month, int entityId)
        {
            var report = await GetBaseReport(type, new DateTime(year, month, 1),
                new DateTime(year, month, 1).AddMonths(1).AddSeconds(-1), entityId, "dd.MM");
            MonthReport monthReport = new MonthReport();
            monthReport.Entities = report.Entities;
            monthReport.BarChart = report.BarChart;
            monthReport.Years = GetYears(year);
            monthReport.Months = GetMonths(month);
            return monthReport;
        }
        
        private async Task<YearReport> GetYearReport(ElementType type, int year, int entityId)
        {
            var report = await GetBaseReport(type, new DateTime(year, 1, 1),
                new DateTime(year, 1, 1).AddYears(1).AddSeconds(-1), entityId, "MMMM");
            YearReport yearReport = new YearReport();
            yearReport.Entities = report.Entities;
            yearReport.BarChart = report.BarChart;
            yearReport.Years = GetYears(year);
            return yearReport;
        }
        
        private async Task<PeriodReport> GetPeriodReport(ElementType type, DateTime from, DateTime to, int entityId,
            string groupBy)
        {
            var report = await GetBaseReport(type, from, to, entityId, groupBy);
            PeriodReport periodReport = new PeriodReport();
            periodReport.Entities = report.Entities;
            periodReport.BarChart = report.BarChart;
            periodReport.GroupBy = GetListOfGroup(groupBy);
            periodReport.From = from;
            periodReport.To = from;
            return periodReport;
        }
        
        private async Task<BaseReport> GetBaseReport(ElementType type, DateTime from, DateTime to, int entityId, string groupBy)
        {
            var periodReport = await _reportService.GetPeriodReportAsync(type, from, to, entityId);
                
            var chart = ChartService.ElementDiffToBarChart(
                await _reportService.GroupByDateAsync(periodReport, groupBy));
            
            var entities = new List<KeyValuePair<int, string>>() {new KeyValuePair<int, string>(-1, "Все")};
            entities.AddRange(type switch
            {
                ElementType.Montage => await _montageService.GetListForSelectAsync(),
                ElementType.Design => await _designService.GetListForSelectAsync(),
                ElementType.Device => await _deviceService.GetListForSelectAsync(),
                _ => new List<KeyValuePair<int, string>>(),
            });
            
            var selectedList = new SelectList(entities, "Key", "Value", entityId);
            chart.Label = entities.Find(x => x.Key == entityId).Value;

            return new BaseReport()
            {
                Entities = selectedList,
                BarChart = chart,
            };
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

        private SelectList GetListOfGroup(string selected)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            result.Add(new KeyValuePair<string, string>("dd.MM", "По дням"));
            result.Add(new KeyValuePair<string, string>("MM.yyyy", "По месяцам"));
            result.Add(new KeyValuePair<string, string>("yyyy", "По годам"));
            return new SelectList(result, "Key", "Value", selected);
        }
    }
}
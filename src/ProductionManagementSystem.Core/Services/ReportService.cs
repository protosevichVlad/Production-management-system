using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models.ElementsDifference;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services
{
    public interface IReportService
    {
        Task<List<ElementDifference>> GetMontageMonthReportAsync(int year, int month, int? montageId=null);
    }
    
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _db;

        public ReportService(IUnitOfWork db)
        {
            _db = db;
        }


        public async Task<List<ElementDifference>> GetMontageMonthReportAsync(int year, int month, int? montageId=null)
        {
            DateTime from = new DateTime(year, month, 1);
            DateTime to = new DateTime(year, month, 1).AddMonths(1).AddSeconds(-1);
            return await _db.ElementDifferenceRepository.GetByPeriodGroupByDayAsync(ElementType.Montage, from, to);
        }
    }
}
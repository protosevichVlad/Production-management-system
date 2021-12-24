﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models.ElementsDifference;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services
{
    public interface IReportService
    {
        Task<List<ElementDifference>> GetMontageMonthReportAsync(int year, int month, int? montageId=null);

        Task<List<ElementDifference>> GroupByDateAsync(List<ElementDifference> elementDifferences, string dateFormat);
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
            var result =
                await _db.ElementDifferenceRepository.GetByPeriodGroupByDayAsync(ElementType.Montage, from, to);
            result.AddRange(GenerateElementDifferences(from, to));
            return result;
        }

        public async Task<List<ElementDifference>> GroupByDateAsync(List<ElementDifference> elementDifferences,
            string dateFormat)
        {
            var orderedEnumerable = elementDifferences.OrderBy(x => x.DateTime);
            var dates = orderedEnumerable.Select(x => x.DateTime.ToString(dateFormat)).Distinct().ToList();
            var result = new List<ElementDifference>();
            foreach (var elementDifference in orderedEnumerable)
            {
                int dateIndex = dates.IndexOf(elementDifference.DateTime.ToString(dateFormat));
                if (result.Count() <= dateIndex)
                {
                    result.Add(elementDifference);
                }
                else
                {
                    result[dateIndex].Difference += elementDifference.Difference;
                }
            }

            return result;
        }

        private List<ElementDifference> GenerateElementDifferences(DateTime from, DateTime to)
        {
            List<ElementDifference> result = new List<ElementDifference>((to - from).Days);
            for (; from < to; from = from.AddDays(1))
            {
                result.Add(new ElementDifference()
                {
                    DateTime = from
                });
            }

            return result;
        }
    }
}
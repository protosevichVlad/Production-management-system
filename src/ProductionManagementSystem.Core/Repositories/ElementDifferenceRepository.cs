using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.ElementsDifference;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface IElementDifferenceRepository : IRepository<ElementDifference>
    {
        Task<List<ElementDifference>> GetByPeriodAsync(ElementType type, DateTime from, DateTime to);
        Task<List<ElementDifference>>  GetByPeriodGroupByMonthAsync(ElementType type, DateTime from, DateTime to);
        Task<List<ElementDifference>>  GetByPeriodGroupByDayAsync(ElementType type, DateTime from, DateTime to);
    }
    
    public class ElementDifferenceRepository : Repository<ElementDifference>, IElementDifferenceRepository
    {
        public ElementDifferenceRepository(ApplicationContext context) : base(context)
        {
        }

        public async  Task<List<ElementDifference>> GetByPeriodAsync(ElementType type, DateTime from, DateTime to)
        {
            return _dbSet
                .Where(x => x.ElementType == type && x.DateTime <= to && x.DateTime >= from)
                .Select((Func<ElementDifference, ElementDifference>)(x =>
                {
                    x.Element = x.ElementType switch
                    {
                        ElementType.Design => _db.Designs.Find(x.ElementId),
                        ElementType.Montage => _db.Montages.Find(x.ElementId),
                        ElementType.Device => _db.Devices.Find(x.ElementId),
                    };
                    return x;
                }))
                .OrderBy(x => x.DateTime)
                .ToList();
        }

        public async Task<List<ElementDifference>> GetByPeriodGroupByMonthAsync(ElementType type, DateTime from,
            DateTime to)
        {
            return _dbSet
                .Where(x => x.ElementType == type && x.DateTime <= to && x.DateTime >= from)
                .GroupBy(x => new {x.DateTime.Month, x.DateTime.Year, x.ElementId})
                .Select(x => new ElementDifference()
                {
                    DateTime = new DateTime(x.Key.Year, x.Key.Month, 1),
                    ElementId = x.Key.ElementId,
                    Difference = x.ToList().Sum(x => x.Difference),
                })
                .ToList()
                .Select((Func<ElementDifference, ElementDifference>)(x =>
                {
                    x.Element = x.ElementType switch
                    {
                        ElementType.Design => _db.Designs.Find(x.ElementId),
                        ElementType.Montage => _db.Montages.Find(x.ElementId),
                        ElementType.Device => _db.Devices.Find(x.ElementId),
                    };
                    return x;
                }))
                .OrderBy(x => x.DateTime)
                .ToList();
        }

        public async Task<List<ElementDifference>> GetByPeriodGroupByDayAsync(ElementType type, DateTime from, DateTime to)
        {
            return _dbSet
                .Where(x => x.ElementType == type && x.DateTime <= to && x.DateTime >= from)
                .GroupBy(x => new {x.DateTime.Month, x.DateTime.Year, x.DateTime.Day, x.ElementId})
                .Select(x => new ElementDifference()
                {
                    DateTime = new DateTime(x.Key.Year, x.Key.Month, x.Key.Day),
                    ElementId = x.Key.ElementId,
                    Difference = x.ToList().Sum(x => x.Difference),
                })
                .ToList()
                .Select((Func<ElementDifference, ElementDifference>)(x =>
                {
                    x.Element = x.ElementType switch
                    {
                        ElementType.Design => _db.Designs.Find(x.ElementId),
                        ElementType.Montage => _db.Montages.Find(x.ElementId),
                        ElementType.Device => _db.Devices.Find(x.ElementId),
                    };
                    return x;
                }))
                .OrderBy(x => x.DateTime)
                .ToList();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Exceptions;
using ProductionManagementSystem.Core.Models;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface ICompDbDeviceRepository : IRepository<CompDbDevice>
    {
        Task<List<CompDbDevice>> SearchByKeyWordAsync(string s);
    }
    
    public class CompDbDeviceRepository : Repository<CompDbDevice>, ICompDbDeviceRepository
    {
        private IEntityExtRepository _entityExtRepository;
        private IUsedItemRepository _usedItemRepository;

        public CompDbDeviceRepository(ApplicationContext db) : base(db)
        {
            _entityExtRepository = new EntityExtRepository(db);
            _usedItemRepository = new UsedItemRepository(db);
        }

        public async Task<List<CompDbDevice>> SearchByKeyWordAsync(string s)
        {
            return await _dbSet.Where(x => (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(s))
                                           || !string.IsNullOrEmpty(x.Variant) && x.Variant.Contains(s)
                                           || !string.IsNullOrEmpty(x.Description) && x.Description.Contains(s))
                .ToListAsync();
        }

        public override async Task UpdateAsync(CompDbDevice item)
        {
            var ent = await _usedItemRepository.GetByDeviceIdAsync(item.Id);
            ent = ent.Where(x => item.UsedItems.All(y => y.Id != x.Id)).ToList();
            _db.UsedItems.RemoveRange(ent); 
            foreach (var used in item.UsedItems)
            {
                _db.Entry(used).State = used.Id == 0 ? EntityState.Added : EntityState.Modified;
            }

            await base.UpdateAsync(item);
        }

        public override async Task<CompDbDevice> GetByIdAsync(int id)
        {
            var device = await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (device == null) return null;
            device.UsedItems = await _usedItemRepository.GetByDeviceIdAsync(id);
            return device;
        }

        public override void Delete(CompDbDevice item)
        {
            var used = _db.UsedItems.Any(x => x.ItemType == CDBItemType.Device && x.ItemId == item.Id);
            if (used)
            {
                throw new DeleteReferenceException("Device delete not permitted", "This device used in other device");
            }
            
            base.Delete(item);
        }
    }
}
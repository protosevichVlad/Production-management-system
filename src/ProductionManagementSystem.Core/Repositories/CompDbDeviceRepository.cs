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
        public CompDbDeviceRepository(ApplicationContext db) : base(db)
        {
            _entityExtRepository = new EntityExtRepository(db);
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
            _dbSet.Update(item);
        }

        public override async Task<CompDbDevice> GetByIdAsync(int id)
        {
            var device = await base.GetByIdAsync(id);
            device.UsedInDevice = _db.UsedInDevice.Where(x => x.CompDbDeviceId == id).ToList();
            device.UsedInDevice = device.UsedInDevice.Select(x =>
            {
                x.Component = x.ComponentType switch
                {
                    UsedInDeviceComponentType.Device => _db.CompDbDevices.Find(x.UsedComponentId),
                    UsedInDeviceComponentType.Entity => _entityExtRepository.GetByIdAsync(x.UsedComponentId).Result,
                    UsedInDeviceComponentType.PCB => _db.Projects.Find(x.UsedComponentId),
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                return x;
            }).ToList();
            return device;
        }

        public override void Delete(CompDbDevice item)
        {
            var used = _db.UsedInDevice.Any(x => x.ComponentType == UsedInDeviceComponentType.Device && x.UsedComponentId == item.Id);
            if (used)
            {
                throw new DeleteReferenceException("Device delete not permitted", "This device used in other device");
            }
            base.Delete(item);
        }
    }
}
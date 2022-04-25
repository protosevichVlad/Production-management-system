using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface ICDBObtainedRepository : IRepository<CDBObtained>
    {
        
    }

    public class CDBObtainedRepository : Repository<CDBObtained>, ICDBObtainedRepository
    {
        private readonly IEntityExtRepository _entityExtRepository;

        public CDBObtainedRepository(ApplicationContext db) : base(db)
        {
            _entityExtRepository = new EntityExtRepository(db);
        }

        public override async Task<CDBObtained> GetByIdAsync(int id)
        {
            var obtained = await base.GetByIdAsync(id);
            await InitUniversalItem(obtained);
            return obtained;
        }

        public override async Task<List<CDBObtained>> GetAllAsync()
        {
            var obtained = await base.GetAllAsync();
            await InitUniversalItem(obtained);
            return obtained;
        }

        public override async Task<List<CDBObtained>> FindAsync(Func<CDBObtained, bool> predicate, string includeProperty = null)
        {
            var obtained = await base.FindAsync(predicate, includeProperty);
            await InitUniversalItem(obtained);
            return obtained;
        }

        private async Task InitUniversalItem(CDBObtained obtained)
        {
            if (obtained == null) return;

            obtained.Task = await _db.CdbTasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == obtained.TaskId);
            object item = obtained.Task.ItemType switch
            {
                CDBItemType.Device => await _db.Devices.FindAsync(obtained.TaskId),
                CDBItemType.PCB => await _db.Projects.FindAsync(obtained.TaskId),
                CDBItemType.Entity => await _entityExtRepository.GetByIdAsync(obtained.TaskId),
                _ => throw new ArgumentOutOfRangeException()
            };

            obtained.ObtainedItem = new UniversalItem(item);
        }
        
        private async Task InitUniversalItem(List<CDBObtained> obtained)
        {
            obtained.ForEach(async o => await InitUniversalItem(o));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface ICDBTasksRepository : IRepository<CDBTask>
    {
        
    }

    public class CDBTasksRepository : Repository<CDBTask>, ICDBTasksRepository
    {
        private readonly IEntityExtRepository _entityExtRepository;

        public CDBTasksRepository(ApplicationContext db) : base(db)
        {
            _entityExtRepository = new EntityExtRepository(db);
        }

        public override async Task<CDBTask> GetByIdAsync(int id)
        {
            var task = await _db.CdbTasks
                .Include(x => x.Obtained)
                .ThenInclude(y => y.UsedItem)
                .FirstOrDefaultAsync(x => x.Id == id);
            await Include(task);
            return task;
        }

        public override async Task<List<CDBTask>> FindAsync(Func<CDBTask, bool> predicate, string includeProperty = null)
        {
            var tasks = await base.FindAsync(predicate, includeProperty);
            await Include(tasks);
            return tasks;
        }

        public override async Task<List<CDBTask>> GetAllAsync()
        {
            var tasks = await _db.CdbTasks
                .Include(x => x.Obtained)
                .ThenInclude(y => y.UsedItem)
                .ToListAsync();
            await Include(tasks);
            return tasks;
        }

        private async Task Include(CDBTask task)
        {
            if (task == null)
                return;
            
            object taskItem = task.ItemType switch
            {
                CDBItemType.Device => await _db.CDBDevices.FindAsync(task.ItemId),
                CDBItemType.PCB => await _db.Projects.FindAsync(task.ItemId),
                CDBItemType.Entity => await _entityExtRepository.GetByIdAsync(task.ItemId),
                _ => throw new ArgumentOutOfRangeException()
            };

            task.Item = new UniversalItem(taskItem);

            for (int i = 0; i < task.Obtained.Count; i++)
            {
                task.Obtained[i].UsedItem.Item = new UniversalItem(task.Obtained[i].UsedItem.ItemType switch
                {
                    CDBItemType.Device => await _db.CDBDevices.FindAsync(task.Obtained[i].UsedItem.ItemId),
                    CDBItemType.PCB => await _db.Projects.FindAsync(task.Obtained[i].UsedItem.ItemId),
                    CDBItemType.Entity => await _entityExtRepository.GetByIdAsync(task.Obtained[i].UsedItem.ItemId),
                    _ => throw new ArgumentOutOfRangeException()
                });
            }
        }

        private async Task Include(List<CDBTask> tasks)
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                await Include(tasks[i]);
            }
        }
    }
}
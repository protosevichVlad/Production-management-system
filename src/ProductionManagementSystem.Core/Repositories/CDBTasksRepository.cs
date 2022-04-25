using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            var task = await base.GetByIdAsync(id);
            if (task == null) return null;
            await InitUniversalItem(task);
            return task;
        }

        public override async Task<List<CDBTask>> FindAsync(Func<CDBTask, bool> predicate, string includeProperty = null)
        {
            var tasks = await base.FindAsync(predicate, includeProperty);
            await InitUniversalItem(tasks);
            return tasks;
        }

        public override async Task<List<CDBTask>> GetAllAsync()
        {
            var tasks = await base.GetAllAsync();
            await InitUniversalItem(tasks);
            return tasks;
        }

        private async Task InitUniversalItem(CDBTask task)
        {
            object taskItem = task.TaskItem.ItemType switch
            {
                CDBItemType.Device => await _db.Devices.FindAsync(task.TaskItemId),
                CDBItemType.PCB => await _db.Projects.FindAsync(task.TaskItemId),
                CDBItemType.Entity => await _entityExtRepository.GetByIdAsync(task.TaskItemId),
                _ => throw new ArgumentOutOfRangeException()
            };

            task.TaskItem = new UniversalItem(taskItem);
        }

        private async Task InitUniversalItem(List<CDBTask> tasks)
        {
            tasks.ForEach(async t => await InitUniversalItem(t));
        }
    }
}
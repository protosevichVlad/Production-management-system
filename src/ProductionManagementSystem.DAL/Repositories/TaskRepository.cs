using System;
using System.Collections.Generic;
using System.Linq;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Tasks;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface ITaskRepository : IRepository<Task>
    {
        
    }
    public class TaskRepository : Repository<Task>, ITaskRepository
    {
        public TaskRepository(ApplicationContext db) : base(db)
        {
        }

        public override async System.Threading.Tasks.Task<IEnumerable<Task>> GetAllAsync()
        {
            var tasks = await base.GetAllAsync();
            foreach (var task in tasks)
            {
                task.ObtainedDesigns = _db.ObtainedDesigns.Where(d => d.TaskId == task.Id);
                task.ObtainedMontages = _db.ObtainedMontages.Where(m => m.TaskId == task.Id);
            }

            return tasks;
        }

        public override async System.Threading.Tasks.Task<Task> GetByIdAsync(int id)
        {
            var task = await base.GetByIdAsync(id);
            if (task == null)
                return null;
            
            task.ObtainedDesigns = _db.ObtainedDesigns.Where(d => d.TaskId == task.Id);
            task.ObtainedMontages = _db.ObtainedMontages.Where(m => m.TaskId == task.Id);

            return task;
        }

        public override async System.Threading.Tasks.Task<IEnumerable<Task>> FindAsync(Func<Task, bool> predicate)
        {
            var tasks = await base.FindAsync(predicate);
            foreach (var task in tasks)
            {
                task.ObtainedDesigns = _db.ObtainedDesigns.Where(d => d.TaskId == task.Id);
                task.ObtainedMontages = _db.ObtainedMontages.Where(m => m.TaskId == task.Id);
            }

            return tasks;
        }

        public override void Delete(Task item)
        {
            _db.ObtainedMontages.RemoveRange(_db.ObtainedMontages.Where(m => m.TaskId == item.Id));
            _db.ObtainedDesigns.RemoveRange(_db.ObtainedDesigns.Where(d => d.TaskId == item.Id));
            
            base.Delete(item);
        }

        public override async System.Threading.Tasks.Task CreateAsync(Task item)
        {
            if (item.ObtainedDesigns != null)
                await _db.ObtainedDesigns.AddRangeAsync(item.ObtainedDesigns);
            if(item.ObtainedMontages != null)
                await _db.ObtainedMontages.AddRangeAsync(item.ObtainedMontages);
            
            await base.CreateAsync(item);
        }
    }
}
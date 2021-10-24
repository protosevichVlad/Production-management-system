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

        public override IEnumerable<Task> GetAll()
        {
            var tasks = base.GetAll();
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
            task.ObtainedDesigns = _db.ObtainedDesigns.Where(d => d.TaskId == task.Id);
            task.ObtainedMontages = _db.ObtainedMontages.Where(m => m.TaskId == task.Id);

            return task;
        }

        public override IEnumerable<Task> Find(Func<Task, bool> predicate)
        {
            var tasks = base.Find(predicate);
            foreach (var task in tasks)
            {
                task.ObtainedDesigns = _db.ObtainedDesigns.Where(d => d.TaskId == task.Id);
                task.ObtainedMontages = _db.ObtainedMontages.Where(m => m.TaskId == task.Id);
            }

            return tasks;
        }

        public override void Delete(Task item)
        {
            _db.ObtainedDesigns.RemoveRange(item.ObtainedDesigns);
            _db.ObtainedMontages.RemoveRange(item.ObtainedMontages);
            base.Delete(item);
        }

        public override void Update(Task item)
        {
            _db.ObtainedDesigns.UpdateRange(item.ObtainedDesigns);
            _db.ObtainedMontages.UpdateRange(item.ObtainedMontages);
            base.Update(item);
        }

        public override async System.Threading.Tasks.Task CreateAsync(Task item)
        {
            await _db.ObtainedDesigns.AddRangeAsync(item.ObtainedDesigns);
            await _db.ObtainedMontages.AddRangeAsync(item.ObtainedMontages);
            await base.CreateAsync(item);
        }
    }
}
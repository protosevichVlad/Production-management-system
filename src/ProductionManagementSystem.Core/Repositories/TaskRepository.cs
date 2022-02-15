using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.Tasks;

namespace ProductionManagementSystem.Core.Repositories
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
            var tasks = (await base.GetAllAsync()).ToList();
            foreach (var task in tasks)
                await InitTaskAsync(task);

            return tasks;
        }

        public override async System.Threading.Tasks.Task<Task> GetByIdAsync(int id)
        {
            var task = await base.GetByIdAsync(id);
            if (task == null)
                return null;
            
            await InitTaskAsync(task);
            return task;
        }

        public override async System.Threading.Tasks.Task<IEnumerable<Task>> FindAsync(Func<Task, bool> predicate)
        {
            var tasks = (await base.FindAsync(predicate)).ToList();
            foreach (var task in tasks)
                await InitTaskAsync(task);

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
            await base.CreateAsync(item);
            await _db.SaveChangesAsync();
            
            if (item.ObtainedDesigns != null)
                await _db.ObtainedDesigns.AddRangeAsync(item.ObtainedDesigns.Select(d =>
                {
                    d.TaskId = item.Id;
                    return d;
                }));
            if(item.ObtainedMontages != null)
                await _db.ObtainedMontages.AddRangeAsync(item.ObtainedMontages.Select(m =>
                {
                    m.TaskId = item.Id;
                    return m;
                }));
        }

        private async System.Threading.Tasks.Task InitTaskAsync(Task task)
        {
            task.ObtainedDesigns = await _db.ObtainedDesigns.Where(d => d.TaskId == task.Id).ToListAsync();
            task.ObtainedMontages = await _db.ObtainedMontages.Where(m => m.TaskId == task.Id).ToListAsync();
            foreach (var obtainedMontage in task.ObtainedMontages)
                obtainedMontage.Montage = await _db.Montages.FindAsync(obtainedMontage.ComponentId);
            foreach (var obtainedDesign in task.ObtainedDesigns)
                obtainedDesign.Design = await _db.Designs.FindAsync(obtainedDesign.ComponentId);

            task.Device = await _db.Devices.FindAsync(task.DeviceId);
            if (task.Device == null)
                return;
            
            task.Device.Designs = await _db.DesignInDevices.Where(d => d.DeviceId == task.Device.Id).ToListAsync();
            task.Device.Montages = await _db.MontageInDevices.Where(m => m.DeviceId == task.Device.Id).ToListAsync();
            foreach (var montage in task.Device.Montages)
                montage.Montage = await _db.Montages.FindAsync(montage.ComponentId);
            foreach (var design in task.Device.Designs)
                design.Design = await _db.Designs.FindAsync(design.ComponentId);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;
using Task = ProductionManagementSystem.Core.Models.Tasks.Task;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface ITaskRepository : IRepository<Task>
    {
        Task<bool> ContainDeviceAsync(int taskId, int deviceId);
        Task<List<Task>> GetTasksByDeviceIdAsync(int deviceId);
    }
    public class TaskRepository : Repository<Task>, ITaskRepository
    {
        public TaskRepository(ApplicationContext db) : base(db)
        {
        }

        public override async System.Threading.Tasks.Task<List<Task>> GetAllAsync()
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

        public override async System.Threading.Tasks.Task<List<Task>> FindAsync(Func<Task, bool> predicate, string includeProperty=null)
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

        public async Task<bool> ContainDeviceAsync(int taskId, int deviceId)
        {
            return await _db.DevicesInTasks.AnyAsync(x => x.TaskId == taskId && x.DeviceId == deviceId);
        }

        public async Task<List<Task>> GetTasksByDeviceIdAsync(int deviceId)
        {
            return await _db.DevicesInTasks
                .Where(x => x.DeviceId == deviceId)
                .Include(x => x.Task)
                .Select(x => x.Task).ToListAsync();
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

            task.Devices = await _db.DevicesInTasks
                .Where(d => d.TaskId == task.Id)
                .Include(x => x.Device)
                .ToListAsync();
            if (task.Devices == null)
                return;

            for (int i = 0; i < task.Devices.Count; i++)
            {
                task.Devices[i].Device.Designs = await _db.DesignInDevices.Where(d => d.DeviceId == task.Devices[i].Device.Id).ToListAsync();
                task.Devices[i].Device.Montages = await _db.MontageInDevices.Where(m => m.DeviceId == task.Devices[i].Device.Id).ToListAsync();
                
                foreach (var montage in task.Devices[i].Device.Montages)
                    montage.Montage = await _db.Montages.FindAsync(montage.ComponentId);
                foreach (var design in task.Devices[i].Device.Designs)
                    design.Design = await _db.Designs.FindAsync(design.ComponentId);
            }
        }
    }
}
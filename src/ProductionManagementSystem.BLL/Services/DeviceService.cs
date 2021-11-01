using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Components;
using ProductionManagementSystem.Models.Devices;
using ProductionManagementSystem.Models.Logs;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.BLL.Services
{
    public interface IDeviceService : IBaseService<Device>
    {
        Task<IEnumerable<string>> GetNamesAsync();
        Task AddDeviceAsync(int? id);
        Task ReceiveDeviceAsync(int? id);
        Task DeleteByIdAsync(int id);
        Task<IEnumerable<Montage>> GetMontagesFromDeviceByDeviceId(int deviceId);
        Task<IEnumerable<Design>> GetDesignsFromDeviceByDeviceId(int deviceId);
    }

    public class DeviceService : BaseService<Device>, IDeviceService
    {
        private readonly ILogService _log;
        
        public DeviceService(IUnitOfWork uow) : base(uow)
        {
            _log = new LogService(uow);
            _currentRepository = _db.DeviceRepository;
        }
        
        public override async Task UpdateAsync(Device device)
        {
            var checkInTask = await CheckInTaskAsync(device);
            string errorMessage = checkInTask.Item2;
            if (!checkInTask.Item1)
            {
                throw new IntersectionOfEntitiesException("Ошибка. Невозможно изменение прибора.", errorMessage);
            }
            
            await base.UpdateAsync(device);
        }

        public override async Task DeleteAsync(Device device)
        {
            var checkInTask = (await CheckInTaskAsync(device));
            string errorMessage = checkInTask.Item2;
            if (!checkInTask.Item1)
            {
                throw new IntersectionOfEntitiesException("Ошибка. Невозможно удаление прибора.", errorMessage);
            }

            await base.DeleteAsync(device);
        }

        private async Task<Tuple<bool, string>> CheckInTaskAsync(Device device)
        {
            string errorMessage;
            var task = (await _db.TaskRepository.GetAllAsync())
                .FirstOrDefault(t => device.Id == t.DeviceId);
            if (task != null)
            {
                errorMessage = $"<i class='bg-light'>{device.ToString()}</i> используется в <i class='bg-light'>задаче №{task.Id}</i>.<br />";
                return new Tuple<bool, string>(false, errorMessage);
            }
            
            errorMessage = String.Empty;
            return new Tuple<bool, string>(true, errorMessage);
        }

        public async Task<IEnumerable<string>> GetNamesAsync()
        {
            var devices = await GetAllAsync();
            if (devices == null)
            {
                return Array.Empty<string>();
            }

            return devices.Select(d => d.ToString());
        }
        
        public async Task ReceiveDeviceAsync(int? id)
        {
            await AddDeviceAsync(id, -1);
        }
        
        public async Task AddDeviceAsync(int? id)
        {
            await AddDeviceAsync(id, 1);
        }

        public async Task DeleteByIdAsync(int id)
        {
            await this.DeleteAsync(new Device() {Id = id});
        }

        public async Task<IEnumerable<Montage>> GetMontagesFromDeviceByDeviceId(int deviceId)
        {
            return (await _db.MontageInDeviceRepository.FindAsync(m => m.DeviceId == deviceId)).Select(async md => await _db.MontageRepository.GetByIdAsync(md.ComponentId))
                .Select(t => t.Result);
        }

        public async Task<IEnumerable<Design>> GetDesignsFromDeviceByDeviceId(int deviceId)
        {
            return (await _db.DesignInDeviceRepository.FindAsync(m => m.DeviceId == deviceId)).Select(async md => await _db.DesignRepository.GetByIdAsync(md.ComponentId))
                .Select(t => t.Result);
        }
        
        private async Task AddDeviceAsync(int? id, int quantity)
        {
            if (!id.HasValue)
            {
                throw new PageNotFoundException();
            }

            var device = await GetByIdAsync(id.Value);
            device.Quantity += quantity;
            await UpdateAsync(device);

            if (quantity < 0)
            {
                await _log.CreateAsync(new Log() { Message = $"Был получен прибор {device} со склада {quantity}шт.", DeviceId = device.Id});
            }
            else
            {
                await _log.CreateAsync(new Log() { Message = $"Был добавлен прибор {device} на склад {-quantity}шт.", DeviceId = device.Id});
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Components;
using ProductionManagementSystem.Models.Devices;
using ProductionManagementSystem.Models.Logs;

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

    public class DeviceService : BaseServiceWithLogs<Device>, IDeviceService
    {
        public DeviceService(IUnitOfWork uow) : base(uow)
        {
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
                errorMessage = $"<i class='bg-light'>{device}</i> используется в <i class='bg-light'>задаче №{task.Id}</i>.<br />";
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
            await DeleteAsync(new Device {Id = id});
        }

        public async Task<IEnumerable<Montage>> GetMontagesFromDeviceByDeviceId(int deviceId)
        {
            return (await _db.MontageInDeviceRepository.FindAsync(m => m.DeviceId == deviceId)).ToList()
                .Select(async md => await _db.MontageRepository.GetByIdAsync(md.ComponentId))
                .Select(t => t.Result).Where(t => t != null).ToList();
        }

        public async Task<IEnumerable<Design>> GetDesignsFromDeviceByDeviceId(int deviceId)
        {
            return (await _db.DesignInDeviceRepository.FindAsync(m => m.DeviceId == deviceId)).ToList()
                .Select(async md => await _db.DesignRepository.GetByIdAsync(md.ComponentId))
                .Select(t => t.Result).Where(t => t != null).ToList();
        }
        
        private async Task AddDeviceAsync(int? id, int quantity)
        {
            if (!id.HasValue)
            {
                throw new PageNotFoundException();
            }

            var device = await _currentRepository.GetByIdAsync(id.Value);
            device.Quantity += quantity;
            await _currentRepository.UpdateAsync(device);

            if (quantity < 0)
            {
                await _db.LogRepository.CreateAsync(new Log { Message = $"Был получен прибор {device} со склада {quantity}шт.", DeviceId = device.Id});
            }
            else
            {
                await _db.LogRepository.CreateAsync(new Log { Message = $"Был добавлен прибор {device} на склад {-quantity}шт.", DeviceId = device.Id});
            }
        }
        
        protected override async Task CreateLogForCreatingAsync(Device item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Был создан прибор " + item, DeviceId = item.Id });
        }

        protected override async Task CreateLogForUpdatingAsync(Device item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Был изменён прибор " + item, DeviceId = item.Id });
        }

        protected override async Task CreateLogForDeletingAsync(Device item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Был удалён прибор " + item, DeviceId = item.Id });
        }
    }
}
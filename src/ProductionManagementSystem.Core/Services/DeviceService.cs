using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Infrastructure;
using ProductionManagementSystem.Core.Models.Components;
using ProductionManagementSystem.Core.Models.Devices;
using ProductionManagementSystem.Core.Models.ElementsDifference;
using ProductionManagementSystem.Core.Models.Logs;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services
{
    public interface IDeviceService : IBaseService<Device>
    {
        Task<IEnumerable<string>> GetNamesAsync();
        Task AddDeviceAsync(int? id);
        Task ReceiveDeviceAsync(int? id);
        Task DeleteByIdAsync(int id);
        Task<IEnumerable<KeyValuePair<int, string>>> GetListForSelectAsync();
        Task<List<Montage>> GetMontagesFromDeviceByDeviceId(int deviceId);
        Task<List<Design>> GetDesignsFromDeviceByDeviceId(int deviceId);
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

        public async Task<List<Montage>> GetMontagesFromDeviceByDeviceId(int deviceId)
        {
            return (await _db.MontageInDeviceRepository.FindAsync(m => m.DeviceId == deviceId)).Distinct(new ComponentBaseInDeviceEqualityComparer()).ToList()
                .Select(async md => await _db.MontageRepository.GetByIdAsync(md.ComponentId))
                .Select(t => t.Result).Where(t => t != null).ToList();
        }

        public async Task<List<Design>> GetDesignsFromDeviceByDeviceId(int deviceId)
        {
            return (await _db.DesignInDeviceRepository.FindAsync(m => m.DeviceId == deviceId)).Distinct(new ComponentBaseInDeviceEqualityComparer()).ToList()
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
            
            await _db.ElementDifferenceRepository.CreateAsync(new ElementDifference()
                {Difference = quantity, ElementId = device.Id, ElementType = ElementType.Design});

            if (quantity < 0)
            {
                await _db.LogRepository.CreateAsync(new Log { Message = $"Был получен прибор {device} со склада {quantity}шт.", DeviceId = device.Id});
            }
            else
            {
                await _db.LogRepository.CreateAsync(new Log { Message = $"Был добавлен прибор {device} на склад {-quantity}шт.", DeviceId = device.Id});
            }
            
            await _db.SaveAsync();
        }
        
        public async Task<IEnumerable<KeyValuePair<int, string>>> GetListForSelectAsync()
        {
            return (await GetAllAsync()).Select(x => new KeyValuePair<int, string>(x.Id, x.ToString()));
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
        
        protected override bool UpdateLogPredicate(Log log, Device item) => log.DeviceId == item.Id; 

        protected override void UpdateLog(Log log) => log.DeviceId = null;
    }
}
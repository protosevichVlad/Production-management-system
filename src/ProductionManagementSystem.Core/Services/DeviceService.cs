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

        protected override LogsItemType ItemType => LogsItemType.Device;

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
            var tasks = await _db.TaskRepository.GetTasksByDeviceIdAsync(device.Id);
            if (tasks.Count > 0)
            {
                errorMessage = $"<i class='bg-light'>{device}</i> используется в <i class='bg-light'>задаче №{tasks[0].Id}</i>.<br />";
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
                await _db.LogRepository.CreateAsync(new Log { Message = $"Был получен прибор {device} со склада {quantity}шт.", ItemId = device.Id, ItemType = LogsItemType.Device});
            }
            else
            {
                await _db.LogRepository.CreateAsync(new Log { Message = $"Был добавлен прибор {device} на склад {-quantity}шт.", ItemId = device.Id, ItemType = LogsItemType.Device});
            }
            
            await _db.SaveAsync();
        }
        
        public async Task<IEnumerable<KeyValuePair<int, string>>> GetListForSelectAsync()
        {
            return (await GetAllAsync()).Select(x => new KeyValuePair<int, string>(x.Id, x.ToString()));
        }

        protected override object GetPropValue(Device src, string propName)
        {
            if (propName == nameof(src.Montages) && src.Montages != null)
                return "<br />&nbsp;&nbsp;" + string.Join("<br/>&nbsp;&nbsp;", src.Montages.Select(async x => $"{x.Montage ?? (await _db.MontageRepository.FindAsync(y => y.Id == x.ComponentId)).FirstOrDefault()} {x.Quantity}шт").Select(x => x.Result));
            
            if (propName == nameof(src.Designs) && src.Designs != null)
                return "<br />&nbsp;&nbsp;" + string.Join("<br/>&nbsp;&nbsp;", src.Designs.Select(async x => $"{x.Design ?? (await _db.DesignRepository.FindAsync(y => y.Id == x.ComponentId)).FirstOrDefault()} {x.Quantity}шт").Select(x => x.Result));
            
            return base.GetPropValue(src, propName);
        }
    }
}
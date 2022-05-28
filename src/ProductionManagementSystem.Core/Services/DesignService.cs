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
    public interface IComponentBaseService<T> : IBaseService<T>
        where T : ComponentBase
    {
        Task<List<string>> GetTypesAsync();
        Task<List<T>> GetByDeviceId(int deviceId);
        Task IncreaseQuantityAsync(int id, int quantity);
        Task DecreaseQuantityAsync(int id, int quantity);
        Task<List<KeyValuePair<int, string>>> GetListForSelectAsync();
        Task DeleteByIdAsync(int id);
        Task UsingInDevice(List<T> components);
    }

    public interface IDesignService : IComponentBaseService<Design>
    {
    }

    public class DesignService : BaseServiceWithLogs<Design>, IDesignService
    {
        public DesignService(IUnitOfWork uow) : base(uow)
        {
            _currentRepository = _db.DesignRepository;
        }

        protected override LogsItemType ItemType => LogsItemType.Design;

        public override async Task DeleteAsync(Design design)
        {
            var checkInDevices = (await CheckInDevicesAsync(design));
            string errorMessage = checkInDevices.Item2;
            if (!checkInDevices.Item1)
            {
                throw new IntersectionOfEntitiesException("Ошибка. Невозможно удаление конструктива.", errorMessage);
            }

            await base.DeleteAsync(design);
        }

        public async Task DeleteByIdAsync(int id)
        {
            await DeleteAsync(new Design {Id = id});
        }

        public async Task UsingInDevice(List<Design> components)
        {
            foreach (var component in components)
            {
                var deviceIds = (await _db.DesignInDeviceRepository.FindAsync(x => x.ComponentId == component.Id)).Select(x =>
                        x.DeviceId);
                component.Devices = new List<Device>();
                foreach (var deviceId in deviceIds)
                {
                    component.Devices.Add(await _db.DeviceRepository.GetByIdAsync(deviceId));
                }
            }
        }

        public async Task<List<string>> GetTypesAsync()
        {
            var designs = await GetAllAsync();
            List<string> types = designs.Select(d => d.Type).Distinct().OrderBy(d => d).ToList();
            return types;
        }

        public async Task<List<Design>> GetByDeviceId(int deviceId)
        {
            return (await _db.DesignInDeviceRepository.FindAsync(m => m.DeviceId == deviceId)).Distinct(new ComponentBaseInDeviceEqualityComparer()).ToList()
                .Select(async md => await _db.DesignRepository.GetByIdAsync(md.ComponentId))
                .Select(t => t.Result).Where(t => t != null).ToList();
        }

        public async Task IncreaseQuantityAsync(int id, int quantity)
        {
            if (quantity == 0)
            {
                return;
            }

            var design = await _currentRepository.GetByIdAsync(id);
            design.Quantity += quantity;
            await _currentRepository.UpdateAsync(design);

            await _db.ElementDifferenceRepository.CreateAsync(new ElementDifference()
                {Difference = quantity, ElementId = design.Id, ElementType = ElementType.Design});
            
            if (quantity < 0)
            {
                await _db.LogRepository.CreateAsync(new Log {Message = $"Было получено {-quantity}ед. конструктива {design}", ItemId = design.Id, ItemType = LogsItemType.Design});
            }
            else
            {
                await _db.LogRepository.CreateAsync(new Log {Message = $"Было добавлено {quantity}ед. конструктива {design}", ItemId = design.Id, ItemType = LogsItemType.Design});
            }
            
            await _db.SaveAsync();
        }

        public async Task<List<KeyValuePair<int, string>>> GetListForSelectAsync()
        {
            return (await GetAllAsync()).Select(x => new KeyValuePair<int, string>(x.Id, x.ToString())).ToList();
        }

        public async Task DecreaseQuantityAsync(int id, int quantity)
        {
            await IncreaseQuantityAsync(id, -quantity);
        }
        

        private async Task<Tuple<bool, string>> CheckInDevicesAsync(Design design)
        {
            string errorMessage;
            var designInDevice = (await _db.DesignInDeviceRepository.GetAllAsync())
                .FirstOrDefault(d => design.Id == d.ComponentId);
            if (designInDevice != null)
            {
                var device = (await _db.DeviceRepository.GetAllAsync()).FirstOrDefault(d => d.Id == designInDevice.DeviceId);
                errorMessage = $"<i class='bg-light'>{design}</i> используется в <i class='bg-light'>{device}</i>.<br />" +
                               $"Для удаления <i class='bg-light'>{design}</i>, удалите <i class='bg-light'>{device}</i>.<br />";
                return new Tuple<bool, string>(false, errorMessage);
            }

            errorMessage = String.Empty;
            return new Tuple<bool, string>(true, errorMessage);
        }
    }
}
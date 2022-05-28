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
    
    public interface IMontageService : IComponentBaseService<Montage>
    {
    }

    public class MontageService : BaseServiceWithLogs<Montage>, IMontageService
    {
        public MontageService(IUnitOfWork uow) : base(uow)
        {
            _currentRepository = _db.MontageRepository;
        }

        protected override LogsItemType ItemType => LogsItemType.Montage;

        public override async Task DeleteAsync(Montage montage)
        {
            var checkInDevices = (await CheckInDevicesAsync(montage));
            string errorMessage = checkInDevices.Item2;
            if (!checkInDevices.Item1)
            {
                throw new IntersectionOfEntitiesException("Ошибка. Невозможно удаление монтажа.", errorMessage);
            }

            await base.DeleteAsync(montage);
        }

        public async Task<List<string>> GetTypesAsync()
        {
            var montages = await GetAllAsync();
            List<string> types = montages.OrderBy(c => c.Type).Select(c => c.Type).Distinct().ToList();
            return types;
        }

        public async Task<List<Montage>> GetByDeviceId(int deviceId)
        {
            return (await _db.MontageInDeviceRepository.FindAsync(m => m.DeviceId == deviceId)).Distinct(new ComponentBaseInDeviceEqualityComparer()).ToList()
                .Select(async md => await _db.MontageRepository.GetByIdAsync(md.ComponentId))
                .Select(t => t.Result).Where(t => t != null).ToList();
        }

        public async Task IncreaseQuantityAsync(int id, int quantity)
        {
            if (quantity == 0)
            {
                return;
            }

            var montage = await _currentRepository.GetByIdAsync(id);
            montage.Quantity += quantity;
            await _currentRepository.UpdateAsync(montage);

            await _db.ElementDifferenceRepository.CreateAsync(new ElementDifference()
                {Difference = quantity, ElementId = montage.Id, ElementType = ElementType.Montage});
            
            if (quantity < 0)
            {
                await _db.LogRepository.CreateAsync(new Log {Message = $"Было получено {-quantity}ед. монтажа {montage}", ItemId = montage.Id, ItemType = LogsItemType.Montage});
            }
            else
            {
                await _db.LogRepository.CreateAsync(new Log {Message = $"Было добавлено {quantity}ед. монтажа {montage}", ItemId = montage.Id, ItemType = LogsItemType.Montage});
            }

            await _db.SaveAsync();
        }

        public async Task DecreaseQuantityAsync(int id, int quantity)
        {
            await IncreaseQuantityAsync(id, -quantity);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="montage"></param>
        /// <returns>Return true, if component not using in devices.</returns>
        private async Task<Tuple<bool, string>> CheckInDevicesAsync(Montage montage)
        {
            string errorMessage;
            var montageInDevice = (await _db.MontageInDeviceRepository.GetAllAsync())
                .FirstOrDefault(c => montage.Id == c.ComponentId);
            if (montageInDevice != null)
            {
                var device = (await _db.DeviceRepository.GetAllAsync()).FirstOrDefault(d => d.Id == montageInDevice.DeviceId);
                errorMessage = $"<i class='bg-light'>{montage}</i> используется в <i class='bg-light'>{device}</i>.<br />" +
                               $"Для удаления <i class='bg-light'>{montage}</i>, удалите <i class='bg-light'>{device}</i>.<br />";
                return new Tuple<bool, string>(false, errorMessage);
            }

            errorMessage = String.Empty;
            return new Tuple<bool, string>(true, errorMessage);
        }

        public async Task<List<KeyValuePair<int, string>>> GetListForSelectAsync()
        {
            return (await GetAllAsync()).Select(x => new KeyValuePair<int, string>(x.Id, x.ToString())).ToList();
        }

        public async Task UsingInDevice(List<Montage> components)
        {
            foreach (var component in components)
            {
                var deviceIds = (await _db.MontageInDeviceRepository.FindAsync(x => x.ComponentId == component.Id)).Select(x =>
                    x.DeviceId);
                component.Devices = new List<Device>();
                foreach (var deviceId in deviceIds)
                {
                    component.Devices.Add(await _db.DeviceRepository.GetByIdAsync(deviceId));
                }
            }
        }
    }
}
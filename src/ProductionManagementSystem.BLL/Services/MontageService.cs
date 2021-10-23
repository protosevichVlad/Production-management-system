using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Components;
using ProductionManagementSystem.Models.Logs;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.BLL.Services
{
    public interface IMontageService : IBaseService<Montage>
    {
        Task<IEnumerable<string>> GetTypesAsync();
        Task IncreaseQuantityOfMontageAsync(int id, int quantity);
        Task DecreaseQuantityOfDesignAsync(int id, int quantity);
    }

    public class MontageService : BaseService<Montage>, IMontageService
    {
        private readonly ILogService _log;
        
        public MontageService(IUnitOfWork uow) : base(uow)
        {
            _log = new LogService(uow);
        }
        
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

        public async Task<IEnumerable<string>> GetTypesAsync()
        {
            var montages = GetAll().ToList();
            IEnumerable<string> types = montages.OrderBy(c => c.Type).Select(c => c.Type).Distinct();
            return types;
        }

        public async Task IncreaseQuantityOfMontageAsync(int id, int quantity)
        {
            if (quantity == 0)
            {
                return;
            }

            var montage = await GetByIdAsync(id);
            montage.Quantity += quantity;
            Update(montage);

            if (quantity < 0)
            {
                await _log.CreateAsync(new Log() {Message = $"Было получено {-quantity}ед. монтажа {montage}", MontageId = montage.Id});
            }
            else
            {
                await _log.CreateAsync(new Log() {Message = $"Было добавлено {quantity}ед. монтажа {montage}", MontageId = montage.Id});
            }
        }

        public async Task DecreaseQuantityOfDesignAsync(int id, int quantity)
        {
            await this.IncreaseQuantityOfMontageAsync(id, -quantity);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns>Return true, if component not using in devices.</returns>
        private async Task<Tuple<bool, string>> CheckInDevicesAsync(Montage montage)
        {
            string errorMessage;
            var montageInDevice = _db.MontageInDeviceRepository.GetAll()
                .FirstOrDefault(c => montage.Id == c.ComponentId);
            if (montageInDevice != null)
            {
                var device = _db.DeviceRepository.GetAll().FirstOrDefault(d => d.Id == montageInDevice.DeviceId);
                errorMessage = $"<i class='bg-light'>{montage}</i> используется в <i class='bg-light'>{device}</i>.<br />" +
                               $"Для удаления <i class='bg-light'>{montage}</i>, удалите <i class='bg-light'>{device}</i>.<br />";
                return new Tuple<bool, string>(false, errorMessage);
            }

            errorMessage = String.Empty;
            return new Tuple<bool, string>(true, errorMessage);
        }
        
        private async Task<bool> ComponentExistsAsync(int id)
        {
            return GetAll().Any(e => e.Id == id);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Components;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.BLL.Services
{
    public interface IDesignService : IBaseService<Design>
    {
        Task<IEnumerable<string>> GetTypesAsync();
        Task IncreaseQuantityOfDesignAsync(int id, int quantity);
        Task DecreaseQuantityOfDesignAsync(int id, int quantity);
    }

    public class DesignService : BaseService<Design>, IDesignService
    {
        private readonly ILogService _log;
        
        public DesignService(IUnitOfWork uow) : base(uow)
        {
            _log = new LogService(_db);
        }

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

        public async Task<IEnumerable<string>> GetTypesAsync()
        {
            var designs = GetAll().ToList();
            IEnumerable<string> types = designs.Select(d => d.Type).Distinct().OrderBy(d => d);
            return types;
        }

        public async Task IncreaseQuantityOfDesignAsync(int id, int quantity)
        {
            if (quantity == 0)
            {
                return;
            }

            var design = await GetByIdAsync(id);
            design.Quantity += quantity;
            Update(design);
            
            if (quantity < 0)
            {
                await _log.CreateLogAsync(new LogDTO($"Было получено {-quantity}ед. конструктива {design}"){DesignId = design.Id});
            }
            else
            {
                await _log.CreateLogAsync(new LogDTO($"Было добавлено {quantity}ед. конструктива {design}"){DesignId = design.Id});
            }
        }
        
        public async Task DecreaseQuantityOfDesignAsync(int id, int quantity)
        {
            await this.IncreaseQuantityOfDesignAsync(id, -quantity);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
        
        private async Task<Tuple<bool, string>> CheckInDevicesAsync(Design design)
        {
            string errorMessage;
            var designInDevice = _db.DesignInDeviceRepository.GetAll()
                .FirstOrDefault(d => design.Id == d.ComponentId);
            if (designInDevice != null)
            {
                // TODO: use device Service
                var device = _db.DeviceRepository.GetAll().FirstOrDefault(d => d.Id == designInDevice.DeviceId);
                errorMessage = $"<i class='bg-light'>{design}</i> используется в <i class='bg-light'>{device}</i>.<br />" +
                               $"Для удаления <i class='bg-light'>{design}</i>, удалите <i class='bg-light'>{device}</i>.<br />";
                return new Tuple<bool, string>(false, errorMessage);
            }

            errorMessage = String.Empty;
            return new Tuple<bool, string>(true, errorMessage);
        }
        
                
        private async Task<bool> DesignExistsAsync(int id)
        {
            return _db.DesignRepository.GetAll().Any(e => e.Id == id);
        }
    }
}
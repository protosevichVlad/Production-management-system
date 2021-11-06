using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Components;
using ProductionManagementSystem.Models.Logs;

namespace ProductionManagementSystem.BLL.Services
{
    public interface IDesignService : IBaseService<Design>
    {
        Task<IEnumerable<string>> GetTypesAsync();
        Task IncreaseQuantityOfDesignAsync(int id, int quantity);
        Task DecreaseQuantityOfDesignAsync(int id, int quantity);
        Task DeleteByIdAsync(int id);
    }

    public class DesignService : BaseServiceWithLogs<Design>, IDesignService
    {
        public DesignService(IUnitOfWork uow) : base(uow)
        {
            _currentRepository = _db.DesignRepository;
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

        public async Task DeleteByIdAsync(int id)
        {
            await DeleteAsync(new Design {Id = id});
        }

        public async Task<IEnumerable<string>> GetTypesAsync()
        {
            var designs = await GetAllAsync();
            IEnumerable<string> types = designs.Select(d => d.Type).Distinct().OrderBy(d => d);
            return types;
        }

        public async Task IncreaseQuantityOfDesignAsync(int id, int quantity)
        {
            if (quantity == 0)
            {
                return;
            }

            var design = await _currentRepository.GetByIdAsync(id);
            design.Quantity += quantity;
            await _currentRepository.UpdateAsync(design);
            
            if (quantity < 0)
            {
                await _db.LogRepository.CreateAsync(new Log {Message = $"Было получено {-quantity}ед. конструктива {design}", DesignId = design.Id});
            }
            else
            {
                await _db.LogRepository.CreateAsync(new Log {Message = $"Было добавлено {quantity}ед. конструктива {design}", DesignId = design.Id});
            }
        }
        
        public async Task DecreaseQuantityOfDesignAsync(int id, int quantity)
        {
            await IncreaseQuantityOfDesignAsync(id, -quantity);
        }
        
        protected override async Task CreateLogForCreatingAsync(Design item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Был создан конструктив " + item, DesignId = item.Id });
        }

        protected override async Task CreateLogForUpdatingAsync(Design item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Был изменён конструктив " + item, DesignId = item.Id });
        }

        protected override async Task CreateLogForDeletingAsync(Design item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Был удалён конструктив " + item, DesignId = item.Id });
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
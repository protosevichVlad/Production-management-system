using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.BLL.Services
{
    public class DesignService : IDesignService
    {
        private IUnitOfWork _database { get; }
        private ILogService _log;
        private IMapper _mapperToDTO;
        private IMapper _mapperFromDTO;
        
        public DesignService(IUnitOfWork uow)
        {
            _database = uow;
            _log = new LogService(uow);
            _mapperToDTO = new MapperConfiguration(cfg => cfg.CreateMap<Design, DesignDTO>())
                .CreateMapper();
            _mapperFromDTO = new MapperConfiguration(cfg => cfg.CreateMap<DesignDTO, Design>())
                .CreateMapper();
        }
        
        public async Task CreateDesignAsync(DesignDTO designDto)
        {
            var design = _mapperFromDTO.Map<DesignDTO, Design>(designDto);
            await _database.Designs.CreateAsync(design);
            await _database.SaveAsync();
            
            await _log.CreateLogAsync(new LogDTO($"Был создан конструктив {design}"){DesignId = design.Id});
        }

        public async Task UpdateDesignAsync(DesignDTO designDto)
        {
            var design = await _database.Designs.GetAsync(designDto.Id);

            design.Name = designDto.Name;
            design.Quantity = designDto.Quantity;
            design.Type = designDto.Type;
            design.Description = designDto.Description;
            design.ShortDescription = designDto.ShortDescription;
            
            _database.Designs.Update(design);
            await _database.SaveAsync();
            
            await _log.CreateLogAsync(new LogDTO($"Был изменён конструктив {design}"){DesignId = design.Id});
        }

        public async Task<IEnumerable<DesignDTO>> GetDesignsAsync()
        {
            return _mapperToDTO.Map<IEnumerable<Design>, IEnumerable<DesignDTO>>(await _database.Designs.GetAllAsync());
        }

        public async Task<DesignDTO> GetDesignAsync(int? id)
        {
            if (id == null)
            {
                throw new NotImplementedException();
            }

            return _mapperToDTO.Map<Design, DesignDTO>(await _database.Designs.GetAsync((int) id)) ?? throw new NotImplementedException();
        }

        public async Task DeleteDesignAsync(int? id)
        {
            if (id == null)
            {
                throw new NotImplementedException();
            }

            var design = await _database.Designs.GetAsync((int) id) ?? throw new NotImplementedException();
            var checkInDevices = (await CheckInDevicesAsync(design));
            string errorMessage = checkInDevices.Item2;
            if (!checkInDevices.Item1)
            {
                throw new IntersectionOfEntitiesException("Ошибка. Невозможно удаление конструктива.", errorMessage);
            }
            
            await _database.Designs.DeleteAsync((int) id);
            await _database.SaveAsync();
            
            var designLogs = (await _database.Logs.GetAllAsync()).Where(l => l.DeviceId == design.Id);
            foreach (var log in designLogs)
            {
                log.DesignId = null;
                _database.Logs.Update(log);
            }
            
            await _database.SaveAsync();
            
            await _log.CreateLogAsync(new LogDTO($"Был удалён конструктив: {design}"));
        }

        public async Task<IEnumerable<string>> GetTypesAsync()
        {
            var designs = await _database.Designs.GetAllAsync();
            IEnumerable<string> types = designs.OrderBy(d => d.Type).Select(d => d.Type).Distinct();
            return types;
        }

        public async Task AddDesignAsync(int? id, int quantity)
        {
            if (id == null)
            {
                throw new NotImplementedException();
            }

            if (quantity == 0)
            {
                return;
            }

            var design = await _database.Designs.GetAsync((int) id);
            design.Quantity += quantity;
            await _database.SaveAsync();
            
            if (quantity < 0)
            {
                await _log.CreateLogAsync(new LogDTO($"Было получено {-quantity}ед. конструктива {design}"){DesignId = design.Id});
            }
            else
            {
                await _log.CreateLogAsync(new LogDTO($"Было добавлено {quantity}ед. конструктива {design}"){DesignId = design.Id});
            }
        }
        
        public async Task ReceiveDesignAsync(int? id, int quantity)
        {
            await this.AddDesignAsync(id, -quantity);
        }

        public void Dispose()
        {
            _database.Dispose();
        }
        
        private async Task<Tuple<bool, string>> CheckInDevicesAsync(Design design)
        {
            string errorMessage = "";
            var designInDevice = (await _database.DeviceDesignTemplate.GetAllAsync())
                .FirstOrDefault(d => design.Id == d.DesignId);
            if (designInDevice != null)
            {
                var device = (await _database.Devices.GetAllAsync()).FirstOrDefault(d => d.Id == designInDevice.DeviceId);
                errorMessage = $"<i class='bg-light'>{design.ToString()}</i> используется в <i class='bg-light'>{device.ToString()}</i>.<br />" +
                               $"Для удаления <i class='bg-light'>{design.ToString()}</i>, удалите <i class='bg-light'>{device.ToString()}</i>.<br />";
                return new Tuple<bool, string>(false, errorMessage);
            }

            errorMessage = "";
            return new Tuple<bool, string>(true, errorMessage);
        }
        
                
        private async Task<bool> DesignExistsAsync(int id)
        {
            return (await _database.Designs.GetAllAsync()).Any(e => e.Id == id);
        }
    }
}
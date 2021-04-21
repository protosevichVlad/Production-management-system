using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

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
        
        public void CreateDesign(DesignDTO designDto)
        {
            var design = _mapperFromDTO.Map<DesignDTO, Design>(designDto);
            _database.Designs.Create(design);
            _database.Save();
            
            _log.CreateLog(new LogDTO($"Был создан конструктив {design}"){ComponentId = design.Id});
        }

        public void UpdateDesign(DesignDTO designDto)
        {
            var design = _database.Designs.Get(designDto.Id);

            design.Name = designDto.Name;
            design.Quantity = designDto.Quantity;
            design.Type = designDto.Type;
            design.Description = designDto.Description;
            design.ShortDescription = designDto.ShortDescription;
            
            _database.Designs.Update(design);
            _database.Save();
            
            _log.CreateLog(new LogDTO($"Был изменён конструктив {design}"){ComponentId = design.Id});
        }

        public IEnumerable<DesignDTO> GetDesigns()
        {
            return _mapperToDTO.Map<IEnumerable<Design>, IEnumerable<DesignDTO>>(_database.Designs.GetAll());
        }

        public DesignDTO GetDesign(int? id)
        {
            if (id == null)
            {
                throw new NotImplementedException();
            }

            return _mapperToDTO.Map<Design, DesignDTO>(_database.Designs.Get((int) id));
        }

        public void DeleteDesign(int? id)
        {
            if (id == null)
            {
                throw new NotImplementedException();
            }

            var design = _database.Designs.Get((int) id);
            if (!CheckInDevices(design, out string errorMessage))
            {
                throw new IntersectionOfEntitiesException("Ошибка. Невозможно удаление конструктива.", errorMessage);
            }
            
            _database.Designs.Delete((int) id);
            _database.Save();
            
            var designLogs = _database.Logs.GetAll().Where(l => l.DeviceId == design.Id);
            foreach (var log in designLogs)
            {
                log.DesignId = null;
                _database.Logs.Update(log);
            }
            
            _database.Save();
            
            _log.CreateLog(new LogDTO($"Был удалён конструктив: {design}"));
        }

        public IEnumerable<string> GetTypes()
        {
            var designs = _database.Designs.GetAll();
            IEnumerable<string> types = designs.OrderBy(d => d.Type).Select(d => d.Type).Distinct();
            return types;
        }

        public void AddDesign(int? id, int quantity)
        {
            if (id == null)
            {
                throw new NotImplementedException();
            }

            var design = _database.Designs.Get((int) id);
            design.Quantity += quantity;
            _database.Save();
            
            if (quantity < 0)
            {
                _log.CreateLog(new LogDTO($"Было получено {-quantity}ед. конструктива {design}"){ComponentId = design.Id});
            }
            else
            {
                _log.CreateLog(new LogDTO($"Было добавлено {quantity}ед. конструктива {design}"){ComponentId = design.Id});
            }
        }

        public void Dispose()
        {
            _database.Dispose();
        }
        
        private bool CheckInDevices(Design design , out string errorMessage)
        {
            var designInDevice = _database.DeviceDesignTemplate.GetAll()
                .FirstOrDefault(d => design.Id == d.DesignId);
            if (designInDevice != null)
            {
                var device = _database.Devices.GetAll().FirstOrDefault(d => d.Id == designInDevice.DeviceId);
                errorMessage = $"<i class='bg-light'>{design.ToString()}</i> используется в <i class='bg-light'>{device.ToString()}</i>.<br />" +
                               $"Для удаления <i class='bg-light'>{design.ToString()}</i>, удалите <i class='bg-light'>{device.ToString()}</i>.<br />";
                return false;
            }

            errorMessage = "";
            return true;
        }
        
                
        private bool DesignExists(int id)
        {
            return _database.Designs.GetAll().Any(e => e.Id == id);
        }
    }
}
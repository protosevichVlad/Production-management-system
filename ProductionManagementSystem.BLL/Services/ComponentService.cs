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
    public class ComponentService : IComponentService
    {
        private IUnitOfWork _database { get; set; }
        private ILogService _log;
        private IMapper _mapperToDTO;
        private IMapper _mapperFromDTO;
        
        public ComponentService(IUnitOfWork uow)
        {
            _database = uow;
            _log = new LogService(uow);
            _mapperToDTO = new MapperConfiguration(cfg => cfg.CreateMap<Component, ComponentDTO>())
                .CreateMapper();
            _mapperFromDTO = new MapperConfiguration(cfg => cfg.CreateMap<ComponentDTO, Component>())
                .CreateMapper();
        }
        
        public void CreateComponent(ComponentDTO componentDto)
        {
            var component = _mapperFromDTO.Map<ComponentDTO, Component>(componentDto);
            _database.Components.Create(component);
            _database.Save();
            
            _log.CreateLog(new LogDTO($"Был создан новый монтаж: {component}"){ComponentId = component.Id});
        }

        public void UpdateComponent(ComponentDTO componentDto)
        {
            var component = _database.Components.Get(componentDto.Id);

            component.Corpus = componentDto.Corpus;
            component.Explanation = componentDto.Explanation;
            component.Manufacturer = componentDto.Manufacturer;
            component.Name = componentDto.Name;
            component.Nominal = componentDto.Nominal;
            component.Quantity = componentDto.Quantity;
            component.Type = componentDto.Type;

            _database.Components.Update(component);
            _database.Save();
            
            _log.CreateLog(new LogDTO($"Был изменён монтаж: {component}"){ComponentId = component.Id});
        }

        public IEnumerable<ComponentDTO> GetComponents()
        {
            return _mapperToDTO.Map<IEnumerable<Component>, IEnumerable<ComponentDTO>>(_database.Components.GetAll());
        }

        public ComponentDTO GetComponent(int? id)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }
            
            return _mapperToDTO.Map<Component, ComponentDTO>(_database.Components.Get((int) id));
        }

        public void DeleteComponent(int? id)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }

            var component = _database.Components.Get((int) id);
            if (!CheckInDevices(component, out string errorMessage))
            {
                throw new IntersectionOfEntitiesException("Ошибка. Невозможно удаление монтажа.", errorMessage);
            }
            
            _database.Components.Delete((int) id);
            _database.Save();

            var componetLogs = _database.Logs.GetAll().Where(l => l.ComponentId == component.Id);
            foreach (var log in componetLogs)
            {
                log.ComponentId = null;
                _database.Logs.Update(log);
            }
            
            _database.Save();
            
            _log.CreateLog(new LogDTO($"Был удалён монтаж: {component}"));
        }

        public IEnumerable<string> GetTypes()
        {
            var components = _database.Components.GetAll();
            IEnumerable<string> types = components.OrderBy(c => c.Type).Select(c => c.Type).Distinct();
            return types;
        }

        public void AddComponent(int? id, int quantity)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }

            var component = _database.Components.Get((int) id);
            component.Quantity += quantity;
            _database.Save();

            if (quantity < 0)
            {
                _log.CreateLog(new LogDTO($"Былы получено {-quantity}ед. монтажа {component}"){ComponentId = component.Id});
            }
            else
            {
                _log.CreateLog(new LogDTO($"Былы добавлено {quantity}ед. монтажа {component}"){ComponentId = component.Id});
            }
        }

        public void Dispose()
        {
            _database.Dispose();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <param name="errorMessage"></param>
        /// <returns>Return true, if component not using in devices.</returns>
        private bool CheckInDevices(Component component , out string errorMessage)
        {
            var componentInDevice = _database.DeviceComponentsTemplate.GetAll()
                .FirstOrDefault(c => component.Id == c.ComponentId);
            if (componentInDevice != null)
            {
                var device = _database.Devices.GetAll().FirstOrDefault(d => d.Id == componentInDevice.DeviceId);
                errorMessage = $"<i class='bg-light'>{component.ToString()}</i> используется в <i class='bg-light'>{device.ToString()}</i>.<br />" +
                               $"Для удаления <i class='bg-light'>{component.ToString()}</i>, удалите <i class='bg-light'>{device.ToString()}</i>.<br />";
                return false;
            }
            
            errorMessage = "";
            return true;
        }
        
        private bool ComponentExists(int id)
        {
            return _database.Components.GetAll().Any(e => e.Id == id);
        }
    }
}
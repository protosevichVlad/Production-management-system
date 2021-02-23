using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.BLL.Services
{
    public class ComponentService : IComponentService
    {
        private IUnitOfWork _database { get; set; }
        private IMapper _mapperToDTO;
        private IMapper _mapperFromDTO;
        
        public ComponentService(IUnitOfWork uow)
        {
            _database = uow;
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
        }

        public void UpdateComponent(ComponentDTO componentDto)
        {
            var component = _mapperFromDTO.Map<ComponentDTO, Component>(componentDto);
            _database.Components.Update(component);
            _database.Save();
        }

        public IEnumerable<ComponentDTO> GetComponents()
        {
            return _mapperToDTO.Map<IEnumerable<Component>, IEnumerable<ComponentDTO>>(_database.Components.GetAll());
        }

        public ComponentDTO GetComponent(int? id)
        {
            if (id == null)
            {
                throw new NotImplementedException();
            }
            
            return _mapperToDTO.Map<Component, ComponentDTO>(_database.Components.Get((int) id));
        }

        public void DeleteComponent(int? id)
        {
            if (id == null)
            {
                throw new NotImplementedException();
            }
            
            _database.Components.Delete((int) id);
            _database.Save();
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
                throw new NotImplementedException();
            }

            var component = _database.Components.Get((int) id);
            component.Quantity += quantity;
            _database.Save();
        }

        public void Dispose()
        {
            _database.Dispose();
        }
        
        private bool CheckInDevices(Component component , out string errorMessage)
        {
            var componentsInDevices = _database.DeviceComponentsTemplate.GetAll();
            foreach (var componentInDevice in componentsInDevices)
            {
                if (component.Id == componentInDevice.ComponentId)
                {
                    var device = _database.Devices.Find(d => d.Id == componentInDevice.DeviceId);
                    errorMessage = "Невозможно удаление монтажа.<br />" +
                                   $"<i class='bg-light'>{component}</i> используется в <i class='bg-light'>{device}</i>.<br />" +
                                   $"Для удаления <i class='bg-light'>{component}</i>, удалите <i class='bg-light'>{device}</i>.<br />";
                    return false;
                }
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
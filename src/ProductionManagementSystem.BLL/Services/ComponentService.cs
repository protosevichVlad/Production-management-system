﻿using System;
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
    public class ComponentService : IComponentService
    {
        private readonly IUnitOfWork _database;
        private readonly ILogService _log;
        private readonly IMapper _mapper;
        
        public ComponentService(IUnitOfWork uow)
        {
            _database = uow;
            _log = new LogService(uow);
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Component, ComponentDTO>();
                    cfg.CreateMap<ComponentDTO, Component>();
                })
                .CreateMapper();
        }
        
        public async Task CreateComponentAsync(ComponentDTO componentDto)
        {
            var component = _mapper.Map<ComponentDTO, Component>(componentDto);
            await _database.Components.CreateAsync(component);
            await _database.SaveAsync();
            
            await _log.CreateLogAsync(new LogDTO($"Был создан новый монтаж: {component}"){ComponentId = component.Id});
        }

        public async Task UpdateComponentAsync(ComponentDTO componentDto)
        {
            var component = await _database.Components.GetAsync(componentDto.Id);

            component.Corpus = componentDto.Corpus;
            component.Explanation = componentDto.Explanation;
            component.Manufacturer = componentDto.Manufacturer;
            component.Name = componentDto.Name;
            component.Nominal = componentDto.Nominal;
            component.Quantity = componentDto.Quantity;
            component.Type = componentDto.Type;

            _database.Components.Update(component);
            await _database.SaveAsync();
            
            await _log.CreateLogAsync(new LogDTO($"Был изменён монтаж: {component}"){ComponentId = component.Id});
        }

        public async Task<IEnumerable<ComponentDTO>> GetComponentsAsync()
        {
            return _mapper.Map<IEnumerable<Component>, IEnumerable<ComponentDTO>>(await _database.Components.GetAllAsync());
        }

        public async Task<ComponentDTO> GetComponentAsync(int? id)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }
            
            return _mapper.Map<Component, ComponentDTO>(await _database.Components.GetAsync((int) id)) ?? throw new NotImplementedException();
        }

        public async Task DeleteComponentAsync(int? id)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }

            var component = await _database.Components.GetAsync((int) id) ?? throw new NotImplementedException();
            var checkInDevices = (await CheckInDevicesAsync(component));
            string errorMessage = checkInDevices.Item2;
            if (!checkInDevices.Item1)
            {
                throw new IntersectionOfEntitiesException("Ошибка. Невозможно удаление монтажа.", errorMessage);
            }
            
            await _database.Components.DeleteAsync((int) id);
            await _database.SaveAsync();

            var componentLogs = (await _database.Logs.GetAllAsync()).Where(l => l.ComponentId == component.Id);
            foreach (var log in componentLogs)
            {
                log.ComponentId = null;
                _database.Logs.Update(log);
            }
            
            await _database.SaveAsync();
            
            await _log.CreateLogAsync(new LogDTO($"Был удалён монтаж: {component}"));
        }

        public async Task<IEnumerable<string>> GetTypesAsync()
        {
            var components = await _database.Components.GetAllAsync();
            IEnumerable<string> types = components.OrderBy(c => c.Type).Select(c => c.Type).Distinct();
            return types;
        }

        public async Task AddComponentAsync(int? id, int quantity)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }

            if (quantity == 0)
            {
                return;
            }

            var component = await _database.Components.GetAsync((int) id);
            component.Quantity += quantity;
            await _database.SaveAsync();

            if (quantity < 0)
            {
                await _log.CreateLogAsync(new LogDTO($"Было получено {-quantity}ед. монтажа {component}"){ComponentId = component.Id});
            }
            else
            {
                await _log.CreateLogAsync(new LogDTO($"Было добавлено {quantity}ед. монтажа {component}"){ComponentId = component.Id});
            }
        }

        public async Task ReceiveComponentAsync(int? id, int quantity)
        {
            await this.AddComponentAsync(id, -quantity);
        }

        public void Dispose()
        {
            _database.Dispose();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns>Return true, if component not using in devices.</returns>
        private async Task<Tuple<bool, string>> CheckInDevicesAsync(Component component)
        {
            string errorMessage;
            var componentInDevice = (await _database.DeviceComponentsTemplate.GetAllAsync())
                .FirstOrDefault(c => component.Id == c.ComponentId);
            if (componentInDevice != null)
            {
                var device = (await _database.Devices.GetAllAsync()).FirstOrDefault(d => d.Id == componentInDevice.DeviceId);
                errorMessage = $"<i class='bg-light'>{component}</i> используется в <i class='bg-light'>{device}</i>.<br />" +
                               $"Для удаления <i class='bg-light'>{component}</i>, удалите <i class='bg-light'>{device}</i>.<br />";
                return new Tuple<bool, string>(false, errorMessage);
            }

            errorMessage = String.Empty;
            return new Tuple<bool, string>(true, errorMessage);
        }
        
        private async Task<bool> ComponentExistsAsync(int id)
        {
            return (await _database.Components.GetAllAsync()).Any(e => e.Id == id);
        }
    }
}
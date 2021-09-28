using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Enums;
using ProductionManagementSystem.DAL.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.BLL.Services
{
    public class ComponentsSupplyRequestService : IComponentsSupplyRequestService
    {
        private readonly IUnitOfWork _database;
        private ILogService _log;
        private readonly IMapper _mapper;
        
        public ComponentsSupplyRequestService(IUnitOfWork uow)
        {
            _database = uow;
            _log = new LogService(uow);
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ComponentsSupplyRequestDTO, ComponentsSupplyRequest>();
                    cfg.CreateMap<ComponentsSupplyRequest, ComponentsSupplyRequestDTO>();
                    cfg.CreateMap<ComponentDTO, Component>();
                    cfg.CreateMap<Component, ComponentDTO>();
                    cfg.CreateMap<DAL.Entities.Task, TaskDTO>();
                    cfg.CreateMap<TaskDTO, DAL.Entities.Task>();
                    cfg.CreateMap<StatusSupplyEnum, StatusSupplyEnumDTO>();
                    cfg.CreateMap<StatusSupplyEnumDTO, StatusSupplyEnum>();
                    cfg.CreateMap<ProductionManagementSystemUser, UserDTO>();
                    cfg.CreateMap<UserDTO, ProductionManagementSystemUser>();
                    cfg.CreateMap<Device, DeviceDTO>();
                    cfg.CreateMap<DeviceDTO, Device>();
                    cfg.CreateMap<Log, LogDTO>();
                    cfg.CreateMap<LogDTO, Log>();
                    cfg.CreateMap<ObtainedDesign, ObtainedDesignDTO>();
                    cfg.CreateMap<ObtainedDesignDTO, ObtainedDesign>();
                    cfg.CreateMap<ObtainedComponent, ObtainedComponentDTO>();
                    cfg.CreateMap<ObtainedComponentDTO, ObtainedComponent>();
                    cfg.CreateMap<DesignDTO, Design>();
                    cfg.CreateMap<Design, DesignDTO>();
                })
                .CreateMapper();
        }
        
        public void Dispose()
        {
            _database.Dispose();
        }

        public async Task CreateComponentSupplyRequestAsync(ComponentsSupplyRequestDTO componentsSupplyRequest)
        {
            if (componentsSupplyRequest == null)
            {
                throw new ArgumentNullException(nameof(componentsSupplyRequest));
            }

            await _database.ComponentSupplyRequests.CreateAsync(
                _mapper.Map<ComponentsSupplyRequestDTO, ComponentsSupplyRequest>(componentsSupplyRequest)
                );
            await _database.SaveAsync();
        }

        public async Task UpdateComponentSupplyRequestAsync(ComponentsSupplyRequestDTO componentsSupplyRequest)
        {
            if (componentsSupplyRequest == null)
            {
                throw new ArgumentNullException(nameof(componentsSupplyRequest));
            }

            _database.ComponentSupplyRequests.Update(
                _mapper.Map<ComponentsSupplyRequestDTO, ComponentsSupplyRequest>(componentsSupplyRequest)
            );
            await _database.SaveAsync();
        }

        public async Task<IEnumerable<ComponentsSupplyRequestDTO>> GetComponentSupplyRequestsAsync()
        {
            return _mapper.Map<IEnumerable<ComponentsSupplyRequest>, IEnumerable<ComponentsSupplyRequestDTO>>(
                    await _database.ComponentSupplyRequests.GetAllAsync()
                );
        }

        public async Task<ComponentsSupplyRequestDTO> GetComponentSupplyRequestAsync(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            
            return _mapper.Map<ComponentsSupplyRequest, ComponentsSupplyRequestDTO>(
                await _database.ComponentSupplyRequests.GetAsync((int)id)
            );    
        }

        public async Task DeleteComponentSupplyRequestAsync(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            await _database.ComponentSupplyRequests.DeleteAsync((int) id);
            await _database.SaveAsync();
        }

        public async Task ChangeStatusAsync(int id, int to, string message = "")
        {
            var componentSupplyRequest = await GetComponentSupplyRequestAsync(id);
            componentSupplyRequest.StatusSupply = (StatusSupplyEnumDTO) to;
            await UpdateComponentSupplyRequestAsync(componentSupplyRequest);

            await _log.CreateLogAsync(new LogDTO(message));
        }
    }
}
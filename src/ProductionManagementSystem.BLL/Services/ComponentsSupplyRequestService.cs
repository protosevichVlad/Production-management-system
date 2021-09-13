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
        private IUnitOfWork _database { get; set; }
        private ILogService _log;
        private IMapper _mapper;
        
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
                    cfg.CreateMap<Task, TaskDTO>();
                    cfg.CreateMap<TaskDTO, Task>();
                    cfg.CreateMap<StatusSupplyEnum, StatusSupplyEnumDTO>();
                    cfg.CreateMap<StatusSupplyEnumDTO, StatusSupplyEnum>();
                    cfg.CreateMap<ProductionManagementSystemUser, UserDTO>();
                    cfg.CreateMap<UserDTO, ProductionManagementSystemUser>();
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
        }
    }
}
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
    public class DesignsSupplyRequestService : IDesignsSupplyRequestService
    {
        private readonly IUnitOfWork _database;
        private readonly IDesignService _designService;
        private ILogService _log;
        private readonly IMapper _mapper;
    
        public DesignsSupplyRequestService(IUnitOfWork uow)
        {
            _database = uow;
            _designService = new DesignService(uow);
            _log = new LogService(uow);
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DesignsSupplyRequestDTO, DesignsSupplyRequest>();
                    cfg.CreateMap<DesignsSupplyRequest, DesignsSupplyRequestDTO>();
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

        public async Task CreateDesignSupplyRequestAsync(DesignsSupplyRequestDTO designsSupplyRequest)
        {
            if (designsSupplyRequest == null)
            {
                throw new ArgumentNullException(nameof(designsSupplyRequest));
            }

            await _database.DesignsSupplyRequests.CreateAsync(
                _mapper.Map<DesignsSupplyRequestDTO, DesignsSupplyRequest>(designsSupplyRequest));
            await _database.SaveAsync();
        }

        public async Task UpdateDesignSupplyRequestAsync(DesignsSupplyRequestDTO designsSupplyRequest)
        {
            if (designsSupplyRequest == null)
            {
                throw new ArgumentNullException(nameof(designsSupplyRequest));
            }

            _database.DesignsSupplyRequests.Update(
                _mapper.Map<DesignsSupplyRequestDTO, DesignsSupplyRequest>(designsSupplyRequest));
            await _database.SaveAsync();
        }

        public async Task<IEnumerable<DesignsSupplyRequestDTO>> GetDesignSupplyRequestsAsync()
        {
            return _mapper.Map<IEnumerable<DesignsSupplyRequest>, IEnumerable<DesignsSupplyRequestDTO>>(
                await _database.DesignsSupplyRequests.GetAllAsync());
        }

        public async Task<DesignsSupplyRequestDTO> GetDesignSupplyRequestAsync(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return _mapper.Map<DesignsSupplyRequest, DesignsSupplyRequestDTO>(await _database.DesignsSupplyRequests.GetAsync(id.Value));
        }

        public async Task DeleteDesignSupplyRequestAsync(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            await _database.DesignsSupplyRequests.DeleteAsync(id.Value);
            await _database.SaveAsync();
        }

        public async Task ChangeStatusAsync(int supplyRequestId, int to, string message)
        {
            var designSupplyRequest = await GetDesignSupplyRequestAsync(supplyRequestId);
            designSupplyRequest.StatusSupply = (StatusSupplyEnumDTO) to;
            if ((StatusSupplyEnumDTO) to == StatusSupplyEnumDTO.Ready)
            {
                await _designService.AddDesignAsync(designSupplyRequest.DesignId,
                    designSupplyRequest.Quantity);
            }
            await UpdateDesignSupplyRequestAsync(designSupplyRequest);

            await _log.CreateLogAsync(new LogDTO(message));
        }
    }
}
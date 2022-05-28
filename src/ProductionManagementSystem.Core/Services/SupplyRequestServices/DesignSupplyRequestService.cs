using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models.Logs;
using ProductionManagementSystem.Core.Models.SupplyRequests;
using ProductionManagementSystem.Core.Repositories;
using ProductionManagementSystem.Core.Services.SupplyRequestServices;

namespace ProductionManagementSystem.Core.Services
{
    public interface IDesignSupplyRequestService : IBaseService<DesignSupplyRequest>, ISupplyRequestService<DesignSupplyRequest>
    {
        Task ChangeStatusAsync(int id, int to, string message);
        Task DeleteByIdAsync(int id);
    }

    public class DesignSupplyRequestService : BaseServiceWithLogs<DesignSupplyRequest>, IDesignSupplyRequestService
    {
        private readonly IDesignService _designService;
        
        public DesignSupplyRequestService(IUnitOfWork uow) : base(uow)
        {
            _designService = new DesignService(uow);
            _currentRepository = _db.DesignsSupplyRequestRepository;
        }
        
        public async Task ChangeStatusAsync(int id, int to, string message = "")
        {
            var designSupplyRequest = await GetByIdAsync(id);
            designSupplyRequest.StatusSupply = (SupplyStatusEnum) to;
            if ((SupplyStatusEnum) to == SupplyStatusEnum.Ready)
            {
                await _designService.IncreaseQuantityAsync(designSupplyRequest.ComponentId,
                    designSupplyRequest.Quantity);
            }
            
            await _currentRepository.UpdateAsync(designSupplyRequest);
            await _db.SaveAsync();
            await _db.LogRepository.CreateAsync(new Log
            {
                Message = message, 
                ItemId = designSupplyRequest.Id, 
                ItemType = LogsItemType.DesignSupplyRequest, 
            });
            await _db.SaveAsync();
        }
        
        public async Task<List<DesignSupplyRequest>> GetSupplyRequestsByTaskIdAsync(int taskId)
        {
            return await _currentRepository.FindAsync(sr => sr.TaskId == taskId);
        }

        public async Task DeleteByIdAsync(int id)
        {
            await DeleteAsync(new DesignSupplyRequest {Id = id});
        }

        protected override LogsItemType ItemType => LogsItemType.DesignSupplyRequest;
        protected override object GetPropValue(DesignSupplyRequest src, string propName)
        {
            if (propName == nameof(src.StatusSupply))
                return GetStatusName(src.StatusSupply);
                    
            return base.GetPropValue(src, propName);
        }
        
        public string GetStatusName(SupplyStatusEnum item)
        {
            List<string> result = new List<string>();
            foreach (var value in Enum.GetValues<SupplyStatusEnum>())
            {
                if ((item & value) == value)
                {
                    result.Add(value.GetType()
                        .GetMember(value.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()
                        ?.GetName());
                }
            }
            return String.Join(", ", result);
        }
    }
}
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
    public interface IMontageSupplyRequestService : IBaseService<MontageSupplyRequest>, ISupplyRequestService<MontageSupplyRequest>
    {
        Task ChangeStatusAsync(int id, int to, string message);
        Task DeleteByIdAsync(int id);
    }

    public class MontageSupplyRequestService : BaseServiceWithLogs<MontageSupplyRequest>, IMontageSupplyRequestService
    {
        private readonly IMontageService _montageService;
        
        public MontageSupplyRequestService(IUnitOfWork uow) : base(uow)
        {
            _montageService = new MontageService(uow);
            _currentRepository = _db.MontageSupplyRequestRepository;
        }

        public async Task ChangeStatusAsync(int id, int to, string message = "")
        {
            var montageSupplyRequest = await GetByIdAsync(id);
            montageSupplyRequest.StatusSupply = (SupplyStatusEnum) to;
            if ((SupplyStatusEnum) to == SupplyStatusEnum.Ready)
            {
                await _montageService.IncreaseQuantityAsync(montageSupplyRequest.ComponentId,
                    montageSupplyRequest.Quantity);
            }
            
            await _currentRepository.UpdateAsync(montageSupplyRequest);
            await _db.SaveAsync();
            await _db.LogRepository.CreateAsync(new Log
            {
                Message = message, 
                ItemId = montageSupplyRequest.Id,
                ItemType = LogsItemType.MontageSupplyRequest,
            });
            await _db.SaveAsync();
        }
        
        public async Task<List<MontageSupplyRequest>> GetSupplyRequestsByTaskIdAsync(int taskId)
        {
            return await _currentRepository.FindAsync(sr => sr.TaskId == taskId);
        }

        protected override LogsItemType ItemType => LogsItemType.MontageSupplyRequest;
        
        protected override object GetPropValue(MontageSupplyRequest src, string propName)
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
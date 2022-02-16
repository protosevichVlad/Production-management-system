using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProductionManagementSystem.Core.Models.SupplyRequests;

namespace ProductionManagementSystem.Core.Services.SupplyRequestServices
{
    public interface ISupplyRequestService<TEntity> where TEntity : SupplyRequest
    {
        Task<List<TEntity>> GetSupplyRequestsByTaskIdAsync(int taskId);
    }
    
    public class SupplyRequestService : ISupplyRequestService<SupplyRequest>
    {
        private readonly IDesignSupplyRequestService _designSupplyRequestService;
        private readonly IMontageSupplyRequestService _montageSupplyRequestService;

        public SupplyRequestService(IMontageSupplyRequestService montageSupplyRequestService, IDesignSupplyRequestService designSupplyRequestService)
        {
            _montageSupplyRequestService = montageSupplyRequestService;
            _designSupplyRequestService = designSupplyRequestService;
        }

        public async Task<List<SupplyRequest>> GetSupplyRequestsByTaskIdAsync(int taskId)
        {
            List<SupplyRequest> result = (await _montageSupplyRequestService.GetSupplyRequestsByTaskIdAsync(taskId)).Cast<SupplyRequest>().ToList();
            result.AddRange((await _designSupplyRequestService.GetSupplyRequestsByTaskIdAsync(taskId)).Cast<SupplyRequest>().ToList());
            return result;
        }
    }
}
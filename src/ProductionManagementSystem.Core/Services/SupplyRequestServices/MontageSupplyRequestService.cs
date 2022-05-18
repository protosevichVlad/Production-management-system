using System.Collections.Generic;
using System.Linq;
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
                MontageSupplyRequestId = montageSupplyRequest.Id, 
                DesignId = montageSupplyRequest.ComponentId, 
                TaskId = montageSupplyRequest.TaskId
            });
            await _db.SaveAsync();
        }
        
        public async Task<List<MontageSupplyRequest>> GetSupplyRequestsByTaskIdAsync(int taskId)
        {
            return await _currentRepository.FindAsync(sr => sr.TaskId == taskId);
        }

        public async Task DeleteByIdAsync(int id)
        {
            await DeleteAsync(new MontageSupplyRequest {Id = id});
        }
        
        protected override async Task CreateLogForCreatingAsync(MontageSupplyRequest item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Была создана заявка на снабжения монтажа " + item, MontageSupplyRequestId = item.Id });
        }

        protected override async Task CreateLogForUpdatingAsync(MontageSupplyRequest item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Была изменена заявка на снабжения монтажа " + item, MontageSupplyRequestId = item.Id });
        }

        protected override async Task CreateLogForDeletingAsync(MontageSupplyRequest item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Была удалена заявка на снабжения монтажа " + item, MontageSupplyRequestId = item.Id });
        }
        
        
        protected override bool UpdateLogPredicate(Log log, MontageSupplyRequest item) => log.MontageSupplyRequestId == item.Id; 

        protected override void UpdateLog(Log log) => log.MontageSupplyRequestId = null;
    }
}
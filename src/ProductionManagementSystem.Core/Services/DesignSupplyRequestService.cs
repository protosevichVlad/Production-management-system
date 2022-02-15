using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models.Logs;
using ProductionManagementSystem.Core.Models.SupplyRequests;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services
{
    public interface IDesignSupplyRequestService : IBaseService<DesignSupplyRequest>
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
                DesignSupplyRequestId = designSupplyRequest.Id, 
                DesignId = designSupplyRequest.ComponentId, 
                TaskId = designSupplyRequest.TaskId
            });
            await _db.SaveAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            await DeleteAsync(new DesignSupplyRequest {Id = id});
        }
        
        protected override async Task CreateLogForCreatingAsync(DesignSupplyRequest item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Была создана заявка на снабжения конструктива " + item, DesignSupplyRequestId = item.Id });
        }

        protected override async Task CreateLogForUpdatingAsync(DesignSupplyRequest item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Была изменёна заявка на снабжения конструктива " + item, DesignSupplyRequestId = item.Id });
        }

        protected override async Task CreateLogForDeletingAsync(DesignSupplyRequest item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Была удалёна заявка на снабжения конструктива " + item, DesignSupplyRequestId = item.Id });
        }
        
        protected override bool UpdateLogPredicate(Log log, DesignSupplyRequest item) => log.DesignSupplyRequestId == item.Id; 

        protected override void UpdateLog(Log log) => log.DesignSupplyRequestId = null;
    }
}
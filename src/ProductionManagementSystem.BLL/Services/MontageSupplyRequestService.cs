using System.Threading.Tasks;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Logs;
using ProductionManagementSystem.Models.SupplyRequests;

namespace ProductionManagementSystem.BLL.Services
{
    public interface IMontageSupplyRequestService : IBaseService<MontageSupplyRequest>
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
                await _montageService.IncreaseQuantityOfMontageAsync(montageSupplyRequest.ComponentId,
                    montageSupplyRequest.Quantity);
            }
            
            await _currentRepository.UpdateAsync(montageSupplyRequest);
            await _db.LogRepository.CreateAsync(new Log
            {Message = message, 
                MontageSupplyRequestId = montageSupplyRequest.Id, 
                DesignId = montageSupplyRequest.ComponentId, 
                TaskId = montageSupplyRequest.TaskId});
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
            await _db.LogRepository.CreateAsync(new Log { Message = "Была изменёна заявка на снабжения монтажа " + item, MontageSupplyRequestId = item.Id });
        }

        protected override async Task CreateLogForDeletingAsync(MontageSupplyRequest item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Была удалёна заявка на снабжения монтажа " + item, MontageSupplyRequestId = item.Id });
        }
    }
}
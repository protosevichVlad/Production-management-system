using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Logs;
using ProductionManagementSystem.Models.SupplyRequests;
using Task = System.Threading.Tasks.Task;

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
        private ILogService _log;
        
        public MontageSupplyRequestService(IUnitOfWork uow) : base(uow)
        {
            _montageService = new MontageService(uow);
            _log = new LogService(uow);
            _currentRepository = _db.MontageSupplyRequestRepository;
        }
        
        public void Dispose()
        {
            _db.Dispose();
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
            
            await UpdateAsync(montageSupplyRequest);
            await _log.CreateAsync(new Log() {Message = message});
        }

        public async Task DeleteByIdAsync(int id)
        {
            await this.DeleteAsync(new MontageSupplyRequest() {Id = id});
        }
        
        protected override async Task CreateLogForCreatingAsync(MontageSupplyRequest item)
        {
            await _db.LogRepository.CreateAsync(new Log()
                { Message = "Была создана заявка на снабжения монтажа " + item.ToString(), MontageSupplyRequestId = item.Id });
        }

        protected override async Task CreateLogForUpdatingAsync(MontageSupplyRequest item)
        {
            await _db.LogRepository.CreateAsync(new Log()
                { Message = "Была изменёна заявка на снабжения монтажа " + item.ToString(), MontageSupplyRequestId = item.Id });
        }

        protected override async Task CreateLogForDeletingAsync(MontageSupplyRequest item)
        {
            await _db.LogRepository.CreateAsync(new Log()
                { Message = "Была удалёна заявка на снабжения монтажа " + item.ToString(), MontageSupplyRequestId = item.Id });
        }
    }
}
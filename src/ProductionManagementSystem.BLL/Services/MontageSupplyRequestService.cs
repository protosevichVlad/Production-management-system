using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Logs;
using ProductionManagementSystem.Models.SupplyRequests;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.BLL.Services
{
    public interface IMontageSupplyRequestService : IBaseService<MontageSupplyRequest>
    {
        Task ChangeStatusAsync(int id, int to, string message);
    }

    public class MontageSupplyRequestService : BaseService<MontageSupplyRequest>, IMontageSupplyRequestService
    {
        private readonly IMontageService _montageService;
        private ILogService _log;
        
        public MontageSupplyRequestService(IUnitOfWork uow) : base(uow)
        {
            _montageService = new MontageService(uow);
            _log = new LogService(uow);
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
    }
}
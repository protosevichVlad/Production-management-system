using System.Threading.Tasks;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Logs;
using ProductionManagementSystem.Models.SupplyRequests;

namespace ProductionManagementSystem.BLL.Services
{
    public interface IDesignSupplyRequestService : IBaseService<DesignSupplyRequest>
    {
        Task ChangeStatusAsync(int id, int to, string message);
        Task DeleteByIdAsync(int id);
    }

    public class DesignSupplyRequestService : BaseServiceWithLogs<DesignSupplyRequest>, IDesignSupplyRequestService
    {
        private readonly IDesignService _designService;
        private ILogService _log;
        
        public DesignSupplyRequestService(IUnitOfWork uow) : base(uow)
        {
            _designService = new DesignService(uow);
            _log = new LogService(uow);
            _currentRepository = _db.DesignsSupplyRequestRepository;
        }
        
        public void Dispose()
        {
            _db.Dispose();
        }

        public async Task ChangeStatusAsync(int id, int to, string message = "")
        {
            var designSupplyRequest = await GetByIdAsync(id);
            designSupplyRequest.StatusSupply = (SupplyStatusEnum) to;
            if ((SupplyStatusEnum) to == SupplyStatusEnum.Ready)
            {
                await _designService.IncreaseQuantityOfDesignAsync(designSupplyRequest.ComponentId,
                    designSupplyRequest.Quantity);
            }
            
            await UpdateAsync(designSupplyRequest);
            await _log.CreateAsync(new Log() {Message = message});
        }

        public async Task DeleteByIdAsync(int id)
        {
            await this.DeleteAsync(new DesignSupplyRequest() {Id = id});
        }
        
        protected override async Task CreateLogForCreatingAsync(DesignSupplyRequest item)
        {
            await _db.LogRepository.CreateAsync(new Log()
                { Message = "Была создана заявка на снабжения конструктива " + item.ToString(), DesignSupplyRequestId = item.Id });
        }

        protected override async Task CreateLogForUpdatingAsync(DesignSupplyRequest item)
        {
            await _db.LogRepository.CreateAsync(new Log()
                { Message = "Была изменёна заявка на снабжения конструктива " + item.ToString(), DesignSupplyRequestId = item.Id });
        }

        protected override async Task CreateLogForDeletingAsync(DesignSupplyRequest item)
        {
            await _db.LogRepository.CreateAsync(new Log()
                { Message = "Была удалёна заявка на снабжения конструктива " + item.ToString(), DesignSupplyRequestId = item.Id });
        }
    }
}
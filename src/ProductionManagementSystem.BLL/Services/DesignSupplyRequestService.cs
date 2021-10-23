using System.Threading.Tasks;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Logs;
using ProductionManagementSystem.Models.SupplyRequests;

namespace ProductionManagementSystem.BLL.Services
{
    public interface IDesignSupplyRequestService : IBaseService<DesignSupplyRequest>
    {
        Task ChangeStatusAsync(int id, int to, string message);
    }

    public class DesignSupplyRequestService : BaseService<DesignSupplyRequest>, IDesignSupplyRequestService
    {
        private readonly IDesignService _designService;
        private ILogService _log;
        
        public DesignSupplyRequestService(IUnitOfWork uow) : base(uow)
        {
            _designService = new DesignService(uow);
            _log = new LogService(uow);
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
    }
}
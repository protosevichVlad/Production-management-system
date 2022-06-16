using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.Logs;
using ProductionManagementSystem.Core.Models.SupplyRequests;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services
{
    public interface ICDBSupplyRequestService : IBaseService<CDBSupplyRequest>
    {
        
    }
    
    public class CDBSupplyRequestService : BaseServiceWithLogs<CDBSupplyRequest>, ICDBSupplyRequestService
    {
        public CDBSupplyRequestService(IUnitOfWork db) : base(db)
        {
            _currentRepository = db.CdbSupplyRequestRepository;
        }

        protected override LogsItemType ItemType => LogsItemType.SupplyRequest;
        protected override int GetEntityId(CDBSupplyRequest model) => model.Id;

        protected override object GetPropValue(CDBSupplyRequest src, string propName)
        {
            if (nameof(src.Entity) == propName)
                return src.Entity != null ? src.Entity.ToString() : "";
            
            return base.GetPropValue(src, propName);
        }
    }
}
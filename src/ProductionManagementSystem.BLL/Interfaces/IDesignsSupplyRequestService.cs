using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.BLL.DTO;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface IDesignsSupplyRequestService : IDisposable
    {
        Task CreateDesignSupplyRequestAsync(DesignsSupplyRequestDTO designsSupplyRequest);
        Task UpdateDesignSupplyRequestAsync(DesignsSupplyRequestDTO designsSupplyRequest);
        Task<IEnumerable<DesignsSupplyRequestDTO>> GetDesignSupplyRequestsAsync();
        Task<DesignsSupplyRequestDTO> GetDesignSupplyRequestAsync(int? id);
        Task DeleteDesignSupplyRequestAsync(int? id);
    }
}
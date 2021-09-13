using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.BLL.DTO;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface IDesignsSupplyRequestService : IDisposable
    {
        Task CreateDesignSupplyRequestAsync(DesignsSupplyRequestDTO componentsSupplyRequest);
        Task UpdateDesignSupplyRequestAsync(DesignsSupplyRequestDTO componentsSupplyRequest);
        Task<IEnumerable<DesignsSupplyRequestDTO>> GetDesignSupplyRequestsAsync();
        Task<DesignsSupplyRequestDTO> GetDesignSupplyRequestAsync(int? id);
        Task DeleteDesignSupplyRequestAsync(int? id);
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.DAL.Entities;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface IComponentsSupplyRequestService : IDisposable
    {
        Task CreateComponentSupplyRequestAsync(ComponentsSupplyRequestDTO componentsSupplyRequest);
        Task UpdateComponentSupplyRequestAsync(ComponentsSupplyRequestDTO componentsSupplyRequest);
        Task<IEnumerable<ComponentsSupplyRequestDTO>> GetComponentSupplyRequestsAsync();
        Task<ComponentsSupplyRequestDTO> GetComponentSupplyRequestAsync(int? id);
        Task DeleteComponentSupplyRequestAsync(int? id);
    }
}
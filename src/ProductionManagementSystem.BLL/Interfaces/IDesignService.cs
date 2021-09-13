using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.BLL.DTO;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface IDesignService
    {
        Task CreateDesignAsync(DesignDTO designDto);
        Task UpdateDesignAsync(DesignDTO designDto);
        Task<IEnumerable<DesignDTO>> GetDesignsAsync();
        Task<DesignDTO> GetDesignAsync(int? id);
        Task DeleteDesignAsync(int? id);
        Task<IEnumerable<string>> GetTypesAsync();
        Task AddDesignAsync(int? id, int quantity);
        Task ReceiveDesignAsync(int? id, int quantity);
        
        void Dispose();
    }
}
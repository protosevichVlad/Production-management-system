using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.BLL.DTO;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface IComponentService
    {
        Task CreateComponentAsync(ComponentDTO componentDto);
        Task UpdateComponentAsync(ComponentDTO componentDto);
        Task<IEnumerable<ComponentDTO>> GetComponentsAsync();
        Task<ComponentDTO> GetComponentAsync(int? id);
        Task DeleteComponentAsync(int? id);
        Task<IEnumerable<string>> GetTypesAsync();
        Task AddComponentAsync(int? id, int quantity);
        Task ReceiveComponentAsync(int? id, int quantity);
    
        void Dispose();
    }
}
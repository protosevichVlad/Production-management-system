using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.DAL.Entities;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface IDeviceService
    {
        Task CreateDeviceAsync(DeviceDTO deviceDto);
        Task UpdateDeviceAsync(DeviceDTO deviceDto);
        Task<IEnumerable<DeviceDTO>> GetDevicesAsync();
        Task<DeviceDTO> GetDeviceAsync(int? id);
        Task DeleteDeviceAsync(int? id);
        Task<IEnumerable<string>> GetNamesAsync();
        Task<IEnumerable<DeviceComponentsTemplate>> GetComponentsTemplatesAsync(int deviceId);
        Task<IEnumerable<DeviceDesignTemplate>> GetDesignTemplatesAsync(int deviceId);
        Task AddDeviceAsync(int? id);
        Task ReceiveDeviceAsync(int? id);
        
        void Dispose();
    }
}
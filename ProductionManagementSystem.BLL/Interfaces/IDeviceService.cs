using System.Collections.Generic;
using ProductionManagementSystem.BLL.DTO;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface IDeviceService
    {
        void CreateDevice(DeviceDTO deviceDto);
        void UpdateDevice(DeviceDTO deviceDto);
        IEnumerable<DeviceDTO> GetDevices();
        DeviceDTO GetDevice(int? id);
        void DeleteDevice(int? id);
        IEnumerable<string> GetNames();
        
        void Dispose();
    }
}
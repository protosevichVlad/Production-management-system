using System.Collections.Generic;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.DAL.Entities;

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
        IEnumerable<DeviceComponentsTemplate> GetComponentsTemplates(int deviceId);
        IEnumerable<DeviceDesignTemplate> GetDesignTemplates(int deviceId);
        void AddDevice(int? id);
        void ReceiveDevice(int? id);
        
        void Dispose();
    }
}
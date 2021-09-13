using System;
using ProductionManagementSystem.DAL.Entities;

namespace ProductionManagementSystem.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Device> Devices { get; }
        IRepository<Design> Designs { get; }
        IRepository<Component> Components { get; }
        IRepository<Order> Orders { get; }
        IRepository<Task> Tasks { get; }
        IRepository<Log> Logs { get; }

        IRepository<DeviceComponentsTemplate> DeviceComponentsTemplate { get; }
        IRepository<DeviceDesignTemplate> DeviceDesignTemplate { get; }
        
        IRepository<ObtainedComponent> ObtainedСomponents { get; }
        IRepository<ObtainedDesign> ObtainedDesigns { get; }

        IRepository<ComponentsSupplyRequest> ComponentSupplyRequests { get; }
        IRepository<DesignsSupplyRequest> DesignsSupplyRequests { get; }

        void ResetDatabase();
        
        System.Threading.Tasks.Task SaveAsync();
    }
}
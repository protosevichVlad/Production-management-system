using System;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Interfaces;
using ProductionManagementSystem.DAL.Entities;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _db;
        private ComponentRepository _componentRepository;
        private DesignRepository _designRepository;
        private DeviceRepository _deviceRepository;
        private LogRepository _logRepository;
        private OrderRepository _orderRepository;
        private TaskRepository _taskRepository;
        private DeviceComponentsTemplateRepository _deviceComponentsTemplateRepository;
        private DeviceDesignTemplateRepository _deviceDesignTemplateRepository;
        private ObtainedDesignRepository _obtainedDesignRepository;
        private ObtainedComponentRepository _obtainedComponentRepository;

        public EFUnitOfWork(string connectionString)
        {
            _db = new ApplicationContextFactory().CreateDbContext(new []{connectionString});
        }
        
        public EFUnitOfWork(ApplicationContext applicationContext)
        {
            _db = applicationContext;
        }

        public IRepository<Device> Devices
        {
            get { return _deviceRepository ??= new DeviceRepository(_db); }
        }

        public IRepository<Design> Designs {
            get { return _designRepository ??= new DesignRepository(_db); }
        }
        
        public IRepository<Component> Components {
            get { return _componentRepository ??= new ComponentRepository(_db); }
        }

        public IRepository<Order> Orders
        {
            get { return _orderRepository ??= new OrderRepository(_db); }
        }

        public IRepository<Task> Tasks
        {
            get { return _taskRepository ??= new TaskRepository(_db); }
        }
        
        public IRepository<Log> Logs
        {
            get { return _logRepository ??= new LogRepository(_db); }
        }
        
        public IRepository<DeviceComponentsTemplate> DeviceComponentsTemplate {
            get
            {
                return _deviceComponentsTemplateRepository ??= new DeviceComponentsTemplateRepository(_db);
            }
        }
        
        public IRepository<DeviceDesignTemplate> DeviceDesignTemplate {
            get
            {
                return _deviceDesignTemplateRepository ??= new DeviceDesignTemplateRepository(_db);
            }
        }

        public IRepository<ObtainedComponent> ObtainedComponents
        {
            get
            {
                return _obtainedComponentRepository ??= new ObtainedComponentRepository(_db);
            }
        }

        public IRepository<ObtainedDesign> ObtainedDesigns
        {
            get { return _obtainedDesignRepository ??= new ObtainedDesignRepository(_db); }
        }
        
        public async System.Threading.Tasks.Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }

                _disposed = true;
            }
        }

        public void ResetDatabase()
        {
            _db.ResetDatabase();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
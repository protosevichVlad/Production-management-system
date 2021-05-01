using System;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Interfaces;
using ProductionManagementSystem.DAL.Entities;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private ApplicationContext _db;
        private ComponentRepository _componentRepository;
        private DesignRepository _designRepository;
        private DeviceRepository _deviceRepository;
        private LogRepository _logRepository;
        private OrderRepository _orderRepository;
        private TaskRepository _taskRepository;
        private DeviceComponentsTemplateRepository _deviceComponentsTemplateRepository;
        private DeviceDesignTemplateRepository _deviceDesignTemplateRepository;
        private ObtainedDesignRepository _obtainedDesignRepository;
        private ObtainedСomponentRepository _obtainedComponentRepository;

        public EFUnitOfWork(string connectionString)
        {
            _db = new ApplicationContextFactory().CreateDbContext(new []{connectionString});
        }


        public IRepository<Device> Devices
        {
            get
            {
                if (_deviceRepository == null)
                    _deviceRepository = new DeviceRepository(_db);
                return _deviceRepository;    
            }
        }

        public IRepository<Design> Designs {
            get
            {
                if (_designRepository == null)
                    _designRepository = new DesignRepository(_db);
                return _designRepository;
            }
        }
        
        public IRepository<Component> Components {
            get
            {
                if (_componentRepository == null)
                    _componentRepository = new ComponentRepository(_db);
                return _componentRepository;
            }
        }

        public IRepository<Order> Orders
        {
            get
            {
                if (_orderRepository == null)
                    _orderRepository = new OrderRepository(_db);
                return _orderRepository;
            }
        }

        public IRepository<Task> Tasks
        {
            get
            {
                if (_taskRepository == null)
                    _taskRepository = new TaskRepository(_db);
                return _taskRepository;
            }
        }
        
        public IRepository<Log> Logs
        {
            get
            {
                if (_logRepository == null)
                    _logRepository = new LogRepository(_db);
                return _logRepository;
            }
        }
        
        public IRepository<DeviceComponentsTemplate> DeviceComponentsTemplate {
            get
            {
                if (_deviceComponentsTemplateRepository == null)
                    _deviceComponentsTemplateRepository = new DeviceComponentsTemplateRepository(_db);
                return _deviceComponentsTemplateRepository;
            }
        }
        
        public IRepository<DeviceDesignTemplate> DeviceDesignTemplate {
            get
            {
                if (_deviceDesignTemplateRepository == null)
                    _deviceDesignTemplateRepository = new DeviceDesignTemplateRepository(_db);
                return _deviceDesignTemplateRepository;
            }
        }

        public IRepository<ObtainedComponent> ObtainedСomponents
        {
            get
            {
                if (_obtainedComponentRepository == null)
                    _obtainedComponentRepository = new ObtainedСomponentRepository(_db);
                return _obtainedComponentRepository;
            }
        }

        public IRepository<ObtainedDesign> ObtainedDesigns
        {
            get
            {
                if (_obtainedDesignRepository == null)
                    _obtainedDesignRepository = new ObtainedDesignRepository(_db);
                return _obtainedDesignRepository;
            }
        }
        
        public void Save()
        {
            _db.SaveChanges();
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
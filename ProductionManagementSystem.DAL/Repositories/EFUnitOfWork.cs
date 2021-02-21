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
        private RoleRepository _roleRepository;
        private TaskRepository _taskRepository;
        private UserRepository _userRepository;
        private DeviceComponentsTemplateRepository _deviceComponentsTemplateRepository;
        private DeviceDesignTemplateRepository _deviceDesignTemplateRepository;
        private LogComponentRepository _logComponentRepository;
        private LogDesignRepository _logDesignRepository;
        private ObtainedDesignRepository _obtainedDesignRepository;
        private ObtainedСomponentRepository _obtainedСomponentRepository;

        public EFUnitOfWork(string connectionString)
        {
            _db = new ApplicationContext(connectionString);
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
        
        public IRepository<User> Users {
            get
            {
                if (_userRepository == null)
                    _userRepository = new UserRepository(_db);
                return _userRepository;
            }
        }
        
        public IRepository<Role> Roles {
            get
            {
                if (_roleRepository == null)
                    _roleRepository = new RoleRepository(_db);
                return _roleRepository;
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

        public IRepository<ObtainedСomponent> ObtainedСomponents
        {
            get
            {
                if (_obtainedСomponentRepository == null)
                    _obtainedСomponentRepository = new ObtainedСomponentRepository(_db);
                return _obtainedСomponentRepository;
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
        
        public IRepository<LogDesign> LogsDesign {
            get
            {
                if (_logDesignRepository == null)
                    _logDesignRepository = new LogDesignRepository(_db);
                return _logDesignRepository;
            }
        }

        public IRepository<LogComponent> LogsComponent {
            get
            {
                if (_logComponentRepository == null)
                    _logComponentRepository = new LogComponentRepository(_db);
                return _logComponentRepository;
            }
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        private bool _disposed;

        public virtual void Dispose(bool disposing)
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
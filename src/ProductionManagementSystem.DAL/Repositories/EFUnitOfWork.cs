using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private ComponentsSupplyRequestRepository _componentsSupplyRequestRepository;
        private DesignsSupplyRequestRepository _designsSupplyRequestRepository;

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

        public IRepository<ComponentsSupplyRequest> ComponentSupplyRequests
        {
            get { return _componentsSupplyRequestRepository ??= new ComponentsSupplyRequestRepository(_db); }
        }
        
        public IRepository<DesignsSupplyRequest> DesignsSupplyRequests
        {
            get { return _designsSupplyRequestRepository ??= new DesignsSupplyRequestRepository(_db); }
        }

        public async System.Threading.Tasks.Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public void InitDb()
        {
            if (_db.Users.FirstOrDefault(u => u.UserName == "admin") != null)
            {
                return;
            }
            
            _db.Users.Add(new ProductionManagementSystemUser()
            {
                Id = "6c81929d-c900-4f7a-8fe8-ba7549ad8bf3",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                PasswordHash = "AQAAAAEAACcQAAAAEBpyITUr6yaFaqB+3Z/nZZojnX391pnEqSBpT3fBQd9ujaXRHVi4mfo+bb/VEeCjMw==",
                EmailConfirmed = true,
                SecurityStamp = "42ZFYQUKLLGWDHYY5CHQWG5KNWTKUYC5",
                ConcurrencyStamp = "62d87e43-c04a-4cd4-85bd-4dbe3d5e67a1"
            });
            
            _db.Roles.Add(new IdentityRole("Поверитель"));
            _db.Roles.Add(new IdentityRole("Настройщик"));
            _db.Roles.Add(new IdentityRole("Снабженец"));
            _db.Roles.Add(new IdentityRole("Отгрузчик"));
            _db.Roles.Add(new IdentityRole("Сборщик"));
            _db.Roles.Add(new IdentityRole("Администратор"));
            _db.Roles.Add(new IdentityRole("Монтажник"));
            _db.SaveChanges();
            
            _db.UserRoles.Add(new IdentityUserRole<string>()
            {
                UserId = "6c81929d-c900-4f7a-8fe8-ba7549ad8bf3",
                RoleId = _db.Roles.FirstOrDefault(r => r.Name == "Поверитель")?.Id,
            });
            _db.UserRoles.Add(new IdentityUserRole<string>()
            {
                UserId = "6c81929d-c900-4f7a-8fe8-ba7549ad8bf3",
                RoleId = _db.Roles.FirstOrDefault(r => r.Name == "Настройщик")?.Id,
            });
            _db.UserRoles.Add(new IdentityUserRole<string>()
            {
                UserId = "6c81929d-c900-4f7a-8fe8-ba7549ad8bf3",
                RoleId = _db.Roles.FirstOrDefault(r => r.Name == "Снабженец")?.Id,
            });
            _db.UserRoles.Add(new IdentityUserRole<string>()
            {
                UserId = "6c81929d-c900-4f7a-8fe8-ba7549ad8bf3",
                RoleId = _db.Roles.FirstOrDefault(r => r.Name == "Отгрузчик")?.Id,
            });
            _db.UserRoles.Add(new IdentityUserRole<string>()
            {
                UserId = "6c81929d-c900-4f7a-8fe8-ba7549ad8bf3",
                RoleId = _db.Roles.FirstOrDefault(r => r.Name == "Сборщик")?.Id,
            });
            _db.UserRoles.Add(new IdentityUserRole<string>()
            {
                UserId = "6c81929d-c900-4f7a-8fe8-ba7549ad8bf3",
                RoleId = _db.Roles.FirstOrDefault(r => r.Name == "Администратор")?.Id,
            });
            _db.UserRoles.Add(new IdentityUserRole<string>()
            {
                UserId = "6c81929d-c900-4f7a-8fe8-ba7549ad8bf3",
                RoleId = _db.Roles.FirstOrDefault(r => r.Name == "Монтажник")?.Id,
            });
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
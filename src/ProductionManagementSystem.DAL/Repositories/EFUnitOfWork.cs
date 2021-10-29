using System;
using System.Threading.Tasks;
using ProductionManagementSystem.DAL.EF;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IMontageRepository MontageRepository { get; }
        IDesignRepository DesignRepository { get; }
        IDeviceRepository DeviceRepository { get; }
        IMontageInDeviceRepository MontageInDeviceRepository { get; }
        IDesignInDeviceRepository DesignInDeviceRepository { get; }
        ITaskRepository TaskRepository { get; }
        IObtainedDesignRepository ObtainedDesignRepository { get; }
        IObtainedMontageRepository ObtainedMontageRepository { get; }
        IOrderRepository OrderRepository { get; }
        IDesignsSupplyRequestRepository DesignsSupplyRequestRepository { get; }
        IMontageSupplyRequestRepository MontageSupplyRequestRepository { get; }
        ILogRepository LogRepository { get; }

        void Save();
        Task SaveAsync();
    }
    
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _db;
        private IMontageRepository _montageRepository;
        private IDesignRepository _designRepository;
        private IDeviceRepository _deviceRepository;
        private ILogRepository _logRepository;
        private IOrderRepository _orderRepository;
        private ITaskRepository _taskRepository;
        private IMontageInDeviceRepository _montageInDeviceRepository;
        private IDesignInDeviceRepository _designInDeviceRepository;
        private IObtainedDesignRepository _obtainedDesignRepository;
        private IObtainedMontageRepository _obtainedMontageRepository;
        private IMontageSupplyRequestRepository _montageSupplyRequestRepository;
        private IDesignsSupplyRequestRepository _designsSupplyRequestRepository;

        public EFUnitOfWork(string connectionString)
        {
            _db = new ApplicationContextFactory().CreateDbContext(new []{connectionString});
        }
        
        public EFUnitOfWork(ApplicationContext applicationContext)
        {
            _db = applicationContext;
        }

        public IMontageRepository MontageRepository => _montageRepository ??= new MontageRepository(_db);
        public IDesignRepository DesignRepository => _designRepository ??= new DesignRepository(_db);
        public IDeviceRepository DeviceRepository => _deviceRepository ??= new DeviceRepository(_db);
        public IMontageInDeviceRepository MontageInDeviceRepository =>
            _montageInDeviceRepository ??= new MontageInDeviceRepository(_db);
        public IDesignInDeviceRepository DesignInDeviceRepository =>
            _designInDeviceRepository ?? new DesignInDeviceRepository(_db);
        public ITaskRepository TaskRepository => _taskRepository ??= new TaskRepository(_db);
        public IObtainedDesignRepository ObtainedDesignRepository =>
            _obtainedDesignRepository ??= new ObtainedDesignRepository(_db);

        public IObtainedMontageRepository ObtainedMontageRepository =>
            _obtainedMontageRepository ??= new ObtainedMontageRepository(_db);

        public IOrderRepository OrderRepository => _orderRepository ?? new OrderRepository(_db);

        public IDesignsSupplyRequestRepository DesignsSupplyRequestRepository =>
            _designsSupplyRequestRepository ??= new DesignsSupplyRequestRepository(_db);

        public IMontageSupplyRequestRepository MontageSupplyRequestRepository =>
            _montageSupplyRequestRepository ??= new MontageSupplyRequestRepository(_db);

        public ILogRepository LogRepository => _logRepository ??= new LogRepository(_db);
        
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
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        public void Save()
        {
            _db.SaveChanges();
        }
        
        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
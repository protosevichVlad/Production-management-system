using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Repositories.AltiumDB;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface IBaseUnitOfWork : IDisposable
    {
        void Save();
        Task SaveAsync();
    }
    
    public interface IUnitOfWork : IBaseUnitOfWork
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
        IElementDifferenceRepository ElementDifferenceRepository { get; }
        IDatabaseTableRepository DatabaseTableRepository { get; }
        IToDoNoteRepository ToDoNoteRepository { get; }
        IEntityExtRepository EntityExtRepository { get; }
        IPcbRepository Pcbs { get; }
        ICompDbDeviceRepository CompDbDeviceRepository { get; }
        ICDBObtainedRepository CdbObtainedRepository { get; }
        ICDBTasksRepository CdbTasksRepository { get; }
        IUsedItemRepository UsedItemRepository { get; }
        ICDBSupplyRequestRepository CdbSupplyRequestRepository { get; }

    }

    public class EF_BaseUnitOfWork : IBaseUnitOfWork
    {
        protected ApplicationContext _db;
        
        public EF_BaseUnitOfWork(string connectionString)
        {
            _db = new ApplicationContextFactory().CreateDbContext(new []{connectionString});
        }
        
        public EF_BaseUnitOfWork(ApplicationContext applicationContext)
        {
            _db = applicationContext;
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
    
    public class EFUnitOfWork : EF_BaseUnitOfWork, IUnitOfWork
    {
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
        private IElementDifferenceRepository _elementDifferenceRepository;
        private DatabaseTableRepository _databaseTableRepository;
        private IToDoNoteRepository _toDoNoteRepository;
        private IEntityExtRepository _entityExtRepository;
        private IPcbRepository _pcbRepository;
        private ICompDbDeviceRepository _compDbDeviceRepository;
        private ICDBObtainedRepository _cdbObtainedRepository;
        private ICDBTasksRepository _cdbTasksRepository;
        private IUsedItemRepository _usedItemRepository;
        private ICDBSupplyRequestRepository _cdbSupplyRequestRepository;

        public EFUnitOfWork(string connectionString) : base(connectionString)
        {
            _db = new ApplicationContextFactory().CreateDbContext(new []{connectionString});
        }
        
        public EFUnitOfWork(ApplicationContext applicationContext) : base(applicationContext)
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

        public IElementDifferenceRepository ElementDifferenceRepository =>
            _elementDifferenceRepository ??= new ElementDifferenceRepository(_db);
        
        public IDatabaseTableRepository DatabaseTableRepository =>
            _databaseTableRepository ??= new DatabaseTableRepository(_db);

        public IToDoNoteRepository ToDoNoteRepository =>
            _toDoNoteRepository ??= new ToDoNoteRepository(_db);

        public IEntityExtRepository EntityExtRepository => _entityExtRepository ??= new EntityExtRepository(_db);
        public IPcbRepository Pcbs => _pcbRepository ??= new PcbRepository(_db);
        public ICompDbDeviceRepository CompDbDeviceRepository => _compDbDeviceRepository ??= new CompDbDeviceRepository(_db);

        public ICDBObtainedRepository CdbObtainedRepository => _cdbObtainedRepository ??= new CDBObtainedRepository(_db);
        public ICDBTasksRepository CdbTasksRepository => _cdbTasksRepository ??= new CDBTasksRepository(_db);
        public IUsedItemRepository UsedItemRepository => _usedItemRepository ??= new UsedItemRepository(_db);

        public ICDBSupplyRequestRepository CdbSupplyRequestRepository =>
            _cdbSupplyRequestRepository ??= new CDBSupplyRequestRepository(_db);
    }
}
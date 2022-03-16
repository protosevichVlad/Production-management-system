using ProductionManagementSystem.Core.Data.EF;

namespace ProductionManagementSystem.Core.Repositories.AltiumDB
{
    public interface IAltiumDBUnitOfWork : IBaseUnitOfWork
    {
        public DatabaseTableRepository DatabaseTables { get; }
        public DirectoryRepository Directories { get; }
    }
    
    public class EF_AltiumDBUnitOfWork : EF_BaseUnitOfWork, IAltiumDBUnitOfWork
    {
        private DatabaseTableRepository _databaseTableRepository;
        private DirectoryRepository _directoryRepository;

        public EF_AltiumDBUnitOfWork(string connectionString) : base(connectionString)
        {
        }

        public EF_AltiumDBUnitOfWork(ApplicationContext applicationContext) : base(applicationContext)
        {
        }

        public DatabaseTableRepository DatabaseTables =>
            _databaseTableRepository ??= new DatabaseTableRepository(_db);
        public DirectoryRepository Directories => _directoryRepository ??= new DirectoryRepository(_db);
    }
}
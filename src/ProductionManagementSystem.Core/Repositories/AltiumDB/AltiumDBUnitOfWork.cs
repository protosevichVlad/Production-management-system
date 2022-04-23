using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;

namespace ProductionManagementSystem.Core.Repositories.AltiumDB
{
    public interface IAltiumDBUnitOfWork : IBaseUnitOfWork
    {
        public DatabaseTableRepository DatabaseTables { get; }
        IToDoNoteRepository ToDoNotes { get; }
        IEntityInPcbRepository EntityInPcbs { get; }
        
    }
    
    public class EF_AltiumDBUnitOfWork : EF_BaseUnitOfWork, IAltiumDBUnitOfWork
    {
        private DatabaseTableRepository _databaseTableRepository;
        private IPcbRepository _pcbRepository;
        private IToDoNoteRepository _toDoNoteRepository;
        private IEntityInPcbRepository _entityInPcb;
        
        public EF_AltiumDBUnitOfWork(string connectionString) : base(connectionString)
        {
        }

        public EF_AltiumDBUnitOfWork(ApplicationContext applicationContext) : base(applicationContext)
        {
        }

        public DatabaseTableRepository DatabaseTables =>
            _databaseTableRepository ??= new DatabaseTableRepository(_db);
        public IToDoNoteRepository ToDoNotes =>
            _toDoNoteRepository ??= new ToDoNoteRepository(_db);
        public IEntityInPcbRepository EntityInPcbs =>
            _entityInPcb ??= new EntityInPcbRepository(_db);
    }
}
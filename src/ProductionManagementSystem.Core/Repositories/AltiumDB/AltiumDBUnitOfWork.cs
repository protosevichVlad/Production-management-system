using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;

namespace ProductionManagementSystem.Core.Repositories.AltiumDB
{
    public interface IAltiumDBUnitOfWork : IBaseUnitOfWork
    {
        public DatabaseTableRepository DatabaseTables { get; }
        public IProjectRepository Projects { get; }
        IToDoNoteRepository ToDoNotes { get; }
        IEntityInProjectRepository EntityInProjects { get; }
        IAltiumDBEntityRepository AltiumDbEntityRepository { get; }
        
    }
    
    public class EF_AltiumDBUnitOfWork : EF_BaseUnitOfWork, IAltiumDBUnitOfWork
    {
        private DatabaseTableRepository _databaseTableRepository;
        private IProjectRepository _projectRepository;
        private IToDoNoteRepository _toDoNoteRepository;
        private IEntityInProjectRepository _entityInProject;
        private IAltiumDBEntityRepository _altiumDbEntityRepository;
        
        public EF_AltiumDBUnitOfWork(string connectionString) : base(connectionString)
        {
        }

        public EF_AltiumDBUnitOfWork(ApplicationContext applicationContext) : base(applicationContext)
        {
        }

        public DatabaseTableRepository DatabaseTables =>
            _databaseTableRepository ??= new DatabaseTableRepository(_db);
        public IProjectRepository Projects => _projectRepository ??= new ProjectRepository(_db);
        public IToDoNoteRepository ToDoNotes =>
            _toDoNoteRepository ??= new ToDoNoteRepository(_db);
        public IEntityInProjectRepository EntityInProjects =>
            _entityInProject ??= new EntityInProjectRepository(_db);

        public IAltiumDBEntityRepository AltiumDbEntityRepository =>
            _altiumDbEntityRepository ?? new AltiumDBEntityRepository(_db);
    }
}
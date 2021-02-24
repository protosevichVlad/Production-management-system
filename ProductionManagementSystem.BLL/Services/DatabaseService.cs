using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Interfaces;
using ProductionManagementSystem.DAL.Repositories;

namespace ProductionManagementSystem.BLL.Services
{
    public class DatabaseService : IDatabaseService
    {
        private IUnitOfWork _database { get; set; }

        public DatabaseService(IUnitOfWork uow)
        {
            _database = uow;
        }
        
        public void ResetDatabase()
        {
            _database.ResetDatabase();
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}
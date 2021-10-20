using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Tasks;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface ITaskRepository : IRepository<Task>
    {
        
    }
    public class TaskRepository : Repository<Task>, ITaskRepository
    {
        public TaskRepository(ApplicationContext db) : base(db)
        {
        }
    }
}
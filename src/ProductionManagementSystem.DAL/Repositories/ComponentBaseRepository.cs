using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IComponentBaseRepository<TComponent> : IRepository<TComponent>
        where TComponent : ComponentBase
    {
        
    }
    
    public class ComponentBaseRepository<TComponent> : Repository<TComponent>, IComponentBaseRepository<TComponent>
        where TComponent : ComponentBase
    {
        public ComponentBaseRepository(ApplicationContext db) : base(db)
        {
        }
    }
}
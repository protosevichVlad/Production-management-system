using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Services.AltiumDB;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface IDatabaseTableRepository : IRepository<DatabaseTable>
    {
        
    }
    
    public class DatabaseTableRepository : Repository<DatabaseTable>, IDatabaseTableRepository
    {
        public DatabaseTableRepository(ApplicationContext db) : base(db)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class ComponentsSupplyRequestRepository : IRepository<ComponentsSupplyRequest>
    {
        private ApplicationContext _db;

        public ComponentsSupplyRequestRepository(ApplicationContext context)
        {
            _db = context;
        }
        
        public async Task<IEnumerable<ComponentsSupplyRequest>> GetAllAsync()
        {
            return await _db.ComponentsSupplyRequests
                .Include(c => c.Component)                
                .ToListAsync();
        }

        public async Task<ComponentsSupplyRequest> GetAsync(int id)
        {
            return await _db.ComponentsSupplyRequests.FirstOrDefaultAsync(c => c.Id == id);
        }

        public IEnumerable<ComponentsSupplyRequest> Find(Func<ComponentsSupplyRequest, bool> predicate)
        {
            return _db.ComponentsSupplyRequests.Where(predicate).ToList();
        }

        public async Task CreateAsync(ComponentsSupplyRequest item)
        {
            await _db.ComponentsSupplyRequests.AddAsync(item);
        }

        public void Update(ComponentsSupplyRequest item)
        {
            var local = _db.Set<ComponentsSupplyRequest>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(item.Id));
            if (local != null)
            {
                _db.Entry(local).State = EntityState.Detached;
            }
            
            _db.Entry(item).State = EntityState.Modified;
        }

        public async Task DeleteAsync(int id)
        {
            ComponentsSupplyRequest componentsSupplyRequest = await GetAsync(id);
            if (componentsSupplyRequest != null)
                _db.ComponentsSupplyRequests.Remove(componentsSupplyRequest);
        }
    }
}
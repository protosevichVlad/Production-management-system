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
    public class DesignsSupplyRequestRepository : IRepository<DesignsSupplyRequest>
    {
        private ApplicationContext _db;

        public DesignsSupplyRequestRepository(ApplicationContext context)
        {
            _db = context;
        }
        
        public async Task<IEnumerable<DesignsSupplyRequest>> GetAllAsync()
        {
            return await _db.DesignsSupplyRequests.ToListAsync();
        }

        public async Task<DesignsSupplyRequest> GetAsync(int id)
        {
            return await _db.DesignsSupplyRequests.FirstOrDefaultAsync(d => d.Id == id);
        }

        public IEnumerable<DesignsSupplyRequest> Find(Func<DesignsSupplyRequest, bool> predicate)
        {
            return _db.DesignsSupplyRequests.Where(predicate).ToList();
        }

        public async Task CreateAsync(DesignsSupplyRequest item)
        {
            await _db.DesignsSupplyRequests.AddAsync(item);
        }

        public void Update(DesignsSupplyRequest item)
        {
            var local = _db.Set<DesignsSupplyRequest>()
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
            DesignsSupplyRequest designsSupplyRequest = await GetAsync(id);
            if (designsSupplyRequest != null)
                _db.DesignsSupplyRequests.Remove(designsSupplyRequest);
        }
    }
}
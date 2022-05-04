using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Exceptions;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.PCB;

namespace ProductionManagementSystem.Core.Repositories.AltiumDB
{
    public interface IPcbRepository : IRepository<Pcb>
    {
        Task<List<Pcb>> GetProjectsWithEntityAsync(string partNumber);
        Task<List<Pcb>> SearchByKeyWordAsync(string keyWord);
        Task UpdateQuantityAsync(int entityId, int quantity);
    }
    
    public class PcbRepository : Repository<Pcb>, IPcbRepository
    {
        private readonly EntityExtRepository _entityExtRepository;
        private readonly IUsedItemRepository _usedItemRepository;
        public PcbRepository(ApplicationContext db) : base(db)
        {
            _entityExtRepository = new EntityExtRepository(db);
            _usedItemRepository = new UsedItemRepository(db);
        }

        public async Task<List<Pcb>> GetProjectsWithEntityAsync(string partNumber)
        {
            var entity = await _db.Entities.FirstOrDefaultAsync(x => x.PartNumber == partNumber);
            if (entity == null) return new List<Pcb>();
            var pcbIds = await _db.UsedItems.Where(x =>
                x.ItemType == CDBItemType.Entity && x.ItemId == entity.KeyId && x.InItemType == CDBItemType.PCB).Select(x => x.InItemId).ToListAsync();

            var result = new List<Pcb>();
            foreach (var id in pcbIds)
            {
                var pcb = await GetByIdAsync(id);
                if (pcb != null)
                    result.Add(pcb);
            }

            return result;
        }

        public override async Task<Pcb> GetByIdAsync(int id)
        {
            var pcb = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (pcb == null) return null;
            await Include(pcb);
            return pcb;
        }

        public async Task<List<Pcb>> SearchByKeyWordAsync(string keyWord)
        {
            if (string.IsNullOrEmpty(keyWord))
                return new List<Pcb>();
            return await _dbSet.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(keyWord)).ToListAsync();
        }

        public async Task UpdateQuantityAsync(int entityId, int quantity)
        {
            var pcb = await _db.Projects.FindAsync(entityId);
            pcb.Quantity = quantity;
        }

        public override async Task UpdateAsync(Pcb item)
        {
            var ent = await _usedItemRepository.GetByPcbIdAsync(item.Id);
            ent = ent.Where(x => item.UsedItems.All(y => y.Id != x.Id)).ToList();
            _db.UsedItems.RemoveRange(ent); 
            foreach (var used in item.UsedItems)
            {
                _db.Entry(used).State = used.Id == 0 ? EntityState.Added : EntityState.Modified;
            }

            await base.UpdateAsync(item);
        }

        public override void Delete(Pcb item)
        {
            var usedInDevice = _db.UsedItems.Any(x => x.ItemType == CDBItemType.PCB && x.ItemId == item.Id);
            if (usedInDevice)
            {
                throw new DeleteReferenceException("PCB removal is not possible", $"This PCB used in device");
            }
            
            var ent = _usedItemRepository.GetByPcbIdAsync(item.Id);
            _db.UsedItems.RemoveRange(ent.Result);
            base.Delete(item);
        }

        private async Task Include(Pcb item)
        {
            item.UsedItems = (await _usedItemRepository.GetByPcbIdAsync(item.Id)).ToList();
        }
    }
}
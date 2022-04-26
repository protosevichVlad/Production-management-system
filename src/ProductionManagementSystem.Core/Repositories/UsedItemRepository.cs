using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.PCB;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface IUsedItemRepository : IRepository<UsedItem>
    {
        Task<List<UsedItem>> GetByPcbIdAsync(int pcbId);
        Task IncludeItems(List<UsedItem> usedItems);
        Task IncludeItems(UsedItem usedItem);
    }
    
    public class UsedItemRepository : Repository<UsedItem>, IUsedItemRepository
    {
        private readonly IEntityExtRepository _entityExtRepository;

        public UsedItemRepository(ApplicationContext db) : base(db)
        {
            _entityExtRepository = new EntityExtRepository(db);
        }
        
        public async Task<List<UsedItem>> GetByPcbIdAsync(int pcbId)
        {
            var result =  await _db.UsedItems.AsNoTracking().Where(x => x.InItemType == CDBItemType.PCB && x.InItemId == pcbId).ToListAsync();
            await IncludeItems(result);
            return result;
        }

        public async Task IncludeItems(List<UsedItem> usedItems)
        {
            foreach (var item in usedItems)
            {
                await IncludeItems(item);
            }
        }

        public async Task IncludeItems(UsedItem usedItem)
        {
            switch (usedItem.ItemType)
            {
                case CDBItemType.Entity:
                    usedItem.Item = new UniversalItem(await _entityExtRepository.GetByIdAsync(usedItem.ItemId));
                    break;
                case CDBItemType.PCB:
                    usedItem.Item = new UniversalItem(await _db.Projects.AsNoTracking().FirstOrDefaultAsync(x => x.Id == usedItem.ItemId));
                    break;
                case CDBItemType.Device:
                    usedItem.Item = new UniversalItem(await _db.CDBDevices.AsNoTracking().FirstOrDefaultAsync(x => x.Id == usedItem.ItemId));
                    break;
            }
                
            switch (usedItem.InItemType)
            {
                case CDBItemType.Entity:
                    usedItem.InItem = new UniversalItem(await _entityExtRepository.GetByIdAsync(usedItem.InItemId));
                    break;
                case CDBItemType.PCB:
                    usedItem.InItem = new UniversalItem(await _db.Projects.AsNoTracking().FirstOrDefaultAsync(x => x.Id == usedItem.InItemId));
                    break;
                case CDBItemType.Device:
                    usedItem.InItem = new UniversalItem(await _db.CDBDevices.AsNoTracking().FirstOrDefaultAsync(x => x.Id == usedItem.InItemId));
                    break;
            }
        }
    }
}
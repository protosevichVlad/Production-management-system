using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Models.ElementsDifference;
using ProductionManagementSystem.Core.Repositories;
using ProductionManagementSystem.Core.Services.AltiumDB;

namespace ProductionManagementSystem.Core.Services
{
    public interface ICalculableService
    {
        Task IncreaseQuantityAsync(int id, int quantity);
        Task DecreaseQuantityAsync(int id, int quantity);
        Task ChangeQuantityAsync(int id, int quantity);
    }
    
    public interface IEntityExtService : IBaseService<EntityExt>, ICalculableService
    {
        Task<List<EntityExt>> GetFromTable(int tableId);
        Task<EntityExt> GetEntityExtByPartNumber(string partNumber);
        Task<List<string>> GetValues(string column);
        Task<List<string>> GetValues(string column, string tableName);
        Task<List<string>> GetValues(string column, int? tableId);
        Task DeleteByIdAsync(int id);
        Task<List<EntityExt>> SearchByKeyWordAsync(string s, int? tableId=null);
    }

    public class EntityExtService : BaseService<EntityExt, IUnitOfWork>, IEntityExtService
    {
        private readonly IToDoNoteService _toDoNoteService;
        public EntityExtService(IUnitOfWork db, IToDoNoteService toDoNoteService) : base(db)
        {
            _toDoNoteService = toDoNoteService;
            _currentRepository = _db.EntityExtRepository;
        }
        
        public async Task<List<EntityExt>> GetFromTable(int tableId)
        {
            return await _db.EntityExtRepository.GetAllByTableId(tableId);
        }

        public async Task<EntityExt> GetEntityExtByPartNumber(string partNumber)
        {
            return await _db.EntityExtRepository.GetByPartNumber(partNumber);
        }

        public async Task<List<string>> GetValues(string column)
        {
            return await _db.EntityExtRepository.GetValues(column);
        }

        public async Task<List<string>> GetValues(string column, string tableName)
        {
            return await _db.EntityExtRepository.GetValues(column, tableName);
        }

        public async Task<List<string>> GetValues(string column, int? tableId)
        {
            return await _db.EntityExtRepository.GetValues(column, tableId);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var entity = await _db.EntityExtRepository.GetByIdAsync(id);
            if (entity == null) throw new NotImplementedException();
            _db.EntityExtRepository.Delete(entity);
        }

        public async Task<List<EntityExt>> SearchByKeyWordAsync(string s, int? tableId=null)
        {
            return await _db.EntityExtRepository.SearchByKeyWordAsync(s, tableId);
        }
        
        private async Task AddToToDoNotes(Table table, EntityExt data)
        {
            if ((await GetValues("Library Ref", table.Id)).Count(x => data.LibraryRef == x) == 0)
            {
                await _toDoNoteService.CreateAsync(new ToDoNote()
                {
                    Completed = false,
                    CreatedDateTime = DateTime.Now,
                    Description = "",
                    Title = $"Create Library Ref: '{data.LibraryRef}' for {table.DisplayName} table",
                });
            }
            
            if ((await GetValues("Footprint Ref", table.Id)).Count(x => data.FootprintRef == x) == 0)
            {
                await _toDoNoteService.CreateAsync(new ToDoNote()
                {
                    Completed = false,
                    CreatedDateTime = DateTime.Now,
                    Description = "",
                    Title = $"Create Footprint Ref: '{data.FootprintRef}' for {table.DisplayName} table",
                });
            }

            await _db.SaveAsync();
        }

        public override async Task CreateAsync(EntityExt item)
        {
            var table = await _db.DatabaseTableRepository.GetByIdAsync(item.TableId);
            await AddToToDoNotes(table, item);
            await base.CreateAsync(item);
        }

        public override async Task UpdateAsync(EntityExt item)
        {
            var table = await _db.DatabaseTableRepository.GetByIdAsync(item.TableId);
            await AddToToDoNotes(table, item);
            await base.UpdateAsync(item);
        }
        
        public async Task IncreaseQuantityAsync(int id, int quantity)
        {
            await ChangeQuantityAsync(id, quantity);
        }
        
        public async Task DecreaseQuantityAsync(int id, int quantity)
        {
            await ChangeQuantityAsync(id, -quantity);
        }

        public async Task ChangeQuantityAsync(int id, int quantity)
        {
            if (quantity == 0)
            {
                return;
            }

            var entity = await _currentRepository.GetByIdAsync(id);
            entity.Quantity += quantity;
            if (entity.Quantity < 0)
                throw new NotImplementedException();
            await _currentRepository.UpdateAsync(entity);

            await _db.ElementDifferenceRepository.CreateAsync(new ElementDifference()
                {Difference = quantity, ElementId = entity.KeyId, ElementType = ElementType.Montage});
            await _db.SaveAsync();
        }
    }
}
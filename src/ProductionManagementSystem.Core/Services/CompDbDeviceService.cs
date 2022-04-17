using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.ElementsDifference;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services
{
    public interface ICompDbDeviceService : IBaseService<CompDbDevice>, ICalculableObject
    {
        Task<List<CompDbDevice>> SearchByKeyWordAsync(string s);
        Task DeleteByIdAsync(int id);
    }

    public class CompDbDeviceService : BaseService<CompDbDevice, IUnitOfWork>, ICompDbDeviceService
    {
        public CompDbDeviceService(IUnitOfWork db) : base(db)
        {
            _currentRepository = _db.CompDbDeviceRepository;
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

            var item = await _currentRepository.GetByIdAsync(id);
            item.Quantity += quantity;
            if (item.Quantity < 0)
                throw new NotImplementedException();
            await _currentRepository.UpdateAsync(item);

            await _db.ElementDifferenceRepository.CreateAsync(new ElementDifference()
                {Difference = quantity, ElementId = item.Id, ElementType = ElementType.Device});
            await _db.SaveAsync();
        }

        public async Task<List<CompDbDevice>> SearchByKeyWordAsync(string s)
        {
            return await _db.CompDbDeviceRepository.SearchByKeyWordAsync(s);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var item = await base.GetByIdAsync(id);
            await base.DeleteAsync(item);
        }
    }
}
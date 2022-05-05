using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.ElementsDifference;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services
{
    public interface ICompDbDeviceService : IBaseService<CompDbDevice>, ICalculableService
    {
        Task CreateAsync(CreateEditDevice device);
        Task UpdateAsync(CreateEditDevice device);
        Task<List<CompDbDevice>> SearchByKeyWordAsync(string s);
        Task DeleteByIdAsync(int id);
        Task<CompDbDevice> GetLatest();
        Task<bool> UsingInDevice(int deviceId, int entityId);
    }

    public class CompDbDeviceService : BaseService<CompDbDevice, IUnitOfWork>, ICompDbDeviceService
    {
        private readonly IFileService _fileService;
        
        public CompDbDeviceService(IUnitOfWork db, IFileService fileService) : base(db)
        {
            _fileService = fileService;
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
            if (item == null) return;
            item.Quantity += quantity;
            if (item.Quantity < 0)
                return;
            await _db.CompDbDeviceRepository.UpdateQuantityAsync(item.Id, item.Quantity);

            await _db.ElementDifferenceRepository.CreateAsync(new ElementDifference()
                {Difference = quantity, ElementId = item.Id, ElementType = ElementType.Device});
            await _db.SaveAsync();
        }

        public async Task CreateAsync(CreateEditDevice device)
        {
            await CreateAsync(await device.GetDevice(_fileService));
        }

        public async Task UpdateAsync(CreateEditDevice device)
        {
            await UpdateAsync(await device.GetDevice(_fileService));
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

        public async Task<CompDbDevice> GetLatest()
        {
            var devices = await _currentRepository.GetAllAsync();
            var deviceId = devices.FirstOrDefault(x => x.Id == devices.Max(y => y.Id)).Id;
            return await GetByIdAsync(deviceId);
        }

        public async Task<bool> UsingInDevice(int deviceId, int entityId)
        {
            return (await _db.UsedItemRepository.GetByDeviceIdAsync(deviceId)).Any(x =>
                x.ItemId == entityId && x.ItemType == CDBItemType.Entity);
        }

        public override async Task CreateAsync(CompDbDevice item)
        {
            if (item.ReportDate == default)
            {
                item.ReportDate = DateTime.Now;
            }
            
            await base.CreateAsync(item);
            item.UsedItems.ForEach(x => x.InItemId = item.Id);
            await _db.UsedItemRepository.CreateRangeAsync(item.UsedItems);
            await _db.SaveAsync();
        }

        public override async Task UpdateAsync(CompDbDevice item)
        {
            var deviceFromDb = await GetByIdAsync(item.Id);
            item.ImagePath = await _fileService.GetNewUrl(item.ImagePath, deviceFromDb.ImagePath);
            item.ThreeDModelPath = await _fileService.GetNewUrl(item.ThreeDModelPath, deviceFromDb.ThreeDModelPath);
            
            await base.UpdateAsync(item);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Components;
using ProductionManagementSystem.Models.Logs;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.BLL.Services
{
    public interface IMontageService : IBaseService<Montage>
    {
        Task<IEnumerable<string>> GetTypesAsync();
        Task IncreaseQuantityOfMontageAsync(int id, int quantity);
        Task DecreaseQuantityOfDesignAsync(int id, int quantity);
        Task DeleteByIdAsync(int id);
    }

    public class MontageService : BaseServiceWithLogs<Montage>, IMontageService
    {
        private readonly ILogService _log;
        
        public MontageService(IUnitOfWork uow) : base(uow)
        {
            _log = new LogService(uow);
            _currentRepository = _db.MontageRepository;
        }
        
        public override async Task DeleteAsync(Montage montage)
        {
            var checkInDevices = (await CheckInDevicesAsync(montage));
            string errorMessage = checkInDevices.Item2;
            if (!checkInDevices.Item1)
            {
                throw new IntersectionOfEntitiesException("Ошибка. Невозможно удаление монтажа.", errorMessage);
            }

            await base.DeleteAsync(montage);
        }

        public async Task<IEnumerable<string>> GetTypesAsync()
        {
            var montages = await GetAllAsync();
            IEnumerable<string> types = montages.OrderBy(c => c.Type).Select(c => c.Type).Distinct();
            return types;
        }

        public async Task IncreaseQuantityOfMontageAsync(int id, int quantity)
        {
            if (quantity == 0)
            {
                return;
            }

            var montage = await GetByIdAsync(id);
            montage.Quantity += quantity;
            await UpdateAsync(montage);

            if (quantity < 0)
            {
                await _log.CreateAsync(new Log() {Message = $"Было получено {-quantity}ед. монтажа {montage}", MontageId = montage.Id});
            }
            else
            {
                await _log.CreateAsync(new Log() {Message = $"Было добавлено {quantity}ед. монтажа {montage}", MontageId = montage.Id});
            }
        }

        public async Task DecreaseQuantityOfDesignAsync(int id, int quantity)
        {
            await this.IncreaseQuantityOfMontageAsync(id, -quantity);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns>Return true, if component not using in devices.</returns>
        private async Task<Tuple<bool, string>> CheckInDevicesAsync(Montage montage)
        {
            string errorMessage;
            var montageInDevice = (await _db.MontageInDeviceRepository.GetAllAsync())
                .FirstOrDefault(c => montage.Id == c.ComponentId);
            if (montageInDevice != null)
            {
                var device = (await _db.DeviceRepository.GetAllAsync()).FirstOrDefault(d => d.Id == montageInDevice.DeviceId);
                errorMessage = $"<i class='bg-light'>{montage}</i> используется в <i class='bg-light'>{device}</i>.<br />" +
                               $"Для удаления <i class='bg-light'>{montage}</i>, удалите <i class='bg-light'>{device}</i>.<br />";
                return new Tuple<bool, string>(false, errorMessage);
            }

            errorMessage = String.Empty;
            return new Tuple<bool, string>(true, errorMessage);
        }
        
        private async Task<bool> ComponentExistsAsync(int id)
        {
            return (await GetAllAsync()).Any(e => e.Id == id);
        }

        public async Task DeleteByIdAsync(int id)
        {
            await this.DeleteAsync(new Montage() {Id = id});
        }
        
         
        protected override async Task CreateLogForCreatingAsync(Montage item)
        {
            await _db.LogRepository.CreateAsync(new Log()
                { Message = "Был создан монтаж " + item.ToString(), MontageId = item.Id });
        }

        protected override async Task CreateLogForUpdatingAsync(Montage item)
        {
            await _db.LogRepository.CreateAsync(new Log()
                { Message = "Был изменён монтаж " + item.ToString(), MontageId = item.Id });
        }

        protected override async Task CreateLogForDeletingAsync(Montage item)
        {
            await _db.LogRepository.CreateAsync(new Log()
                { Message = "Был удалён монтаж " + item.ToString(), MontageId = item.Id });
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Devices;
using ProductionManagementSystem.Models.Logs;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.BLL.Services
{
    public interface IDeviceService : IBaseService<Device>
    {
        Task<IEnumerable<string>> GetNamesAsync();
        Task AddDeviceAsync(int? id);
        Task ReceiveDeviceAsync(int? id);
    }

    public class DeviceService : BaseService<Device>, IDeviceService
    {
        private readonly ILogService _log;
        
        public DeviceService(IUnitOfWork uow) : base(uow)
        {
            _log = new LogService(uow);
        }
        
        public override async Task UpdateAsync(Device device)
        {
            var checkInTask = await CheckInTaskAsync(device);
            string errorMessage = checkInTask.Item2;
            if (!checkInTask.Item1)
            {
                throw new IntersectionOfEntitiesException("Ошибка. Невозможно изменение прибора.", errorMessage);
            }
            
            await base.UpdateAsync(device);
        }

        public override async Task DeleteAsync(Device device)
        {
            var checkInTask = (await CheckInTaskAsync(device));
            string errorMessage = checkInTask.Item2;
            if (!checkInTask.Item1)
            {
                throw new IntersectionOfEntitiesException("Ошибка. Невозможно удаление прибора.", errorMessage);
            }

            await base.DeleteAsync(device);
        }

        private async Task<Tuple<bool, string>> CheckInTaskAsync(Device device)
        {
            string errorMessage;
            var task = _db.TaskRepository.GetAll()
                .FirstOrDefault(t => device.Id == t.DeviceId);
            if (task != null)
            {
                errorMessage = $"<i class='bg-light'>{device.ToString()}</i> используется в <i class='bg-light'>задаче №{task.Id}</i>.<br />";
                return new Tuple<bool, string>(false, errorMessage);
            }
            
            errorMessage = String.Empty;
            return new Tuple<bool, string>(true, errorMessage);
        }

        public async Task<IEnumerable<string>> GetNamesAsync()
        {
            var devices = GetAll();
            if (devices == null)
            {
                return Array.Empty<string>();
            }

            return devices.Select(d => d.ToString());
        }
        
        public async Task ReceiveDeviceAsync(int? id)
        {
            await AddDeviceAsync(id, -1);
        }
        
        public async Task AddDeviceAsync(int? id)
        {
            await AddDeviceAsync(id, 1);
        }
        
        private async Task AddDeviceAsync(int? id, int quantity)
        {
            if (!id.HasValue)
            {
                throw new PageNotFoundException();
            }

            var device = await GetByIdAsync(id.Value);
            device.Quantity += quantity;
            await UpdateAsync(device);

            if (quantity < 0)
            {
                await _log.CreateAsync(new Log() { Message = $"Был получен прибор {device} со склада {quantity}шт.", DeviceId = device.Id});
            }
            else
            {
                await _log.CreateAsync(new Log() { Message = $"Был добавлен прибор {device} на склад {-quantity}шт.", DeviceId = device.Id});
            }
        }
    }
}
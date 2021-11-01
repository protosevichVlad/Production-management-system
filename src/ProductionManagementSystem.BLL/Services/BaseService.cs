﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Components;
using ProductionManagementSystem.Models.Devices;
using ProductionManagementSystem.Models.Logs;
using ProductionManagementSystem.Models.Orders;
using ProductionManagementSystem.Models.SupplyRequests;

namespace ProductionManagementSystem.BLL.Services
{
    public interface IBaseService<TItem> : IDisposable
    {
        public Task<IEnumerable<TItem>> GetAllAsync();
        public Task<TItem> GetByIdAsync(int id);
        public Task<IEnumerable<TItem>> Find(Func<TItem, bool> predicate);
        public Task CreateAsync(TItem item);
        public Task UpdateAsync(TItem item);
        public Task DeleteAsync(TItem item);
    }
    
    public abstract class BaseService<TItem> : IBaseService<TItem>
    {
        protected IUnitOfWork _db;
        protected IRepository<TItem> _currentRepository;

        protected BaseService(IUnitOfWork db)
        {
            _db = db;
        }

        public virtual async Task<IEnumerable<TItem>> GetAllAsync()
        {
            return await _currentRepository.GetAllAsync();
        }

        public virtual async Task<TItem> GetByIdAsync(int id)
        {
            return await _currentRepository.GetByIdAsync(id);
        }

        public virtual async Task<IEnumerable<TItem>> Find(Func<TItem, bool> predicate)
        {
            return await _currentRepository.FindAsync(predicate);
        }

        public virtual async Task CreateAsync(TItem item)
        {
            await _currentRepository.CreateAsync(item);
            CreateLogAsync(item);
            
            await _db.SaveAsync();
        }

        public virtual async Task UpdateAsync(TItem item)
        {
            await _currentRepository.UpdateAsync(item);
            await _db.SaveAsync();
            //TODO: await _log.CreateLogAsync(new LogDTO($"Был изменён конструктив {design}"){DesignId = design.Id});
        }

        public virtual async Task DeleteAsync(TItem item)
        {
            _currentRepository.Delete(item);
            await _db.SaveAsync();
            
            // TODO: await _log.CreateLogAsync(new LogDTO($"Был удалён конструктив: {design}"));
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        private async void CreateLogAsync(TItem item)
        {
            if (item is Montage montage)
                await _db.LogRepository.CreateAsync(new Log()
                    { Message = "Был создан монтаж " + montage.ToString(), MontageId = montage.Id });
            if (item is Design design)
                await _db.LogRepository.CreateAsync(new Log()
                    { Message = "Был создан конструктив " + design.ToString(), DesignId = design.Id });
            if (item is Device device)
                await _db.LogRepository.CreateAsync(new Log()
                    { Message = "Был создан прибор " + device.ToString(), DesignId = device.Id });
            if (item is Task task)
                await _db.LogRepository.CreateAsync(new Log()
                    { Message = "Была создана задача " + task.ToString(), TaskId = task.Id });
            if (item is Order order)
                await _db.LogRepository.CreateAsync(new Log()
                    { Message = "Был создан заказ " + order.ToString(), OrderId = order.Id });
            if (item is MontageSupplyRequest montageSupplyRequest)
                await _db.LogRepository.CreateAsync(new Log()
                    { Message = "Была создана заявка на снабжения монтажа " + montageSupplyRequest.ToString(), MontageSupplyRequestId = montageSupplyRequest.Id });
            if (item is DesignSupplyRequest designSupplyRequest)
                await _db.LogRepository.CreateAsync(new Log()
                    { Message = "Была создана заявка на снабжения конструктива " + designSupplyRequest.ToString(), DesignSupplyRequestId = designSupplyRequest.Id });
        }
    }
}
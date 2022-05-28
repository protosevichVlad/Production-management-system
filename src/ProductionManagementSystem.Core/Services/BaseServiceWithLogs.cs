using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.Logs;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services
{
    public abstract class BaseServiceWithLogs<TItem> : BaseService<TItem, IUnitOfWork>, IBaseService<TItem>
        where TItem : BaseEntity
    {
        protected abstract LogsItemType ItemType { get; }
        protected BaseServiceWithLogs(IUnitOfWork db) : base(db)
        {
            _db = db;
        }

        public new virtual async Task CreateAsync(TItem item)
        {
            await _currentRepository.CreateAsync(item);
            await _db.SaveAsync();
            await _db.LogRepository.CreateAsync(await GetLog(LogEventType.Add, item));
            await _db.SaveAsync();
        }

        public new virtual async Task UpdateAsync(TItem item)
        {
            await _db.LogRepository.CreateAsync(await GetLog(LogEventType.Update, item));
            await _currentRepository.UpdateAsync(item);
            await _db.SaveAsync();
        }

        public new virtual async Task DeleteAsync(TItem item)
        {
            await _db.LogRepository.CreateAsync(await GetLog(LogEventType.Delete, item));
            _currentRepository.Delete(item);
            await _db.SaveAsync();
        }
        
        protected virtual async Task<Log> GetLog(LogEventType eventType, TItem newEntity, TItem oldEntity=null)
        {
            var itemId = GetEntityId(newEntity) ?? 0;
            Log log = new Log(ItemType, itemId);
            await AddMessageToLog(log, eventType, newEntity, oldEntity);
            return log;
        }

        protected virtual async Task AddMessageToLog(Log log, LogEventType eventType, TItem newEntity, TItem oldEntity = null)
        {
            var startMessage = eventType switch
            {
                LogEventType.Add => "Создал",
                LogEventType.Delete => "Удалил",
                LogEventType.Update => "Изменил"
            };

            startMessage += " " + ItemType switch
            {
                LogsItemType.Design => "конструктив",
                LogsItemType.Montage => "монтаж",
                LogsItemType.Device => "прибор",
                LogsItemType.Task => "задачу",
                LogsItemType.Order => "заказ",
                LogsItemType.DesignSupplyRequest => "заявку на снабжение",
                LogsItemType.MontageSupplyRequest => "заявку на снабжение",
            };
            
            log.AddMessage($"{startMessage}: {newEntity}.");

            if (eventType == LogEventType.Delete)
                return;

            if (eventType == LogEventType.Add)
            {
                foreach (var propName in Porperties<TItem>())
                {
                    var value = GetPropValue(newEntity, propName);
                    if (value == null)
                        continue;

                    log.AddMessageParameterSet(GetPropertyDisplayName<TItem>(propName), value.ToString());
                }

                return;
            }

            if (oldEntity == null)
            {
                var id = GetEntityId(newEntity);
                if (!id.HasValue)
                    return;

                oldEntity = (await _currentRepository.FindAsync(x => x.Id == id)).FirstOrDefault();
            }

            foreach (var propName in Porperties<TItem>())
            {
                var newValue = GetPropValue(newEntity, propName)?.ToString() ?? "";
                var oldValue = GetPropValue(oldEntity, propName)?.ToString() ?? "";
                log.AddMessageParameterChange(GetPropertyDisplayName<TItem>(propName), oldValue, newValue);
            }
        }

        protected virtual List<string> Porperties<TEntity>()
        {
            return typeof(TEntity).GetProperties()
                .Where(x => x.PropertyType != typeof(Guid))
                .Select(x => x.Name)
                .Where(x => !x.ToLower().EndsWith("id"))
                .ToList();
        }

        protected virtual async Task<List<Log>> GetLog(LogEventType eventType,List<TItem> newEntities)
        {
            List<Log> logs = new List<Log>();
            foreach (var entity in newEntities)
            {
                logs.Add(await GetLog(eventType, entity));
            }

            return logs;
        }

        protected virtual int? GetEntityId(TItem model)
        {
            var kyePropName = model.GetType().GetProperties().Select(x => x.Name).FirstOrDefault(x => x.ToLower().Contains("id"));
            if (kyePropName == null) return null;
            if (int.TryParse(GetPropValue(model, kyePropName).ToString(), out int id))
            {
                return id;
            }

            return null;
        }


        protected virtual object GetPropValue(TItem src, string propName)
        {
            var value = src.GetType().GetProperty(propName).GetValue(src, null);
            if (value is DateTime dateTime)
                return dateTime.ToString("dd.MM.yyyy HH:mm:ss");

            return value;
        }

        protected enum LogEventType
        {
            Add = 1,
            Update = 2,
            Delete = 3,
        }
        
        private string GetPropertyDisplayName<TItem>(string propertyName)
        {
            List<string> result = new List<string>();
            return typeof(TItem)
                .GetProperty(propertyName)
                ?.GetCustomAttribute<DisplayAttribute>()
                ?.GetName() ?? propertyName;
        }
    }
}
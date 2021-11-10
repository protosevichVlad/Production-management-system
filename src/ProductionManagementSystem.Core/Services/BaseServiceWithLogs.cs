using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.Logs;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services
{
    public abstract class BaseServiceWithLogs<TItem> : BaseService<TItem>, IBaseService<TItem>
        where TItem : BaseEntity
    {
        protected BaseServiceWithLogs(IUnitOfWork db) : base(db)
        {
            _db = db;
        }

        public new virtual async Task CreateAsync(TItem item)
        {
            await _currentRepository.CreateAsync(item);
            await _db.SaveAsync();
            await CreateLogForCreatingAsync(item);
            await _db.SaveAsync();
        }

        public new virtual async Task UpdateAsync(TItem item)
        {
            await _currentRepository.UpdateAsync(item);
            await CreateLogForUpdatingAsync(item);
            await _db.SaveAsync();
        }

        public new virtual async Task DeleteAsync(TItem item)
        {
            _currentRepository.Delete(item);
            await CreateLogForDeletingAsync(item);
            await DeleteLogsAsync(item);
            await _db.SaveAsync();
        }

        private async Task DeleteLogsAsync(TItem item)
        {
            var logs = (await _db.LogRepository.FindAsync(l => UpdateLogPredicate(l, item))).ToList();
            logs.ForEach(UpdateLog);
            await _db.LogRepository.UpdateRangeAsync(logs);
            await _db.SaveAsync();
        }

        protected abstract Task CreateLogForCreatingAsync(TItem item);
        protected abstract Task CreateLogForUpdatingAsync(TItem item);
        protected abstract Task CreateLogForDeletingAsync(TItem item);
        protected abstract bool UpdateLogPredicate(Log log, TItem item);
        protected abstract void UpdateLog(Log log);

    }
}
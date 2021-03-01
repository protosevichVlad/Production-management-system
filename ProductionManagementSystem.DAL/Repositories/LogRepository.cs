using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class LogRepository : IRepository<Log>
    {
        private ApplicationContext _db;

        public LogRepository(ApplicationContext context)
        {
            _db = context;
        }

        public IEnumerable<Log> GetAll()
        {
            return _db.Logs;
        }

        public Log Get(int id)
        {
            return _db.Logs.Find(id);
        }

        public IEnumerable<Log> Find(Func<Log, bool> predicate)
        {
            return _db.Logs.Where(predicate).ToList();
        }

        public void Create(Log item)
        {
            _db.Logs.Add(item);
        }

        public void Update(Log item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = _db.Logs.Find(id);
            if (item != null)
                _db.Logs.Remove(item);
        }
    }
}
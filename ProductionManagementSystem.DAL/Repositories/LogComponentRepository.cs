using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class LogComponentRepository : IRepository<LogComponent>
    {
        private ApplicationContext _db;

        public LogComponentRepository(ApplicationContext context)
        {
            _db = context;
        }

        public IEnumerable<LogComponent> GetAll()
        {
            return _db.LogComponents;
        }

        public LogComponent Get(int id)
        {
            return _db.LogComponents.Find(id);
        }

        public IEnumerable<LogComponent> Find(Func<LogComponent, bool> predicate)
        {
            return _db.LogComponents.Where(predicate).ToList();
        }

        public void Create(LogComponent item)
        {
            _db.LogComponents.Add(item);
        }

        public void Update(LogComponent item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = _db.LogComponents.Find(id);
            if (item != null)
                _db.LogComponents.Remove(item);
        }
    }
}
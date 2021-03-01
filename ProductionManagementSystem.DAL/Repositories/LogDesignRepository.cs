using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class LogDesignRepository : IRepository<LogDesign>
    {
        private ApplicationContext _db;

        public LogDesignRepository(ApplicationContext context)
        {
            _db = context;
        }

        public IEnumerable<LogDesign> GetAll()
        {
            return _db.LogDesigns;
        }

        public LogDesign Get(int id)
        {
            return _db.LogDesigns.Find(id);
        }

        public IEnumerable<LogDesign> Find(Func<LogDesign, bool> predicate)
        {
            return _db.LogDesigns.Where(predicate).ToList();
        }

        public void Create(LogDesign item)
        {
            _db.LogDesigns.Add(item);
        }

        public void Update(LogDesign item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = _db.LogDesigns.Find(id);
            if (item != null)
                _db.LogDesigns.Remove(item);
        }
    }
}
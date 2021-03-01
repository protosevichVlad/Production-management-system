using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class ObtainedСomponentRepository : IRepository<ObtainedComponent>
    {
        private ApplicationContext _db;

        public ObtainedСomponentRepository(ApplicationContext context)
        {
            _db = context;
        }

        public IEnumerable<ObtainedComponent> GetAll()
        {
            return _db.ObtainedСomponents
                .Include(c => c.Task)
                .Include(c => c.Component);
        }

        public ObtainedComponent Get(int id)
        {
            return _db.ObtainedСomponents
                .Include(c => c.Task)
                .Include(c => c.Component)
                .FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<ObtainedComponent> Find(Func<ObtainedComponent, bool> predicate)
        {
            return _db.ObtainedСomponents
                .Include(c => c.Task)
                .Include(c => c.Component)
                .Where(predicate).ToList();
        }

        public void Create(ObtainedComponent item)
        {
            _db.ObtainedСomponents.Add(item);
        }

        public void Update(ObtainedComponent item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = _db.ObtainedСomponents.Find(id);
            if (item != null)
                _db.ObtainedСomponents.Remove(item);
        }
    }
}
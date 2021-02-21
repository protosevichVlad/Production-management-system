using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class ObtainedСomponentRepository : IRepository<ObtainedСomponent>
    {
        private ApplicationContext _db;

        public ObtainedСomponentRepository(ApplicationContext context)
        {
            _db = context;
        }

        public IEnumerable<ObtainedСomponent> GetAll()
        {
            return _db.ObtainedСomponents;
        }

        public ObtainedСomponent Get(int id)
        {
            return _db.ObtainedСomponents.Find(id);
        }

        public IEnumerable<ObtainedСomponent> Find(Func<ObtainedСomponent, bool> predicate)
        {
            return _db.ObtainedСomponents.Where(predicate).ToList();
        }

        public void Create(ObtainedСomponent item)
        {
            _db.ObtainedСomponents.Add(item);
        }

        public void Update(ObtainedСomponent item)
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
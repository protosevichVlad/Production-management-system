using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class DesignRepository : IRepository<Design>
    {
        private ApplicationContext _db;

        public DesignRepository(ApplicationContext context)
        {
            _db = context;
        }

        public IEnumerable<Design> GetAll()
        {
            return _db.Designs;
        }

        public Design Get(int id)
        {
            return _db.Designs.Find(id);
        }

        public IEnumerable<Design> Find(Func<Design, bool> predicate)
        {
            return _db.Designs.Where(predicate).ToList();
        }

        public void Create(Design item)
        {
            _db.Designs.Add(item);
        }

        public void Update(Design item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = _db.Designs.Find(id);
            if (item != null)
                _db.Designs.Remove(item);
        }
    }
}
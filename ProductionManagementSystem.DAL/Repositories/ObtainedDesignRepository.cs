using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class ObtainedDesignRepository : IRepository<ObtainedDesign>
    {
        private ApplicationContext _db;

        public ObtainedDesignRepository(ApplicationContext context)
        {
            _db = context;
        }

        public IEnumerable<ObtainedDesign> GetAll()
        {
            return _db.ObtainedDesigns;
        }

        public ObtainedDesign Get(int id)
        {
            return _db.ObtainedDesigns.Find(id);
        }

        public IEnumerable<ObtainedDesign> Find(Func<ObtainedDesign, bool> predicate)
        {
            return _db.ObtainedDesigns.Where(predicate).ToList();
        }

        public void Create(ObtainedDesign item)
        {
            _db.ObtainedDesigns.Add(item);
        }

        public void Update(ObtainedDesign item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = _db.ObtainedDesigns.Find(id);
            if (item != null)
                _db.ObtainedDesigns.Remove(item);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class DeviceDesignTemplateRepository : IRepository<DeviceDesignTemplate>
    {
        private ApplicationContext _db;

        public DeviceDesignTemplateRepository(ApplicationContext context)
        {
            _db = context;
        }

        public IEnumerable<DeviceDesignTemplate> GetAll()
        {
            return _db.DeviceDesignTemplates;
        }

        public DeviceDesignTemplate Get(int id)
        {
            return _db.DeviceDesignTemplates.Find(id);
        }

        public IEnumerable<DeviceDesignTemplate> Find(Func<DeviceDesignTemplate, bool> predicate)
        {
            return _db.DeviceDesignTemplates.Where(predicate).ToList();
        }

        public void Create(DeviceDesignTemplate item)
        {
            _db.DeviceDesignTemplates.Add(item);
        }

        public void Update(DeviceDesignTemplate item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = _db.DeviceDesignTemplates.Find(id);
            if (item != null)
                _db.DeviceDesignTemplates.Remove(item);
        }
    }
}
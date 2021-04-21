using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class DeviceComponentsTemplateRepository : IRepository<DeviceComponentsTemplate>
    {
        private ApplicationContext _db;

        public DeviceComponentsTemplateRepository(ApplicationContext context)
        {
            _db = context;
        }

        public IEnumerable<DeviceComponentsTemplate> GetAll()
        {
            return _db.DeviceComponentsTemplates;
        }

        public DeviceComponentsTemplate Get(int id)
        {
            return _db.DeviceComponentsTemplates.Find(id);
        }

        public IEnumerable<DeviceComponentsTemplate> Find(Func<DeviceComponentsTemplate, bool> predicate)
        {
            return _db.DeviceComponentsTemplates.Where(predicate).ToList();
        }

        public void Create(DeviceComponentsTemplate item)
        {
            _db.DeviceComponentsTemplates.Add(item);
        }

        public void Update(DeviceComponentsTemplate item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = _db.DeviceComponentsTemplates.Find(id);
            if (item != null)
                _db.DeviceComponentsTemplates.Remove(item);
        }
    }
}
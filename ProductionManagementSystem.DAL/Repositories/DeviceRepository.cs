using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class DeviceRepository : IRepository<Device>
    {
        private ApplicationContext _db;

        public DeviceRepository(ApplicationContext context)
        {
            _db = context;
        }

        public IEnumerable<Device> GetAll()
        {
            return _db.Devices;
        }

        public Device Get(int id)
        {
            return _db.Devices.Find(id);
        }

        public IEnumerable<Device> Find(Func<Device, bool> predicate)
        {
            return _db.Devices.Where(predicate).ToList();
        }

        public void Create(Device item)
        {
            _db.Devices.Add(item);
        }

        public void Update(Device item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = _db.Devices.Find(id);
            if (item != null)
                _db.Devices.Remove(item);
        }
    }
}
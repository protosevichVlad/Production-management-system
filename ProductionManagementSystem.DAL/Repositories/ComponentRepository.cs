using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class ComponentRepository : IRepository<Component>
    {
        private ApplicationContext _db;

        public ComponentRepository(ApplicationContext context)
        {
            _db = context;
        }

        public IEnumerable<Component> GetAll()
        {
            return _db.Components;
        }

        public Component Get(int id)
        {
            return _db.Components.Find(id);
        }

        public IEnumerable<Component> Find(Func<Component, bool> predicate)
        {
            return _db.Components.Where(predicate).ToList();
        }

        public void Create(Component item)
        {
            _db.Components.Add(item);
        }

        public void Update(Component item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            Component component = _db.Components.Find(id);
            if (component != null)
                _db.Components.Remove(component);
        }
    }
}
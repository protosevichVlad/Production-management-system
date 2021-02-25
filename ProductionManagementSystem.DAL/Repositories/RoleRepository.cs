﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class RoleRepository : IRepository<Role>
    {
        private ApplicationContext _db;

        public RoleRepository(ApplicationContext context)
        {
            _db = context;
        }

        public IEnumerable<Role> GetAll()
        {
            return _db.Roles;
        }

        public Role Get(int id)
        {
            return _db.Roles.Find(id);
        }

        public IEnumerable<Role> Find(Func<Role, bool> predicate)
        {
            return _db.Roles.Where(predicate).ToList();
        }

        public void Create(Role item)
        {
            _db.Roles.Add(item);
        }

        public void Update(Role item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = _db.Roles.Find(id);
            if (item != null)
                _db.Roles.Remove(item);
        }
    }
}
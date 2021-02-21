using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class TaskRepository : IRepository<Task>
    {
        private ApplicationContext _db;

        public TaskRepository(ApplicationContext context)
        {
            _db = context;
        }

        public IEnumerable<Task> GetAll()
        {
            return _db.Tasks;
        }

        public Task Get(int id)
        {
            return _db.Tasks.Find(id);
        }

        public IEnumerable<Task> Find(Func<Task, bool> predicate)
        {
            return _db.Tasks.Where(predicate).ToList();
        }

        public void Create(Task item)
        {
            _db.Tasks.Add(item);
        }

        public void Update(Task item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = _db.Tasks.Find(id);
            if (item != null)
                _db.Tasks.Remove(item);
        }
    }
}
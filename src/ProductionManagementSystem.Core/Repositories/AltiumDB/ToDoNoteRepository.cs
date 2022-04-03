﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Repositories.AltiumDB
{
    public interface IToDoNoteRepository : IRepository<ToDoNote>
    {
        Task<List<ToDoNote>> GetNotCompletedAsync();
    }

    public class ToDoNoteRepository : Repository<ToDoNote>, IToDoNoteRepository
    {
        public ToDoNoteRepository(ApplicationContext db) : base(db)
        {
        }

        public override async Task<List<ToDoNote>> GetAllAsync()
        {
            return await _db.ToDoNotes
                .Include(x => x.CompletedBy)
                .Include(x => x.CreatedBy)
                .ToListAsync();
        }

        public async Task<List<ToDoNote>> GetNotCompletedAsync()
        {
            return await _dbSet.Where(x => x.Completed == false).ToListAsync();
        }
    }
}
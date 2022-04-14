using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Repositories;
using ProductionManagementSystem.Core.Repositories.AltiumDB;

namespace ProductionManagementSystem.Core.Services.AltiumDB
{
    public interface IToDoNoteService : IBaseService<ToDoNote>
    {
        Task<List<ToDoNote>> GetNotCompletedAsync();
        Task MarkAsCompleted(int id);
        Task DeleteByIdAsync(int id);
    }
    public class ToDoNoteService : BaseService<ToDoNote, IAltiumDBUnitOfWork>, IToDoNoteService
    {
        private IUserService _userService;
        public ToDoNoteService(IAltiumDBUnitOfWork db, IUserService userService) : base(db)
        {
            _currentRepository = _db.ToDoNotes;
            _userService = userService;
        }

        public async Task<List<ToDoNote>> GetNotCompletedAsync()
        {
            return await _db.ToDoNotes.GetNotCompletedAsync();
        }

        public async Task MarkAsCompleted(int id)
        {
            var note = await _db.ToDoNotes.GetByIdAsync(id);
            note.Completed = true;
            note.CompletedById = _userService.User.Id;
            note.CompletedDateTime = DateTime.Now;
            await _db.ToDoNotes.UpdateAsync(note);
            await _db.SaveAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var item = await base.GetByIdAsync(id);
            await base.DeleteAsync(item);
        }

        public override async Task CreateAsync(ToDoNote item)
        {
            item.CreatedById = _userService.User.Id;
            await base.CreateAsync(item);
        }
    }
}
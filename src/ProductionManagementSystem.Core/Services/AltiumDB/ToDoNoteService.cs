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
    }
    public class ToDoNoteService : BaseService<ToDoNote, IAltiumDBUnitOfWork>, IToDoNoteService
    {
        public ToDoNoteService(IAltiumDBUnitOfWork db) : base(db)
        {
            _currentRepository = _db.ToDoNotes;
        }

        public async Task<List<ToDoNote>> GetNotCompletedAsync()
        {
            return await _db.ToDoNotes.GetNotCompletedAsync();
        }

        public async Task MarkAsCompleted(int id)
        {
            var note = await _db.ToDoNotes.GetByIdAsync(id);
            note.Completed = true;
            await _db.ToDoNotes.UpdateAsync(note);
            await _db.SaveAsync();
        }
    }
}
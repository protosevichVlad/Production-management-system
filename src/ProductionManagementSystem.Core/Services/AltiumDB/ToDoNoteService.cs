using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services.AltiumDB
{
    public interface IToDoNoteService : IBaseService<ToDoNote>
    {
        Task<List<ToDoNote>> GetNotCompletedAsync();
        Task MarkAsCompleted(int id);
    }
    public class ToDoNoteService : BaseService<ToDoNote>, IToDoNoteService
    {
        public ToDoNoteService(IUnitOfWork db) : base(db)
        {
            _currentRepository = _db.ToDoNoteRepository;
        }

        public async Task<List<ToDoNote>> GetNotCompletedAsync()
        {
            return await _db.ToDoNoteRepository.GetNotCompletedAsync();
        }

        public async Task MarkAsCompleted(int id)
        {
            var note = await _db.ToDoNoteRepository.GetByIdAsync(id);
            note.Completed = true;
            await _db.ToDoNoteRepository.UpdateAsync(note);
            await _db.SaveAsync();
        }
    }
}
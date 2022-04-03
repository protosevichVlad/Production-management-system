using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Services.AltiumDB;

namespace ProductionManagementSystem.WEB.Controllers.API
{
    public class ToDoNotesApiController : Controller
    {
        private IToDoNoteService _toDoNoteService;
            
        public ToDoNotesApiController(IToDoNoteService toDoNoteService)
        {
            _toDoNoteService = toDoNoteService;
        }

        [Route("/api/AltiumDB/to-do/{id:int}/completed")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MarkAsCompleted([FromRoute]int id)
        {
            await _toDoNoteService.MarkAsCompleted(id);
            return Ok();
        }
    }
}
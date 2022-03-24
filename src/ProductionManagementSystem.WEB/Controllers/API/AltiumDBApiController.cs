using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Services.AltiumDB;

namespace ProductionManagementSystem.WEB.Controllers.API
{
    public class AltiumDBApiController : Controller
    {
        private IDatabaseService _databaseService;
        private IDirectoryService _directoryService;
        
        public AltiumDBApiController(IDatabaseService databaseService, IDirectoryService directoryService)
        {
            _databaseService = databaseService;
            _directoryService = directoryService;
        }

        [Route("/api/AltiumDB/get-path-by-directory-id/{id:int}")]
        public async Task<string> GetPathByDirectoryId([FromRoute]int id)
        {
            return await _directoryService.GetPathByDirectoryIdAsync(id);
        }
        
        [Route("/api/AltiumDB/get-path-by-table-id/{id:int}")]
        public async Task<string> GetPathByTableId([FromRoute]int id)
        {
            return await _directoryService.GetPathByTableIdAsync(id);
        }
    }
}
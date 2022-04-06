using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.Core.Services.AltiumDB;

namespace ProductionManagementSystem.WEB.Controllers
{
    public class EntitiesController : Controller
    {
        private readonly IEntityExtService _entityExtService;
        private readonly IImportService _importService;

        public EntitiesController(IEntityExtService entityExtService, IImportService importService)
        {
            _entityExtService = entityExtService;
            _importService = importService;
        }

        public async Task<IActionResult> Index(int? tableId)
        {
            if (tableId.HasValue)
                return View(await _entityExtService.GetFromTable(tableId.Value));
            return View(await _entityExtService.GetAllAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var entityExt =  await _entityExtService.GetByIdAsync(id);
            return View(entityExt);
        }

        [HttpGet]
        public async Task<IActionResult> Import()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            var tableName = file.FileName.Split('.')[0];
            var fileExtension = file.FileName.Split('.')[^1];
            switch (fileExtension)
            {
                case "csv":
                    await _importService.ImportFromFile(tableName, new StreamReader(file.OpenReadStream()), new CsvDataImporter());
                    return RedirectToAction(nameof(Index));
                case "xlsx":
                    await _importService.ImportFromFile(tableName, new StreamReader(file.OpenReadStream()), new ExcelImporter());
                    break;
                
                default:
                    throw new NotImplementedException();
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.Core.Services.AltiumDB;

namespace ProductionManagementSystem.WEB.Controllers
{
    public class EntitiesController : Controller
    {
        private readonly IEntityExtService _entityExtService;
        private readonly IImportService _importService;
        private readonly ITableService _tableService;

        public EntitiesController(IEntityExtService entityExtService, IImportService importService, ITableService tableService)
        {
            _entityExtService = entityExtService;
            _importService = importService;
            _tableService = tableService;
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
        
        public async Task<IActionResult> Create(int? tableId)
        {
            if (tableId.HasValue)
            {
                var table = await _tableService.GetByIdAsync(tableId.Value);
                await AddEntityHintsAsync(table.TableName);
                return View(new EntityExt(table)
                {
                    TableId = table.Id
                });
            }

            ViewBag.Tabels = await _tableService.GetAllAsync();
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(EntityExt entityExt)
        {
            await _entityExtService.CreateAsync(entityExt);
            return RedirectToAction(nameof(Details), new {id = entityExt.KeyId});
        }
        
        private async Task AddEntityHintsAsync(string tableName)
        {
            ViewBag.Manufacturers = await _entityExtService.GetValues("Manufacturer");
            ViewBag.Categories = await _entityExtService.GetValues("Category", tableName);
            ViewBag.Suppliers = await _entityExtService.GetValues("Supplier");
            ViewBag.Cases = await _entityExtService.GetValues("Case", tableName);
            ViewBag.LibraryRefs = await _entityExtService.GetValues("Library Ref", tableName);
            ViewBag.FootprintRefs = await _entityExtService.GetValues("Footprint Ref", tableName);
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
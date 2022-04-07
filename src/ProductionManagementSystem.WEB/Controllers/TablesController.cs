using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Services.AltiumDB;

namespace ProductionManagementSystem.WEB.Controllers
{
    public class TablesController : Controller
    {
        private readonly ITableService _tableService;

        public TablesController(ITableService tableService)
        {
            _tableService = tableService;
        }


        public async Task<ViewResult> Index()
        {
            return View(await _tableService.GetAllAsync());
        }

        public async Task<ViewResult> Create()
        {
            var table = new Table();
            table.InitAltiumDB("");
            return View(table);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(Table table)
        {
            await _tableService.CreateAsync(table);
            return RedirectToAction(nameof(Index), "Entities", new {tableId = table.Id});
        }
        
        
        public async Task<ViewResult> Edit(int id)
        {
            return View("Create", await _tableService.GetByIdAsync(id));
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(Table table)
        {
            await _tableService.UpdateAsync(table);
            return RedirectToAction(nameof(Index), "Entities", new {tableId = table.Id});
        }
        
        [HttpDelete]
        [Route("/Tables/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _tableService.DeleteByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> GetPartialViewForTableColumn(int index)
        {
            return PartialView("Table/TableColumn", new TableColumn()
            {
                DatabaseOrder = index,
            });
        }
    }
}
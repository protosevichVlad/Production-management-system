using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Services.AltiumDB;
using ProductionManagementSystem.WEB.Models.AltiumDB;
using ProductionManagementSystem.WEB.Models.Components;

namespace ProductionManagementSystem.WEB.Controllers
{
    public class AltiumDBController : Controller
    {
        private IDatabaseService _databaseService;
        private IDirectoryService _directoryService;
        
        public AltiumDBController(IDatabaseService databaseService, IDirectoryService directoryService)
        {
            _databaseService = databaseService;
            _directoryService = directoryService;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<ViewResult> Tables()
        {
            return View(await _databaseService.GetAllAsync());
        }

        [Authorize(Roles = RoleEnum.AltiumDBTablesAdmin)]
        public async Task<ViewResult> CreateTable()
        {
            var table = new DatabaseTable();
            table.InitAltiumDB("");
            ViewBag.DirectoriesTree = new TreeViewViewModel(await _directoryService.GetByIdAsync(0), false) { ShowPath = true};
            return View(table);
        }

        [HttpPost]
        [Authorize(Roles = RoleEnum.AltiumDBTablesAdmin)]
        public async Task<IActionResult> CreateTable(DatabaseTable table)
        {
            await _databaseService.CreateAsync(table);
            return RedirectToAction(nameof(GetDataFromTable), new {tableName = table.TableName});
        }

        [HttpGet]
        [Route("[controller]/Tables/{tableName}")]
        public async Task<IActionResult> GetDataFromTable(string tableName, string orderBy, Dictionary<string, List<string>> filter)
        {
            var data = await _databaseService.GetDataFromTableAsync(tableName);
            var table = await _databaseService.GetTableByNameAsync(tableName);
            List<FilterViewModel> filters = new List<FilterViewModel>();
            foreach (var column in table.TableColumns.Where(x => BaseAltiumDbEntity.NotFilterFields.All(y => y != x.ColumnName)))
            {
                data = data.Where(x => !filter.ContainsKey(column.ColumnName) || filter[column.ColumnName].Contains(x[column.ColumnName])).ToList();
                filters.Add(new FilterViewModel()
                {
                    FilterName = column.ColumnName,
                    Values = (await _databaseService.GetFiledTable(tableName, column.ColumnName)).Where(x => !string.IsNullOrEmpty(x))
                        .Select(x => ( x, filter.ContainsKey(column.ColumnName) && filter[column.ColumnName].Contains(x)))
                        .ToList()
                });
                if (filters[^1].Values.Count < 2)
                    filters.Remove(filters[^1]);
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                if (orderBy.EndsWith("_desc"))
                {
                    data = data.OrderByDescending(x => x[orderBy.Substring(0, orderBy.Length - "_desc".Length)]).ToList();
                }
                else
                {
                    data = data.OrderBy(x => x[orderBy]).ToList();
                }
            }
            
            DataListViewModel vm = new DataListViewModel()
            {
                DatabaseTable = table,
                Data = data,
                Filters = filters
            };
            return View("GetDataFromTable", vm);
        }

        [HttpGet]
        [Route("[controller]/EditTable/{tableName}")]
        [Authorize(Roles = RoleEnum.AltiumDBTablesAdmin)]
        public async Task<IActionResult> EditTable(string tableName)
        {
            var table = await _databaseService.GetTableByNameAsync(tableName);
            ViewBag.DirectoriesTree = new TreeViewViewModel(await _directoryService.GetByIdAsync(0), false)
            {
                ShowPath = true,
                SelectedId = table.DirectoryId ?? 0,
            };
            return View("CreateTable", table);
        }
        
        [HttpPost]
        [Route("[controller]/EditTable/{tableName}")]
        [Authorize(Roles = RoleEnum.AltiumDBTablesAdmin)]
        public async Task<IActionResult> EditTable(DatabaseTable table)
        {
            await _databaseService.UpdateAsync(table);
            return RedirectToAction(nameof(GetDataFromTable), new {tableName = table.TableName});
        }
        
        [Authorize(Roles = RoleEnum.AltiumDBTablesAdmin)]
        public async Task<IActionResult> GetPartialViewForTableColumn(int index)
        {
            return PartialView("Shared/Table/TableColumn", new TableColumn()
            {
                DatabaseOrder = index,
            });
        }

        [HttpDelete]
        [Authorize(Roles = RoleEnum.AltiumDBTablesAdmin)]
        [Route("AltiumDB/Tables/{tableName}")]
        public async Task<IActionResult> Tables([FromRoute]string tableName)
        {
            await _databaseService.DeleteByTableNameAsync(tableName);
            return Ok();
        }
        
        [HttpGet]
        [Authorize(Roles = RoleEnum.AltiumDBEntitiesAdmin)]
        [Route("AltiumDB/Tables/{tableName}/CreateEntity")]
        public async Task<IActionResult> CreateEntity([FromRoute]string tableName)
        {
            var table = await _databaseService.GetTableByNameAsync(tableName);
            await AddEntityHintsAsync(tableName);
            return View("CreateEntity", new BaseAltiumDbEntity(table));
        }
        
        [HttpPost]
        [Authorize(Roles = RoleEnum.AltiumDBEntitiesAdmin)]
        [Route("AltiumDB/Tables/{tableName}/CreateEntity")]
        public async Task<IActionResult> CreateEntity([FromRoute]string tableName, BaseAltiumDbEntity data)
        {
            await _databaseService.InsertIntoTableByTableNameAsync(tableName, data);
            return RedirectToAction(nameof(GetDataFromTable), new {tableName = tableName});
        }
        
        [HttpGet]
        [Authorize(Roles = RoleEnum.AltiumDBEntitiesAdmin)]
        [Route("AltiumDB/Tables/{tableName}/EditEntity/{partNumber}")]
        public async Task<IActionResult> EditEntity([FromRoute]string tableName, [FromRoute]string partNumber)
        {
            var table = await _databaseService.GetTableByNameAsync(tableName);
            var data = await _databaseService.GetEntityByPartNumber(tableName, partNumber);
            await AddEntityHintsAsync(tableName);
            return View("CreateEntity", data);
        }

        private async Task AddEntityHintsAsync(string tableName)
        {
            ViewBag.Manufacturers = await _databaseService.GetFiledFromAllTables("Manufacturer");
            ViewBag.Categories = await _databaseService.GetFiledTable(tableName, "Category");
            ViewBag.Suppliers = await _databaseService.GetFiledFromAllTables("Supplier");
            ViewBag.Cases = await _databaseService.GetFiledTable(tableName, "Case");
            ViewBag.LibraryRefs = await _databaseService.GetFiledTable(tableName, "Library Ref");
            ViewBag.FootprintRefs = await _databaseService.GetFiledTable(tableName, "Footprint Ref");
        }

        [HttpPost]
        [Authorize(Roles = RoleEnum.AltiumDBEntitiesAdmin)]
        [Route("AltiumDB/Tables/{tableName}/EditEntity/{partNumber}")]
        public async Task<IActionResult> EditEntity([FromRoute]string tableName, [FromRoute]string partNumber, BaseAltiumDbEntity data)
        {
            await _databaseService.UpdateEntityAsync(tableName, partNumber,data);
            return RedirectToAction(nameof(GetDataFromTable), new {tableName = tableName});
        }
        
        [HttpDelete]
        [Authorize(Roles = RoleEnum.AltiumDBEntitiesAdmin)]
        [Route("AltiumDB/Tables/{tableName}/{partNumber}")]
        public async Task<IActionResult> Tables([FromRoute]string tableName, [FromRoute]string partNumber)
        {
            await _databaseService.DeleteEntityById(tableName, partNumber);
            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = RoleEnum.AltiumDBTablesAdmin)]
        [Route("AltiumDB/ImportTables")]
        public async Task<IActionResult> ImportTables()
        {
            return View();
        }
        
        [HttpPost]
        [Authorize(Roles = RoleEnum.AltiumDBTablesAdmin)]
        [Route("AltiumDB/ImportTables")]
        public async Task<IActionResult> ImportTables(IFormFile file)
        {
            var tableName = file.FileName.Split('.')[0];
            var fileExtension = file.FileName.Split('.')[^1];
            switch (fileExtension)
            {
                case "csv":
                    await _databaseService.ImportFromFile(tableName, new StreamReader(file.OpenReadStream()), new CsvDataImporter());
                    return RedirectToAction(nameof(GetDataFromTable), new {tableName = "AltiumDB " + tableName});
                case "xlsx":
                case "xls":
                    await _databaseService.ImportFromFile(tableName, new StreamReader(file.OpenReadStream()), new ExcelImporter());
                    break;
                
                default:
                    throw new NotImplementedException();
            }
            
            return RedirectToAction(nameof(Tables));
        }
        
        [HttpGet]
        [Route("AltiumDB/Tables/{tableName}/{partNumber}")]
        public async Task<IActionResult> EntityDetails([FromRoute]string tableName, [FromRoute]string partNumber)
        {
            var table = await _databaseService.GetTableByNameAsync(tableName);
            var data = await _databaseService.GetEntityByPartNumber(tableName, partNumber);
            return View(data);
        }
        
        [HttpGet]
        [Route("AltiumDB/Directories")]
        public async Task<IActionResult> Directories()
        {
            return View(new TreeViewViewModel(await _directoryService.GetByIdAsync(0), true){ ShowPath = true});
        }
    }
}
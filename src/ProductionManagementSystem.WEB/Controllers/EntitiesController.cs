using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.Core.Services.AltiumDB;
using ProductionManagementSystem.WEB.Models.AltiumDB;

namespace ProductionManagementSystem.WEB.Controllers
{
    public class EntitiesController : Controller
    {
        private readonly IEntityExtService _entityExtService;
        private readonly IProjectService _projectService;
        private readonly IImportService _importService;
        private readonly ITableService _tableService;
        private const string USING_IN_PROJECTS = "Using in projects";
        

        public EntitiesController(IEntityExtService entityExtService, IImportService importService, ITableService tableService, IProjectService projectService)
        {
            _entityExtService = entityExtService;
            _importService = importService;
            _tableService = tableService;
            _projectService = projectService;
        }

        public async Task<IActionResult> Index(string orderBy, Dictionary<string, List<string>> filter, string q, int? tableId)
        {
            return View(await GetEntities(orderBy, filter, q, tableId));
        }

        public async Task<IActionResult> ChangeQuantity(string orderBy, Dictionary<string, List<string>> filter,
            string q, int? tableId)
        {
            return View(await GetEntities(orderBy, filter, q, tableId));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeQuantity(int[] id, int[] quantity, string type)
        {
            for (int i = 0; i < quantity.Length; i++)
            {
                if (type == "add")
                    await _entityExtService.IncreaseQuantityAsync(id[i], quantity[i]);
                else if (type == "get")
                    await _entityExtService.DecreaseQuantityAsync(id[i], quantity[i]);
            }
            return Redirect(HttpContext.Request.Path + HttpContext.Request.QueryString);
        }

        private async Task<DataListViewModel> GetEntities(string orderBy, Dictionary<string, List<string>> filter,
            string q, int? tableId)
        {
            var table = tableId.HasValue ? await _tableService.GetByIdAsync(tableId.Value):null;
            var data = string.IsNullOrEmpty(q) ? 
                tableId.HasValue ? 
                    await _entityExtService.GetFromTable(tableId.Value): 
                    await _entityExtService.GetAllAsync()
                : await _entityExtService.SearchByKeyWordAsync(q, tableId);
            
            List<FilterViewModel> filters = new List<FilterViewModel>();
            var fields = table?.TableColumns?.Where(x => x.Display)?.Select(x => x.ColumnName) ?? AltiumDbEntity.Fields;
            foreach (var column in fields.Where(x => AltiumDbEntity.NotFilterFields.All(y => y != x)))
            {
                data = data.Where(x => !filter.ContainsKey(column) || filter[column].Contains(x[column])).ToList();
                filters.Add(new FilterViewModel()
                {
                    FilterName = column,
                    Values = (await _entityExtService.GetValues(column, tableId)).OrderBy(x => x)
                        .Select(x => ( x, x, filter.ContainsKey(column) && filter[column].Contains(x)))
                        .ToList()
                });
                if (filters[^1].Values.Count < 2)
                    filters.Remove(filters[^1]);
            }
            
            data = data
                .Where(x =>
                {
                    bool Predicate(string y) => _projectService.GetProjectsWithEntityAsync(x.PartNumber).Result.Any(p => p.Id == int.Parse(y));
                    return !filter.ContainsKey(USING_IN_PROJECTS) ||
                           filter[USING_IN_PROJECTS].Any(Predicate
                           );
                }).ToList();
            
            filters.Add(new FilterViewModel()
            {
                FilterName = USING_IN_PROJECTS,
                Values = (await _projectService.GetAllAsync()).OrderBy(x => x)
                    .Select(x => ( x.Id.ToString(), x.ToString(), filter.ContainsKey(USING_IN_PROJECTS) && filter[USING_IN_PROJECTS].Contains(x.Id.ToString())))
                    .ToList()
            });

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
            
            return new DataListViewModel()
            {
                Table = table,
                Data = data.Select(x => (EntityExt)x).ToList(),
                Filters = filters
            };
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
                await AddEntityHintsAsync(tableId.Value);
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
        
        public async Task<IActionResult> Edit(int id)
        {
            EntityExt entityExt = await _entityExtService.GetByIdAsync(id);
            await AddEntityHintsAsync(entityExt.TableId);
            return View("Create", entityExt);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(EntityExt entityExt)
        {
            await _entityExtService.UpdateAsync(entityExt);
            return RedirectToAction(nameof(Details), new {id = entityExt.KeyId});
        }

        [HttpDelete]
        [Route("/Entities/{id:int}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            await _entityExtService.DeleteByIdAsync(id);
            return Ok();
        }
        
        private async Task AddEntityHintsAsync(int tableId)
        {
            ViewBag.Manufacturers = await _entityExtService.GetValues("Manufacturer");
            ViewBag.Categories = await _entityExtService.GetValues("Category", tableId);
            ViewBag.Suppliers = await _entityExtService.GetValues("Supplier");
            ViewBag.Cases = await _entityExtService.GetValues("Case", tableId);
            ViewBag.LibraryRefs = await _entityExtService.GetValues("Library Ref", tableId);
            ViewBag.FootprintRefs = await _entityExtService.GetValues("Footprint Ref", tableId);
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

        [HttpPost]
        [Route("/api/entities/{id:int}/add")]
        public async Task<IActionResult> IncreaseQuantity([FromRoute]int id, [FromBody]int quantity)
        {
            await _entityExtService.IncreaseQuantityAsync(id, quantity);
            return Ok();
        }
        
        [HttpPost]
        [Route("/api/entities/{id:int}/get")]
        public async Task<IActionResult> DecreaseQuantity([FromRoute]int id, [FromBody]int quantity)
        {
            await _entityExtService.DecreaseQuantityAsync(id, quantity);
            return Ok();
        }
        
        
    }
}
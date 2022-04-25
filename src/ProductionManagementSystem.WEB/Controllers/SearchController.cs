using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.Core.Services.AltiumDB;
using ProductionManagementSystem.WEB.Models.AltiumDB.GlobalSearch;
using ProductionManagementSystem.WEB.Models.Device;

namespace ProductionManagementSystem.WEB.Areas.AltiumDB.Controllers
{
    public class SearchController : Controller
    {
        private readonly IPcbService _pcbService;
        private readonly IEntityExtService _entityExtService;
        private readonly ITableService _tableService;
        private readonly ICompDbDeviceService _deviceService;
        
        public SearchController(IPcbService pcbService, IEntityExtService entityService, ITableService tableService, ICompDbDeviceService deviceService)
        {
            _pcbService = pcbService;
            _entityExtService = entityService;
            _tableService = tableService;
            _deviceService = deviceService;
        }

        [Route("/search")]
        public async Task<IActionResult> GlobalSearch(string q)
        {
            var hints = new List<GlobalSearchHintsSectionViewModel>();
            var entity = await _entityExtService.GetEntityExtByPartNumber(q);
            if (!string.IsNullOrWhiteSpace(entity?.PartNumber))
            {
                hints.Add(new GlobalSearchHintsSectionViewModel()
                {
                    Name = "Find by part number",
                    Rows = new List<GlobalSearchHintsSectionRowViewModel>()
                    {
                        new GlobalSearchHintsSectionRowViewModel()
                        {
                            Content = entity,
                            Type = HintsSectionRowType.BaseEntity
                        }
                    }
                });
            }
            
            hints.Add(new GlobalSearchHintsSectionViewModel()
            {
                Name = "Used in pcb",
                Rows = (await _pcbService.GetPcbWithEntityAsync(q)).Take(5).Select(x => 
                    new GlobalSearchHintsSectionRowViewModel() 
                    {
                     Type = HintsSectionRowType.Pcb,
                     Content = x
                }).ToList()
            });
            
            hints.Add(new GlobalSearchHintsSectionViewModel()
            {
                Name = "Entities",
                Rows = (await _entityExtService.SearchByKeyWordAsync(q)).Take(5).Select(x => 
                    new GlobalSearchHintsSectionRowViewModel() 
                    {
                        Type = HintsSectionRowType.BaseEntity,
                        Content = x
                    }).ToList()
            });
            
            hints.Add(new GlobalSearchHintsSectionViewModel()
            {
                Name = "PCB",
                Rows = (await _pcbService.SearchByKeyWordAsync(q)).Take(5).Select(x => 
                    new GlobalSearchHintsSectionRowViewModel() 
                    {
                        Type = HintsSectionRowType.Pcb,
                        Content = x
                    }).ToList()
            });
            
            hints.Add(new GlobalSearchHintsSectionViewModel()
            {
                Name = "Tables",
                Rows = (await _tableService.SearchByKeyWordAsync(q)).Take(5).Select(x => 
                    new GlobalSearchHintsSectionRowViewModel() 
                    {
                        Type = HintsSectionRowType.Table,
                        Content = x
                    }).ToList()
            });
            return PartialView("Partail/GlobalSearch/GlobalSearchHints", hints);
        }
        
        [Route("/api/search/create-device")]
        public async Task<List<CreateDeviceSearchViewModel>> CreateDeviceSearch(string q, int take)
        {
            var result = new List<CreateDeviceSearchViewModel>();
            result.AddRange((await _entityExtService.SearchByKeyWordAsync(q)).Select(x => new CreateDeviceSearchViewModel(x)));
            result.AddRange((await _pcbService.SearchByKeyWordAsync(q)).Select(x => new CreateDeviceSearchViewModel(x)));
            result.AddRange((await _deviceService.SearchByKeyWordAsync(q)).Select(x => new CreateDeviceSearchViewModel(x)));
            return result.Take(take).ToList();
        }
        
        [Route("/api/search/taskItem")]
        public async Task<List<UniversalItem>> TaskItemSearch(string q, int take=5)
        {
            var result = new List<UniversalItem>();
            result.AddRange((await _pcbService.SearchByKeyWordAsync(q)).Select(x => new UniversalItem(x)));
            result.AddRange((await _deviceService.SearchByKeyWordAsync(q)).Select(x => new UniversalItem(x)));
            return result.Take(take).ToList();
        }
    }
}
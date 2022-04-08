using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.Core.Services.AltiumDB;
using ProductionManagementSystem.WEB.Models.AltiumDB.GlobalSearch;

namespace ProductionManagementSystem.WEB.Areas.AltiumDB.Controllers
{
    public class SearchController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IEntityExtService _entityExtService;
        private readonly ITableService _tableService;
        
        public SearchController(IProjectService projectService, IEntityExtService entityService, ITableService tableService)
        {
            _projectService = projectService;
            _entityExtService = entityService;
            _tableService = tableService;
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
                Name = "Used in projects",
                Rows = (await _projectService.GetProjectsWithEntityAsync(q)).Take(5).Select(x => 
                    new GlobalSearchHintsSectionRowViewModel() 
                    {
                     Type = HintsSectionRowType.Project,
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
                Name = "Projects",
                Rows = (await _projectService.SearchByKeyWordAsync(q)).Take(5).Select(x => 
                    new GlobalSearchHintsSectionRowViewModel() 
                    {
                        Type = HintsSectionRowType.Project,
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
    }
}
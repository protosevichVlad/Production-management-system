using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using ProductionManagementSystem.Core.Models.AltiumDB.Projects;
using ProductionManagementSystem.Core.Services.AltiumDB;
using ProductionManagementSystem.WEB.Models;

namespace ProductionManagementSystem.WEB.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task<IActionResult> Index(string orderBy, string q)
        {
            List<Project> data = new List<Project>();
            if (string.IsNullOrEmpty(q))
            {
                data = await _projectService.GetAllAsync();
            }
            else
            {
                data = await _projectService
                    .Find(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(q, StringComparison.InvariantCultureIgnoreCase));
                data.AddRange(await _projectService.GetProjectsWithEntityAsync(q));
            }
            
            return View(data);
        }
        
        public async Task<IActionResult> Create()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(IFormFile bom, IFormFile image, IFormFile circuitDiagram, IFormFile assemblyDrawing, IFormFile treeDModel)
        {
            var project = await _projectService.ImportProjectAsync(bom.OpenReadStream(), image.OpenReadStream(),
                circuitDiagram.OpenReadStream(), assemblyDrawing.OpenReadStream(), treeDModel.OpenReadStream());
            await _projectService.CreateAsync(project);
            return RedirectToAction(nameof(Details), new {id = project.Id});
        }
        
        public async Task<IActionResult> Details(int id)
        {
            return View(await _projectService.GetByIdAsync(id));
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            
            return View(new ProjectCreateEditViewModel()
            {
                Id = project.Id,
                Name = project.Name,
                Variant = project.Variant,
                Description = project.Description,
                Quantity = project.Quantity,
            });
        }

        [HttpGet]
        [Route("api/projects/{id:int}")]
        public async Task<IActionResult> ApiGet(int id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            
            return Ok(project);
        }
        
        [HttpDelete]
        [Route("/Projects/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _projectService.DeleteByIdAsync(id);
            return Ok();
        }
        
        public async Task<IActionResult> PrintVersion(int id)
        {
            return View(await _projectService.GetByIdAsync(id));
        }
        
        [HttpPost]
        [Route("/api/projects/{id:int}/add")]
        public async Task<IActionResult> IncreaseQuantity([FromRoute]int id, [FromBody]int quantity)
        {
            await _projectService.IncreaseQuantityAsync(id, quantity);
            return Ok();
        }
        
        [HttpPost]
        [Route("/api/projects/{id:int}/get")]
        public async Task<IActionResult> DecreaseQuantity([FromRoute]int id, [FromBody]int quantity)
        {
            await _projectService.DecreaseQuantityAsync(id, quantity);
            return Ok();
        }
        
        [HttpPut]
        [Route("/api/projects/{id:int}")]
        public async Task<IActionResult> Update([FromRoute]int id, [FromBody]Project project)
        {
            project.Id = id;
            await _projectService.UpdateAsync(project);
            return Ok();
        }
    }
}
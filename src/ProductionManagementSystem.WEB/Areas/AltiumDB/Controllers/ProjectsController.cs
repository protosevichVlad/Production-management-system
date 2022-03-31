using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using ProductionManagementSystem.Core.Services.AltiumDB;

namespace ProductionManagementSystem.WEB.Areas.AltiumDB.Controllers
{
    [Area("AltiumDB")]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _projectService.GetAllAsync());
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
        
        [HttpDelete]
        [Route("/AltiumDB/Projects/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _projectService.DeleteByIdAsync(id);
            return Ok();
        }
    }
}
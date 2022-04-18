using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Models.PCB;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.WEB.Models;

namespace ProductionManagementSystem.WEB.Controllers
{
    public class PcbController : Controller
    {
        private readonly IPcbService _pcbService;
        private readonly IFileService _fileService;

        public PcbController(IPcbService pcbService, IFileService fileService)
        {
            _pcbService = pcbService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index(string orderBy, string q)
        {
            List<Pcb> data = new List<Pcb>();
            if (string.IsNullOrEmpty(q))
            {
                data = await _pcbService.GetAllAsync();
            }
            else
            {
                data = await _pcbService
                    .Find(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(q, StringComparison.InvariantCultureIgnoreCase));
                data.AddRange(await _pcbService.GetPcbWithEntityAsync(q));
            }
            
            return View(data);
        }
        
        public async Task<IActionResult> Create()
        {
            return View("Edit", new PcbCreateEditViewModel()
            {
                Id = 0,
            });
        }
        
        [HttpPost]
        [Route("/api/pcb")]
        public async Task<IActionResult> Create([FromForm]PcbCreateEditViewModel pcbViewModel)
        {
            var pcb = await MapToPcb(pcbViewModel);
            await _pcbService.CreateAsync(pcb);
            return Ok(pcb);
        }

        private async Task<Pcb> MapToPcb(PcbCreateEditViewModel pcbViewModel)
        {
            Pcb result = new Pcb()
            {
                Description = pcbViewModel.Description,
                Name = pcbViewModel.Name,
                Variant = pcbViewModel.Variant,
                ReportDate = pcbViewModel.ReportDate,
                Entities = pcbViewModel.Entities,
                Id = pcbViewModel.Id,
                Quantity = pcbViewModel.Quantity,
            };
            
            var path = Path.Combine("wwwroot", "uploads", pcbViewModel.Name, pcbViewModel.Variant);
            var url = Path.Combine("/uploads", pcbViewModel.Name, pcbViewModel.Variant)
                .Replace('\\', '/');

            if (pcbViewModel.ImageUploader != null)
            {
                await _fileService.UploadFile(pcbViewModel.ImageUploader.OpenReadStream(), path, pcbViewModel.ImageUploader.FileName);
                result.ImagePath = $"{url}/{pcbViewModel.ImageUploader.FileName}";
            }
            
            if (pcbViewModel.CircuitUploader != null)
            {
                await _fileService.UploadFile(pcbViewModel.CircuitUploader.OpenReadStream(), path, pcbViewModel.CircuitUploader.FileName);
                result.CircuitDiagramPath = $"{url}/{pcbViewModel.CircuitUploader.FileName}";
            }
            
            if (pcbViewModel.AssemblyUploader != null)
            {
                await _fileService.UploadFile(pcbViewModel.AssemblyUploader.OpenReadStream(), path, pcbViewModel.AssemblyUploader.FileName);
                result.AssemblyDrawingPath = $"{url}/{pcbViewModel.AssemblyUploader.FileName}";
            }
            
            if (pcbViewModel.TreeDUploader != null)
            {
                await _fileService.UploadFile(pcbViewModel.TreeDUploader.OpenReadStream(), path, pcbViewModel.TreeDUploader.FileName);
                result.ThreeDModelPath = $"{url}/{pcbViewModel.TreeDUploader.FileName}";
            }

            return result;
        }
        

        public async Task<IActionResult> Import()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile bom, IFormFile image, IFormFile circuitDiagram, IFormFile assemblyDrawing, IFormFile treeDModel)
        {
            var project = await _pcbService.ImportPcbAsync(bom.OpenReadStream(), image.OpenReadStream(),
                circuitDiagram.OpenReadStream(), assemblyDrawing.OpenReadStream(), treeDModel.OpenReadStream());
            await _pcbService.CreateAsync(project);
            return RedirectToAction(nameof(Details), new {id = project.Id});
        }
        
        public async Task<IActionResult> Details(int id)
        {
            return View(await _pcbService.GetByIdAsync(id));
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _pcbService.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            
            return View(new PcbCreateEditViewModel()
            {
                Id = project.Id,
                Name = project.Name,
                Variant = project.Variant,
                Description = project.Description,
                Quantity = project.Quantity,
            });
        }

        [HttpGet]
        [Route("api/pcb/{id:int}")]
        public async Task<IActionResult> ApiGet(int id)
        {
            var project = await _pcbService.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            
            return Ok(project);
        }
        
        [HttpDelete]
        [Route("/pcb/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _pcbService.DeleteByIdAsync(id);
            return Ok();
        }
        
        public async Task<IActionResult> PrintVersion(int id)
        {
            return View(await _pcbService.GetByIdAsync(id));
        }
        
        [HttpPost]
        [Route("/api/pcb/{id:int}/add")]
        public async Task<IActionResult> IncreaseQuantity([FromRoute]int id, [FromBody]int quantity)
        {
            await _pcbService.IncreaseQuantityAsync(id, quantity);
            return Ok();
        }
        
        [HttpPost]
        [Route("/api/pcb/{id:int}/get")]
        public async Task<IActionResult> DecreaseQuantity([FromRoute]int id, [FromBody]int quantity)
        {
            await _pcbService.DecreaseQuantityAsync(id, quantity);
            return Ok();
        }
        
        [HttpPut]
        [Route("/api/pcb/{id:int}")]
        public async Task<IActionResult> Update([FromRoute]int id, [FromForm]PcbCreateEditViewModel pcbViewModel)
        {
            var pcb = await MapToPcb(pcbViewModel);
            pcb.Id = id;
            await _pcbService.UpdateAsync(pcb);
            return Ok(pcb);
        }
    }
}
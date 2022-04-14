using System;
using System.Collections.Generic;
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

        public PcbController(IPcbService pcbService)
        {
            _pcbService = pcbService;
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
        public async Task<IActionResult> Create([FromBody]Pcb pcb)
        {
            await _pcbService.CreateAsync(pcb);
            return Ok(pcb);
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
        public async Task<IActionResult> Update([FromRoute]int id, [FromBody]Pcb pcb)
        {
            pcb.Id = id;
            await _pcbService.UpdateAsync(pcb);
            return Ok();
        }
    }
}
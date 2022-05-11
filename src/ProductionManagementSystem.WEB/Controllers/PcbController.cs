using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Models.PCB;
using ProductionManagementSystem.Core.Models.Users;
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

            if (!string.IsNullOrEmpty(orderBy))
            {
                if (orderBy.EndsWith("_desc"))
                {
                    data = data.OrderByDescending((x) => typeof(Pcb).GetProperty(orderBy.Substring(0, orderBy.Length - "_desc".Length))?.GetValue(x)).ToList();
                }
                else
                {
                    data = data.OrderBy((x) => typeof(Pcb).GetProperty(orderBy)?.GetValue(x)).ToList();
                }
                
            }
            
            return View(data);
        }
        
        [Authorize(Roles = RoleEnum.Admin)]
        public async Task<IActionResult> Create()
        {
            return View("Edit", new PcbCreateEditViewModel()
            {
                Id = 0,
            });
        }
        
        [HttpPost]
        [Authorize(Roles = RoleEnum.Admin)]
        [Route("/api/pcb")]
        public async Task<IActionResult> CreateApi([FromForm]PcbCreateEditViewModel pcbViewModel)
        {
            var pcb = await MapToCreateEditPcb(pcbViewModel);
            await _pcbService.CreateAsync(pcb);
            return Ok(pcb);
        }

        private async Task<CreateEditPcb> MapToCreateEditPcb(PcbCreateEditViewModel pcbViewModel)
        {
            CreateEditPcb result = new CreateEditPcb()
            {
                Description = pcbViewModel.Description,
                Name = pcbViewModel.Name,
                Variant = pcbViewModel.Variant,
                ReportDate = pcbViewModel.ReportDate,
                UsedItems = pcbViewModel.UsedItems,
                Id = pcbViewModel.Id,
                Quantity = pcbViewModel.Quantity,
            };
            
            if (pcbViewModel.ImageUploader != null)
            {
                await using var ms = new MemoryStream();
                await pcbViewModel.ImageUploader.CopyToAsync(ms);
                result.Image = ms.ToArray();
            }
            
            if (pcbViewModel.CircuitUploader != null)
            {
                await using var ms = new MemoryStream();
                await pcbViewModel.CircuitUploader.CopyToAsync(ms);
                result.CircuitDiagram = ms.ToArray();
            }
            
            if (pcbViewModel.TreeDUploader != null)
            {
                await using var ms = new MemoryStream();
                await pcbViewModel.TreeDUploader.CopyToAsync(ms);
                result.ThreeDModel = ms.ToArray();
            }
            
            if (pcbViewModel.AssemblyUploader != null)
            {
                await using var ms = new MemoryStream();
                await pcbViewModel.AssemblyUploader.CopyToAsync(ms);
                result.AssemblyDrawing = ms.ToArray();  
            }
            
            return result;
        }
        
        [Authorize(Roles = RoleEnum.Admin)]
        public async Task<IActionResult> Import()
        {
            return View();
        }
        
        [HttpPost]
        [Authorize(Roles = RoleEnum.Admin)]
        public async Task<IActionResult> Import(IFormFile bom, IFormFile image, IFormFile circuitDiagram, IFormFile assemblyDrawing, IFormFile treeDModel)
        {
            var project = await _pcbService.ImportPcbAsync(bom?.OpenReadStream(), image?.OpenReadStream(),
                circuitDiagram?.OpenReadStream(), assemblyDrawing?.OpenReadStream(), treeDModel?.OpenReadStream());
            await _pcbService.CreateAsync(project);
            return RedirectToAction(nameof(Details), new {id = project.Id});
        }
        
        public async Task<IActionResult> Details(int id)
        {
            var pcb = await _pcbService.GetByIdAsync(id);
            if (pcb == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            return View(pcb);
        }
        
        [Authorize(Roles = RoleEnum.Admin)]
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
        [Authorize(Roles = RoleEnum.Admin)]
        [Route("/pcb/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _pcbService.DeleteByIdAsync(id);
            return Ok();
        }
        
        public async Task<IActionResult> PrintVersion(int id)
        {
            var pcb = await _pcbService.GetByIdAsync(id);
            if (pcb == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            return View(pcb);
        }
        
        [HttpPost]
        [Authorize(Roles = RoleEnum.OrderPicker)]
        [Route("/api/pcb/{id:int}/add")]
        public async Task<IActionResult> IncreaseQuantity([FromRoute]int id, [FromBody]int quantity)
        {
            await _pcbService.IncreaseQuantityAsync(id, quantity);
            return Ok();
        }
        
        [HttpPost]
        [Authorize(Roles = RoleEnum.OrderPicker)]
        [Route("/api/pcb/{id:int}/get")]
        public async Task<IActionResult> DecreaseQuantity([FromRoute]int id, [FromBody]int quantity)
        {
            await _pcbService.DecreaseQuantityAsync(id, quantity);
            return Ok();
        }
        
        [HttpPut]
        [Authorize(Roles = RoleEnum.Admin)]
        [Route("/api/pcb/{id:int}")]
        public async Task<IActionResult> Update([FromRoute]int id, [FromForm]PcbCreateEditViewModel pcbViewModel)
        {
            var pcb = await MapToCreateEditPcb(pcbViewModel);
            pcb.Id = id;
            await _pcbService.UpdateAsync(pcb);
            return Ok(pcb);
        }
    }
}
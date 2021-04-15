using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.BLL.Services;
using ProductionManagementSystem.Models;
using ProductionManagementSystem.WEB.Models;

namespace ProductionManagementSystem.Controllers
{
    [Authorize(Roles = RoleEnum.OrderPicker)]
    public class DesignsController : Controller
    {
        private readonly IDesignService _designService;
        private IMapper _mapperToViewModel;
        private IMapper _mapperFromViewModel;
        
        
        public DesignsController(IDesignService service)
        {
            _designService = service;
            _mapperToViewModel = new MapperConfiguration(cfg => cfg.CreateMap<DesignDTO, DesignViewModel>())
                .CreateMapper();
            _mapperFromViewModel = new MapperConfiguration(cfg => cfg.CreateMap<DesignViewModel, DesignDTO>())
                .CreateMapper();
        }

        // GET: Designs
        [HttpGet]
        public IActionResult Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewData["CurrentFilter"] = searchString;
            var designs = _designService.GetDesigns();

            if (!String.IsNullOrEmpty(searchString))
            {
                designs = designs.Where(d => (d.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                                             || (d.Type?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                                             || (d.ShortDescription?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false));
            }
            
            switch (sortOrder)
            {
                case "name_desc":
                    designs = designs.OrderByDescending(d => d.Name);
                    break;
                case "Type":
                    designs = designs.OrderBy(d => d.Type);
                    break;
                case "type_desc":
                    designs = designs.OrderByDescending(d => d.Type);
                    break;
                case "Quantity":
                    designs = designs.OrderBy(d => d.Quantity);
                    break;
                case "quantity_desc":
                    designs = designs.OrderByDescending(d => d.Quantity);
                    break;
                default:
                    designs = designs.OrderBy(d => d.Name);
                    break;
            }

            var designsViewModule = _mapperToViewModel.Map<IEnumerable<DesignDTO>, IEnumerable<DesignViewModel>>(designs);
            return View(designsViewModule);
        }

        // GET: Designs/Details/5
        public IActionResult Details(int? id)
        {
            try
            {
                var design = _designService.GetDesign(id);
                var designViewModel = _mapperToViewModel.Map<DesignDTO, DesignViewModel>(design);
                return View(designViewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // GET: Designs/Create
        public IActionResult Create()
        {
            ViewBag.AllTypes = _designService.GetTypes();
            return View();
        }
        
        // POST: Designs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Type,Name,Quantity,ShortDescription,Description")] DesignViewModel designViewModel)
        {
            if (ModelState.IsValid)
            {
                var design = _mapperFromViewModel.Map<DesignViewModel, DesignDTO>(designViewModel);
                LogService.UserName = User.Identity?.Name;
                _designService.CreateDesign(design);
                return RedirectToAction(nameof(Index));
            }
            return View(designViewModel);
        }

        // GET: Designs/Edit/5
        public IActionResult Edit(int? id)
        {
            try
            {
                var design = _designService.GetDesign(id);
                var designViewModel = _mapperToViewModel.Map<DesignDTO, DesignViewModel>(design);
                
                ViewBag.AllTypes = _designService.GetTypes();
                return View(designViewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // POST: Designs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public IActionResult Edit(int id, [Bind("Id,Type,Name,Quantity,ShortDescription,Description")] DesignViewModel designViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var design = _mapperFromViewModel.Map<DesignViewModel, DesignDTO>(designViewModel);
                    LogService.UserName = User.Identity?.Name;
                    _designService.UpdateDesign(design);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            return View(designViewModel);
        }

        // GET: Designs/Delete/5
        public IActionResult Delete(int? id)
        {
            try
            {
                var design = _designService.GetDesign(id);
                var designViewModel = _mapperToViewModel.Map<DesignDTO, DesignViewModel>(design);
                return View(designViewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // POST: Designs/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                LogService.UserName = User.Identity?.Name;
                _designService.DeleteDesign(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntersectionOfEntitiesException e)
            {
                ViewBag.ErrorMessage = e.Message;
                ViewBag.ErrorHeader = e.Header;
                return View(
                    _mapperToViewModel.Map<DesignDTO, DesignViewModel>(_designService.GetDesign(id)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet]
        public JsonResult GetAllDesigns()
        {
            IEnumerable<DesignDTO> designs = _designService.GetDesigns();
                
            foreach (var des in designs)
            {
                des.Name = des.ToString();
            }
            return Json(designs);
        }
        
        [HttpGet]
        public IActionResult Add(int? id)
        {
            var design = _designService.GetDesign(id);
            var designViewModel = _mapperToViewModel.Map<DesignDTO, DesignViewModel>(design);
            return View(designViewModel);
        }
        
        [HttpPost]
        public IActionResult Add(int designId, int quantity)
        {

            try
            {
                LogService.UserName = User.Identity?.Name;
                _designService.AddDesign(designId, quantity);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        [HttpGet]
        public IActionResult Receive(int id)
        {

            var design = _designService.GetDesign(id);
            var designViewModel = _mapperToViewModel.Map<DesignDTO, DesignViewModel>(design);
            return View(designViewModel);
        }
        [HttpPost]
        public IActionResult Receive(int designId, int quantity)
        {

            try
            {
                _designService.AddDesign(designId, -quantity);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

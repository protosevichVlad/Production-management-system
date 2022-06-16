using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Repositories;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.WEB.Models;
using ProductionManagementSystem.WEB.Models.AltiumDB;

namespace ProductionManagementSystem.WEB.Controllers
{
    public class CDBSupplyRequestsController : Controller
    {
        private readonly ICDBSupplyRequestService _supplyRequestService;

        public CDBSupplyRequestsController(ICDBSupplyRequestService supplyRequestService)
        {
            _supplyRequestService = supplyRequestService;
        }

        public async Task<IActionResult> Index(string orderBy, Dictionary<string, List<string>> filter, 
            string q, int? itemPerPage, int? page)
        {
            var data = await _supplyRequestService.GetAllAsync();
            var total = data.Count;
            page ??= 1;
            itemPerPage ??= 20;
            
            return View(new DataListViewModel<CDBSupplyRequest>()
            {
                Data = data.Skip((page.Value - 1) * itemPerPage.Value).Take(itemPerPage.Value).ToList(),
                Filters = null,
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = page.Value,
                    ItemPerPage = itemPerPage.Value,
                    TotalItems = total
                }
            });
        }

        public IActionResult Details(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
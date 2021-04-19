using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.WEB.Models;

namespace ProductionManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private IDatabaseService _databaseService;
        
        public HomeController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public string Reset()
        {
            _databaseService.ResetDatabase();
            return "complite";
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error; 
            return View(new ErrorViewModel { Message = exception?.Message});
        }
    }
}

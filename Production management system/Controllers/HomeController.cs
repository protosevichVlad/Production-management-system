using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.BLL.Services;
using ProductionManagementSystem.Models;

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
    }
}

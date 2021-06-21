﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.WEB.Models;

namespace ProductionManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDatabaseService _databaseService;
        private readonly UserManager<ProductionManagementSystemUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public HomeController(IDatabaseService databaseService, UserManager<ProductionManagementSystemUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _databaseService = databaseService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
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

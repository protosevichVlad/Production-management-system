﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
        private UserManager<ProductionManagementSystemUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        
        public HomeController(UserManager<ProductionManagementSystemUser> userManager, RoleManager<IdentityRole> roleManager)
        {
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public string Reset()
        {
            var db = new ApplicationContext();
            db.ResetDatabase();

            string adminEmail = "admin";
            string adminPassword = "123456";

            // добавляем роли
            Role adminRole = new Role { Id = 1, Name = "admin", RusName = "Администратор" };
            Role orderPickerRole = new Role { Id = 2, Name = "order_picker", RusName = "Комплектовщик" };
            Role assemblerRole = new Role { Id = 3, Name = "assembler", RusName = "Монтажник" };
            Role tunerRole = new Role { Id = 4, Name = "tuner", RusName = "Настройщик" };
            Role collectorRole = new Role { Id = 5, Name = "collector", RusName = "Сборщик" };
            Role validatingRole = new Role { Id = 6, Name = "validating", RusName = "Проверяющий" };
            Role shipperRole = new Role { Id = 7, Name = "shipper", RusName = "Грузоотправитель" };

            User adminUser = new User { Login = adminEmail, Password = adminPassword, RoleId = adminRole.Id };

            db.Roles.Add(adminRole);
            db.Roles.Add(orderPickerRole);
            db.Roles.Add(assemblerRole);
            db.Roles.Add(tunerRole);
            db.Roles.Add(collectorRole);
            db.Roles.Add(validatingRole);
            db.Roles.Add(shipperRole);

            db.Users.Add(adminUser);
            db.SaveChanges();

            return "ok";
        }
    }
}

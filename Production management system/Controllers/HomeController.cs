using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext _context;

        public HomeController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public string Reset()
        {
            try
            {
                if (_context.Users.FirstOrDefault(u => u.Login == "admin") != null)
                {
                    return "ok";
                }
            }
            catch (Exception e)
            {
                _context.ResetDatabase();
            }
            
            
            string adminEmail = "admin";
            string adminPassword = "123456";

            // добавляем роли
            Role adminRole = new Role { Name = RoleEnum.Admin, RusName = "Администратор" };
            Role orderPickerRole = new Role { Name = RoleEnum.OrderPicker, RusName = "Комплектовщик" };
            Role assemblerRole = new Role { Name = RoleEnum.Assembler, RusName = "Монтажник" };
            Role tunerRole = new Role { Name = RoleEnum.Tuner, RusName = "Настройщик" };
            Role collectorRole = new Role { Name = RoleEnum.Collector, RusName = "Сборщик" };
            Role validatingRole = new Role { Name = RoleEnum.Validating, RusName = "Проверяющий" };
            Role shipperRole = new Role { Name = RoleEnum.Shipper, RusName = "Грузоотправитель" };

            _context.Roles.Add(adminRole);
            _context.Roles.Add(orderPickerRole);
            _context.Roles.Add(assemblerRole);
            _context.Roles.Add(tunerRole);
            _context.Roles.Add(collectorRole);
            _context.Roles.Add(validatingRole);
            _context.Roles.Add(shipperRole);
            _context.SaveChanges();
            
            User adminUser = new User { Login = adminEmail, Password = adminPassword, RoleId = adminRole.Id };

            

            _context.Users.Add(adminUser);
            _context.SaveChanges();

            return "complite";
        }
    }
}

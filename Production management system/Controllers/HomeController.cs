using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext _context;

        public HomeController()
        {
            _context = new ApplicationContext();
        }

        public IActionResult Index()
        {
            return View();
        }

        public string Reset()
        {
            _context.ResetDatabase();

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

            _context.Roles.Add(adminRole);
            _context.Roles.Add(orderPickerRole);
            _context.Roles.Add(assemblerRole);
            _context.Roles.Add(tunerRole);
            _context.Roles.Add(collectorRole);
            _context.Roles.Add(validatingRole);
            _context.Roles.Add(shipperRole);

            _context.Users.Add(adminUser);
            _context.SaveChanges();

            return "ok";
        }
    }
}

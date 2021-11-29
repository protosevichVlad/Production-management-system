using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Models.Users;

namespace ProductionManagementSystem.WEB.Controllers
{
    [Authorize(Roles=RoleEnum.Admin)]
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MontageMonthReport()
        {
            return View();
        }
    }
}
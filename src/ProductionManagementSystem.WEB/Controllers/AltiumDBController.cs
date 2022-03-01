using Microsoft.AspNetCore.Mvc;

namespace ProductionManagementSystem.WEB.Controllers
{
    public class AltiumDBController : Controller 
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
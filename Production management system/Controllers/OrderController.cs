using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace ProductionManagementSystem.Controllers
{

    public class OrderController : Controller
    {
        [Authorize(Roles = "admin")]
        public IActionResult Show()
        {
            var db = new ApplicationContext();
            ViewBag.Orders = db.Orders.Include(o => o.Device);
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Add()
        {
            var db = new ApplicationContext();
            ViewBag.Devices = db.Devices;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Add(IFormCollection collection)
        {
            var db = new ApplicationContext();
            Order order = new Order();
            order.StartTime = DateTime.Now;
            order.EndTime = new DateTime();
            order.Status = "Комплектация";
            order.QuantityDevice = int.Parse(collection["Quantity"]);
            int.TryParse(collection["NameDivice"], out int indexDevice);
            order.Device = db.Devices.Where(d => d.Id == indexDevice).FirstOrDefault();
            db.Orders.Add(order);
            db.SaveChanges();
            int idOrders = order.Id;
            return Redirect($"/Order/ShowOrder/{idOrders}");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult ShowOrder(int id)
        {
            var db = new ApplicationContext();
            ViewBag.Order = db.Orders
                .Include(o => o.Device)
                .Where(o => o.Id == id).FirstOrDefault();

            int idDevice = ViewBag.Order.Device.Id;
            ViewBag.Device = db.Devices
                .Include(d => d.DeviceComponentsTemplate)
                .ThenInclude(d => d.Component)
                .Include(d => d.DeviceDesignTemplate)
                .ThenInclude(d => d.Design)
                .Where(d => d.Id == idDevice).FirstOrDefault();
            return View();
        }
    }
}

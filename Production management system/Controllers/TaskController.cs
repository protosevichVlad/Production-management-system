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

    public class TaskController : Controller
    {
        [Authorize(Roles = "admin")]
        public IActionResult Show()
        {
            var db = new ApplicationContext();
            return View(db.Tasks);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Add()
        {
            var db = new ApplicationContext();
            ViewBag.Devices = db.Devices;
            ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Add(IFormCollection collection)
        {
            var db = new ApplicationContext();
            Models.Task task = new Models.Task();
            task.Customer = collection["Customer"];
            task.StartTime = DateTime.Now;
            task.Deadline = DateTime.Parse(collection["Deadline"]);
            task.EndTime = new DateTime();
            task.Status = "Комплектация";

            List<DeviceInTask> devicesInTask = new List<DeviceInTask>();

            int idDevice = 0;
            foreach(var key in collection.Keys)
            {
                if (key.Contains("NameDivice"))
                {
                    int.TryParse(collection[key], out idDevice);
                } else if (key.Contains("Quantity"))
                {
                    int.TryParse(collection[key], out int quantity);
                    devicesInTask.Add(new DeviceInTask
                    {
                        Device = db.Devices.Where(d => d.Id == idDevice).FirstOrDefault(),
                        Quantity = quantity,
                    });
                }
            }
            task.DevicesInTask = devicesInTask;
            db.Tasks.Add(task);
            db.SaveChanges();
            int idTasks = task.Id;
            return Redirect($"/Task/ShowTask/{idTasks}");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult ShowTask(int id)
        {
            var db = new ApplicationContext();
            ViewBag.Task = db.Tasks
                .Include(t => t.DevicesInTask)
                .ThenInclude(d => d.Device)
                .Where(t => t.Id == id).FirstOrDefault();

            List<int> idsDevices = new List<int>();
            List<int> quantytiDevices = new List<int>();
            foreach (var d in ViewBag.Task.DevicesInTask)
            {
                idsDevices.Add(d.Device.Id);
                quantytiDevices.Add(d.Quantity);
            }

            List<Device> devices = new List<Device>();
            foreach(var idDevice in idsDevices)
            {
                devices.Add(db.Devices
                    .Include(d => d.DeviceComponentsTemplate)
                    .ThenInclude(d => d.Component)
                    .Include(d => d.DeviceDesignTemplate)
                    .ThenInclude(d => d.Design)
                    .Where(d => d.Id == idDevice).FirstOrDefault());
            }
            ViewBag.QuantytiDevices = quantytiDevices;
            ViewBag.Devices = devices;
            return View();
        }
    }
}

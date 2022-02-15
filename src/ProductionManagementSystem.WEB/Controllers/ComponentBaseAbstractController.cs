using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductionManagementSystem.Core.Models.Components;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.WEB.Models;

namespace ProductionManagementSystem.WEB.Controllers
{
    public class ComponentBaseAbstractController<TEntity> : Controller 
        where TEntity : ComponentBase
    {
        protected readonly IDeviceService _deviceService;
        protected readonly IComponentBaseService<TEntity> _componentBaseService;

        public ComponentBaseAbstractController(IComponentBaseService<TEntity> componentBaseService, IDeviceService deviceService)
        {
            _componentBaseService = componentBaseService;
            _deviceService = deviceService;
        }

        protected async Task<ComponentsForDevice> GetMultipleComponents(int? deviceId, string typeName)
        {
            var selectListDevice = new SelectList(await _deviceService.GetAllAsync(), "Id", "Name");
            var selectListTypes = new SelectList(await _componentBaseService.GetTypesAsync());

            var components = new ComponentsForDevice();
            List<TEntity> componentsInDevice = new List<TEntity>();
            if (deviceId != null)
            {
                var device = selectListDevice.FirstOrDefault(l => l.Value == deviceId.ToString());
                if (device != null)
                    device.Selected = true;
                
                componentsInDevice = await _componentBaseService.GetByDeviceId(deviceId.Value);
            }
            else
            {
                componentsInDevice = await _componentBaseService.GetAllAsync();
            }

            if (componentsInDevice == null)
            {
                throw new NotImplementedException();
            }

            if (typeName != null)
            {
                var type = selectListTypes.FirstOrDefault(l => l.Text == typeName);
                if (type != null)
                {
                    type.Selected = true;
                }
                
                componentsInDevice = componentsInDevice.Where(c => c.Type == typeName).ToList();
            }

            var length = componentsInDevice.Count;
            components.ComponentId = new int[length];
            components.ComponentName = new string[length];
            components.Quantity = new int[length];
            components.QuantityInStock = new int[length];
            for (var index = 0; index < length; index++)
            {
                var componentInDevice = componentsInDevice[index];
                components.ComponentName[index] = componentInDevice.ToString();
                components.ComponentId[index] = componentInDevice.Id;
                components.QuantityInStock[index] = componentInDevice.Quantity;
            }

            ViewBag.TypeNames = selectListTypes;
            ViewBag.Devices = selectListDevice;
            return components;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.API.Models;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Interfaces;

namespace ProductionManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : Controller
    {
        private readonly IDeviceService _deviceService;
        private IMapper _mapper;
        
        
        public DevicesController(IDeviceService service)
        {
            _deviceService = service;
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DeviceDTO, Device>()
                        .ForMember(
                            device => device.DeviceComponentsTemplate,
                            otp => otp.MapFrom(src =>
                                src.ComponentIds.Select((value, index) => new DeviceComponentsTemplate()
                                {
                                    ComponentId = src.ComponentIds[index],
                                    Description = src.ComponentDescriptions[index],
                                    Quantity = src.ComponentQuantity[index],
                                    Id = src.ComponentTemplateId[index],
                                })))
                        .ForMember(
                            device => device.DeviceDesignTemplate,
                            otp => otp.MapFrom(src =>
                                src.DesignIds.Select((value, index) => new DeviceDesignTemplate()
                                {
                                    DesignId = src.DesignIds[index],
                                    Description = src.DesignDescriptions[index],
                                    Quantity = src.DesignQuantity[index],
                                    Id = src.DesignTemplateId[index],
                                })));
                    
                    cfg.CreateMap<Device, DeviceDTO>()
                        .ForMember(
                            device => device.ComponentIds,
                            otp => otp.MapFrom(src =>
                                src.DeviceComponentsTemplate.Select(componentTemplate => componentTemplate.ComponentId)))
                        .ForMember(
                            device => device.DesignIds,
                            otp => otp.MapFrom(src =>
                                src.DeviceDesignTemplate.Select(designTemplate => designTemplate.DesignId)))
                        .ForMember(
                            device => device.ComponentDescriptions,
                            otp => otp.MapFrom(src =>
                                src.DeviceComponentsTemplate.Select(componentTemplate => componentTemplate.Description)))
                        .ForMember(
                            device => device.DesignDescriptions,
                            otp => otp.MapFrom(src =>
                                src.DeviceDesignTemplate.Select(designTemplate => designTemplate.Description)))
                        .ForMember(
                            device => device.ComponentTemplateId,
                            otp => otp.MapFrom(src =>
                                src.DeviceComponentsTemplate.Select(componentTemplate => componentTemplate.Id)))
                        .ForMember(
                            device => device.DesignTemplateId,
                            otp => otp.MapFrom(src =>
                                src.DeviceDesignTemplate.Select(designTemplate => designTemplate.Id)))
                        .ForMember(
                            device => device.ComponentQuantity,
                            otp => otp.MapFrom(src =>
                                src.DeviceComponentsTemplate.Select(componentTemplate => componentTemplate.Quantity)))
                        .ForMember(
                            device => device.DesignQuantity,
                            otp => otp.MapFrom(src =>
                                src.DeviceDesignTemplate.Select(designTemplate => designTemplate.Quantity)));
                })
                .CreateMapper();
        }

        [HttpGet]
        public async Task<IEnumerable<Device>> Get()
        {
            return _mapper.Map<IEnumerable<DeviceDTO>, IEnumerable<Device>>(await _deviceService.GetDevicesAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Device>> Get(int id)
        {
            return _mapper.Map<DeviceDTO, Device>(await _deviceService.GetDeviceAsync(id));
        }
        
        // POST api/designs
        [HttpPost]
        public async Task<ActionResult<Device>> Post(Device device)
        {
            if (device == null)
            {
                return BadRequest();
            }
 
            await _deviceService.CreateDeviceAsync(_mapper.Map<Device, DeviceDTO>(device));
            return Ok(device);
        }
        
        // PUT api/designs/
        [HttpPut]
        public async Task<ActionResult<Device>> Put(Device device)
        {
            if (device == null)
            {
                return BadRequest();
            }
            
            await _deviceService.UpdateDeviceAsync(_mapper.Map<Device, DeviceDTO>(device));
            return Ok(device);
        }
        
        // DELETE api/designs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Device>> Delete(int id)
        {
            var device = _mapper.Map<DeviceDTO, Device>(await _deviceService.GetDeviceAsync(id));
            await _deviceService.DeleteDeviceAsync(device.Id);
            return Ok(device);
        }
    }
}
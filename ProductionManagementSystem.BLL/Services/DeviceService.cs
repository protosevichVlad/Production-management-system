using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.BLL.Services
{
    public class DeviceService : IDeviceService 
    {
        private IUnitOfWork _database { get; }
        private IMapper _mapperToDto;
        private IMapper _mapperFromDto;
        
        public DeviceService(IUnitOfWork uow)
        {
            _database = uow;
            _mapperToDto = new MapperConfiguration(cfg => cfg.CreateMap<Device, DeviceDTO>())
                .CreateMapper();
            _mapperFromDto = new MapperConfiguration(cfg => cfg.CreateMap<DeviceDTO, Device>())
                .CreateMapper();
        }
        
        public void CreateDevice(DeviceDTO deviceDto)
        {
            var device = _mapperFromDto.Map<DeviceDTO, Device>(deviceDto);
            device.DeviceDesignTemplate = GetDesignsFromDeviceDto(deviceDto);
            device.DeviceComponentsTemplate = GetComponentsFromDeviceDto(deviceDto);
            
            _database.Devices.Create(device);
            _database.Save();
        }

        public void UpdateDevice(DeviceDTO deviceDto)
        {
            var device = _mapperFromDto.Map<DeviceDTO, Device>(deviceDto);
            device.DeviceDesignTemplate = GetDesignsFromDeviceDto(deviceDto);
            device.DeviceComponentsTemplate = GetComponentsFromDeviceDto(deviceDto);
            
            _database.Devices.Update(device);
            _database.Save();
        }

        public IEnumerable<DeviceDTO> GetDevices()
        {
            var devices = _database.Devices.GetAll();
            if (devices == null)
            {
                return new List<DeviceDTO>(0);
            }

            return _mapperToDto.Map<IEnumerable<Device>, IEnumerable<DeviceDTO>>(devices);
        }

        public DeviceDTO GetDevice(int? id)
        {
            if (id == null)
            {
                throw new NotImplementedException();
            }
            
            var device = _database.Devices.Get((int) id);
            if (device == null)
            {
                throw new NotImplementedException();   
            }

            var deviceDto = _mapperToDto.Map<Device, DeviceDTO>(device);
            SetNameQuantityAndDescriptionForComponent(deviceDto, device);
            SetNameQuantityAndDescriptionForDesign(deviceDto, device);
            return deviceDto;
        }

        public void DeleteDevice(int? id)
        {
            if (id == null)
            {
                throw new NotImplementedException();
            }
            
            _database.Devices.Delete((int) id);
            _database.Save();
        }

        public IEnumerable<string> GetNames()
        {
            var devices = _database.Devices.GetAll();
            if (devices == null)
            {
                return Array.Empty<string>();
            }

            return devices.Select(d => d.ToString());
        }

        public void Dispose()
        {
            _database.Dispose();
        }

        private List<DeviceComponentsTemplate> GetComponentsFromDeviceDto(DeviceDTO deviceDto)
        {
            var result = new List<DeviceComponentsTemplate>();
            for (int i = 0; i < deviceDto.ComponentIds.Length; i++)
            {
                result.Add(new DeviceComponentsTemplate
                {
                    ComponentId = deviceDto.ComponentIds[i],
                    DeviceId = deviceDto.Id,
                    Quantity = deviceDto.ComponentQuantity[i],
                    Description = deviceDto.ComponentDescriptions[i],
                });
            }

            return result;
        }
        
        private List<DeviceDesignTemplate> GetDesignsFromDeviceDto(DeviceDTO deviceDto)
        {
            var result = new List<DeviceDesignTemplate>();
            for (int i = 0; i < deviceDto.DesignIds.Length; i++)
            {
                result.Add(new DeviceDesignTemplate
                {
                    DesignId = deviceDto.DesignIds[i],
                    DeviceId = deviceDto.Id,
                    Quantity = deviceDto.DesignQuantity[i],
                    Description = deviceDto.DesignDescriptions[i],
                });
            }

            return result;
        }

        private void SetNameQuantityAndDescriptionForComponent(DeviceDTO deviceDto, Device device)
        {
            var componentService = new ComponentService(_database);
            var ids = new List<int>();
            var names = new List<string>();
            var quantity = new List<int>();
            var descriptions = new List<string>();
            for (int i = 0; i < device.DeviceComponentsTemplate.Count; i++)
            {
                ids.Add(device.DeviceComponentsTemplate[i].ComponentId);
                names.Add(componentService.GetComponent(ids[^1]).ToString());
                quantity.Add(device.DeviceComponentsTemplate[i].Quantity);
                descriptions.Add(device.DeviceComponentsTemplate[i].Description);
            }

            deviceDto.ComponentIds = ids.ToArray();
            deviceDto.ComponentNames = names.ToArray();
            deviceDto.ComponentQuantity = quantity.ToArray();
            deviceDto.ComponentDescriptions = descriptions.ToArray();
        }
        
        private void SetNameQuantityAndDescriptionForDesign(DeviceDTO deviceDto, Device device)
        {
            var designService = new DesignService(_database);
            var ids = new List<int>();
            var names = new List<string>();
            var quantity = new List<int>();
            var descriptions = new List<string>();
            for (int i = 0; i < device.DeviceDesignTemplate.Count; i++)
            {
                ids.Add(device.DeviceDesignTemplate[i].DesignId);
                names.Add(designService.GetDesign(ids[^1]).ToString());
                quantity.Add(device.DeviceDesignTemplate[i].Quantity);
                descriptions.Add(device.DeviceDesignTemplate[i].Description);
            }

            deviceDto.DesignIds = ids.ToArray();
            deviceDto.DesignNames = names.ToArray();
            deviceDto.DesignQuantity = quantity.ToArray();
            deviceDto.DesignDescriptions = descriptions.ToArray();
        }
    }
}
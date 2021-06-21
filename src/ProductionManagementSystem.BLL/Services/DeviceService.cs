using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.BLL.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IUnitOfWork _database;
        private readonly IMapper _mapperToDto;
        private readonly IMapper _mapperFromDto;
        private readonly ILogService _log;
        
        public DeviceService(IUnitOfWork uow)
        {
            _database = uow;
            _log = new LogService(uow);
            _mapperToDto = new MapperConfiguration(cfg => cfg.CreateMap<Device, DeviceDTO>())
                .CreateMapper();
            _mapperFromDto = new MapperConfiguration(cfg => cfg.CreateMap<DeviceDTO, Device>())
                .CreateMapper();
        }
        
        public async Task CreateDeviceAsync(DeviceDTO deviceDto)
        {
            var device = _mapperFromDto.Map<DeviceDTO, Device>(deviceDto);
            device.DeviceDesignTemplate = GetDesignsFromDeviceDto(deviceDto);
            device.DeviceComponentsTemplate = GetComponentsFromDeviceDto(deviceDto);
            
            await _database.Devices.CreateAsync(device);
            await _database.SaveAsync();
            
            await _log.CreateLogAsync(new LogDTO($"Был создан новый прибор: {device}"){DeviceId = device.Id});
        }

        public async Task UpdateDeviceAsync(DeviceDTO deviceDto)
        { 
            var deviceFromDb = await _database.Devices.GetAsync(deviceDto.Id) ?? throw new NotImplementedException();
            var checkInTask = (await CheckInTaskAsync(deviceFromDb));
            string errorMessage = checkInTask.Item2;
            if (!checkInTask.Item1)
            {
                throw new IntersectionOfEntitiesException("Ошибка. Невозможно изменение прибора.", errorMessage);
            }

            deviceFromDb.Name = deviceDto.Name;
            deviceFromDb.Description = deviceDto.Description;
            deviceFromDb.Quantity = deviceDto.Quantity;
            
            foreach (var comp in deviceFromDb.DeviceComponentsTemplate)
            {
                await _database.DeviceComponentsTemplate.DeleteAsync(comp.Id);
            }

            foreach (var des in deviceFromDb.DeviceDesignTemplate)
            {
                await _database.DeviceDesignTemplate.DeleteAsync(des.Id);
            }

            deviceFromDb.DeviceDesignTemplate = GetDesignsFromDeviceDto(deviceDto);
            deviceFromDb.DeviceComponentsTemplate = GetComponentsFromDeviceDto(deviceDto);
            
            _database.Devices.Update(deviceFromDb);
            await _database.SaveAsync();
            
            await _log.CreateLogAsync(new LogDTO($"Был изменён прибор: {deviceDto}"){DeviceId = deviceFromDb.Id});
        }

        public async Task<IEnumerable<DeviceDTO>> GetDevicesAsync()
        {
            var devices = await _database.Devices.GetAllAsync();
            if (devices == null)
            {
                return new List<DeviceDTO>(0);
            }

            return _mapperToDto.Map<IEnumerable<Device>, IEnumerable<DeviceDTO>>(devices);
        }

        public async Task<DeviceDTO> GetDeviceAsync(int? id)
        {
            if (id == null)
            {
                throw new NotImplementedException();
            }
            
            var device = await _database.Devices.GetAsync((int) id);
            if (device == null)
            {
                throw new NotImplementedException();   
            }

            var deviceDto = _mapperToDto.Map<Device, DeviceDTO>(device);
            await SetNameQuantityAndDescriptionForComponentAsync(deviceDto, device);
            await SetNameQuantityAndDescriptionForDesignAsync(deviceDto, device);
            return deviceDto;
        }

        public async Task DeleteDeviceAsync(int? id)
        {
            if (id == null)
            {
                throw new NotImplementedException();
            }
            
            var device = await _database.Devices.GetAsync((int) id) ?? throw new NotImplementedException();
            var checkInTask = (await CheckInTaskAsync(device));
            string errorMessage = checkInTask.Item2;
            if (!checkInTask.Item1)
            {
                throw new IntersectionOfEntitiesException("Ошибка. Невозможно удаление прибора.", errorMessage);
            }
            
            await _database.Devices.DeleteAsync((int) id);
            await _database.SaveAsync();
            
            await _log.CreateLogAsync(new LogDTO($"Был удалён прибор: {device}"));
        }

        private async Task<Tuple<bool, string>> CheckInTaskAsync(Device device)
        {
            string errorMessage;
            var task = (await _database.Tasks.GetAllAsync())
                .FirstOrDefault(t => device.Id == t.DeviceId);
            if (task != null)
            {
                errorMessage = $"<i class='bg-light'>{device.ToString()}</i> используется в <i class='bg-light'>задаче №{task.Id}</i>.<br />";
                return new Tuple<bool, string>(false, errorMessage);
            }
            
            errorMessage = String.Empty;
            return new Tuple<bool, string>(true, errorMessage);
        }

        public async Task<IEnumerable<string>> GetNamesAsync()
        {
            var devices = await _database.Devices.GetAllAsync();
            if (devices == null)
            {
                return Array.Empty<string>();
            }

            return devices.Select(d => d.ToString());
        }

        public async Task<IEnumerable<DeviceComponentsTemplate>> GetComponentsTemplatesAsync(int deviceId)
        {
            var device = await _database.Devices.GetAsync(deviceId);
            return device.DeviceComponentsTemplate;
        }

        public async Task<IEnumerable<DeviceDesignTemplate>> GetDesignTemplatesAsync(int deviceId)
        {
            var device = await _database.Devices.GetAsync(deviceId);
            return device.DeviceDesignTemplate;
        }
        
        public async Task ReceiveDeviceAsync(int? id)
        {
            await AddDeviceAsync(id, -1);
        }
        
        public async Task AddDeviceAsync(int? id)
        {
            await AddDeviceAsync(id, 1);
        }
        
        private async Task AddDeviceAsync(int? id, int quantity)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }

            var device = await _database.Devices.GetAsync((int) id);
            device.Quantity += quantity;
            await _database.SaveAsync();

            if (quantity < 0)
            {
                await _log.CreateLogAsync(new LogDTO($"Был получен прибор {device} со склада {-quantity}шт."){DeviceId = device.Id});
            }
            else
            {
                await _log.CreateLogAsync(new LogDTO($"Был добавлен прибор {device} на склад {quantity}шт."){DeviceId = device.Id});
            }
        }

        public void Dispose()
        {
            _database.Dispose();
        }

        private List<DeviceComponentsTemplate> GetComponentsFromDeviceDto(DeviceDTO deviceDto)
        {
            var result = new List<DeviceComponentsTemplate>();
            if (deviceDto.ComponentIds == null)
            {
                return result;
            }
            
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
            if (deviceDto.DesignIds == null)
            {
                return result;
            }
            
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

        private async Task SetNameQuantityAndDescriptionForComponentAsync(DeviceDTO deviceDto, Device device)
        {
            var componentService = new ComponentService(_database);
            var ids = new List<int>();
            var names = new List<string>();
            var quantity = new List<int>();
            var descriptions = new List<string>();
            var templateId = new List<int>();
            for (int i = 0; i < device.DeviceComponentsTemplate.Count; i++)
            {
                ids.Add(device.DeviceComponentsTemplate[i].ComponentId);
                names.Add((await componentService.GetComponentAsync(ids[^1])).ToString());
                quantity.Add(device.DeviceComponentsTemplate[i].Quantity);
                descriptions.Add(device.DeviceComponentsTemplate[i].Description);
                templateId.Add(device.DeviceComponentsTemplate[i].Id);
            }

            deviceDto.ComponentIds = ids.ToArray();
            deviceDto.ComponentNames = names.ToArray();
            deviceDto.ComponentQuantity = quantity.ToArray();
            deviceDto.ComponentDescriptions = descriptions.ToArray();
            deviceDto.ComponentTemplateId = templateId.ToArray();
        }
        
        private async Task SetNameQuantityAndDescriptionForDesignAsync(DeviceDTO deviceDto, Device device)
        {
            var designService = new DesignService(_database);
            var ids = new List<int>();
            var names = new List<string>();
            var quantity = new List<int>();
            var descriptions = new List<string>();
            var templateId = new List<int>();
            for (int i = 0; i < device.DeviceDesignTemplate.Count; i++)
            {
                ids.Add(device.DeviceDesignTemplate[i].DesignId);
                names.Add((await designService.GetDesignAsync(ids[^1])).ToString());
                quantity.Add(device.DeviceDesignTemplate[i].Quantity);
                descriptions.Add(device.DeviceDesignTemplate[i].Description);
                templateId.Add(device.DeviceDesignTemplate[i].Id);

            }

            deviceDto.DesignIds = ids.ToArray();
            deviceDto.DesignNames = names.ToArray();
            deviceDto.DesignQuantity = quantity.ToArray();
            deviceDto.DesignDescriptions = descriptions.ToArray();
            deviceDto.DesignTemplateId = templateId.ToArray();
        }
    }
}
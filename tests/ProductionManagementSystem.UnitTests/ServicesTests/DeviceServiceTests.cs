using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Services;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Repositories;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.UnitTests.ServicesTests
{
    public class DeviceServiceTests : TestBase
    {
        [Test]
        public async Task GetAllDeviceTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Devices.Add(new Device {Id = 1, Name = "name", Quantity = 10});
            context.Devices.Add(new Device {Id = 2, Name = "name2", Quantity = 20});
            context.Devices.Add(new Device {Id = 3, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var deviceService = new DeviceService(new EFUnitOfWork(context));
            var result = await deviceService.GetDevicesAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(3, result.Count());
        }
        
        [Test]
        public async Task GetDeviceByIdTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Devices.Add(new Device {Id = 5, Name = "name", Quantity = 10});
            context.Devices.Add(new Device {Id = 10, Name = "name2", Quantity = 20});
            context.Devices.Add(new Device {Id = 23, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var deviceService = new DeviceService(new EFUnitOfWork(context));
            var device = await deviceService.GetDeviceAsync(5);
            
            // Assert
            Assert.NotNull(device);
            Assert.AreEqual(5, device.Id);
        }
        
        [Test]
        public async Task GetDeviceByIdThrowExceptionTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Devices.Add(new Device {Id = 5, Name = "name", Quantity = 10});
            context.Devices.Add(new Device {Id = 10, Name = "name2", Quantity = 20});
            context.Devices.Add(new Device {Id = 23, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var deviceService = new DeviceService(new EFUnitOfWork(context));
            
            // Assert
            Assert.ThrowsAsync<NotImplementedException>(async () => await deviceService.GetDeviceAsync(4));
        }
        
        [Test]
        public async Task CreateDeviceTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Devices.Add(new Device {Id = 5, Name = "name", Quantity = 10});
            context.Devices.Add(new Device {Id = 10, Name = "name2", Quantity = 20});
            context.Devices.Add(new Device {Id = 23, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var deviceService = new DeviceService(new EFUnitOfWork(context));
            DeviceDTO deviceDto = new DeviceDTO
            {
                Name = "device1",
                Quantity = 10
            };
            await deviceService.CreateDeviceAsync(deviceDto);
            var deviceFromContext =  await deviceService.GetDeviceAsync(24);
            
            // Assert
            Assert.NotNull(deviceFromContext);
            Assert.AreEqual("device1", deviceFromContext.Name);
        }
        
        [Test]
        public async Task UpdateDeviceTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Devices.Add(new Device {Id = 5, Name = "name", Quantity = 10});
            context.Devices.Add(new Device {Id = 10, Name = "name2", Quantity = 20});
            context.Devices.Add(new Device {Id = 23, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var deviceService = new DeviceService(new EFUnitOfWork(context));
            DeviceDTO deviceDto = new DeviceDTO
            {
                Id = 23,
                Name = "device1",
                Quantity = 10
            };
            await deviceService.UpdateDeviceAsync(deviceDto);
            var deviceFromContext =  await deviceService.GetDeviceAsync(23);
            
            // Assert
            Assert.NotNull(deviceFromContext);
            Assert.AreEqual("device1", deviceFromContext.Name);
        }
        
        [Test]
        public async Task UpdateDeviceThrowIntersectionOfEntitiesException()
        {
            // Arrange
            var context = GetDbContext();
            context.Devices.Add(new Device {Id = 23, Name = "name3", Quantity = 30});
            context.Tasks.Add(new DAL.Entities.Task {Id = 1, DeviceId = 23});
            await context.SaveChangesAsync();
            
            // Act
            var deviceService = new DeviceService(new EFUnitOfWork(context));
            DeviceDTO deviceDto = new DeviceDTO
            {
                Id = 23,
                Name = "device1",
                Quantity = 10
            };
            
            // Assert
            Assert.ThrowsAsync<IntersectionOfEntitiesException>(async () => await deviceService.UpdateDeviceAsync(deviceDto));
        }
        
        [Test]
        public async Task UpdateDeviceThrowExceptionTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Devices.Add(new Device {Id = 5, Name = "name", Quantity = 10});
            context.Devices.Add(new Device {Id = 10, Name = "name2", Quantity = 20});
            context.Devices.Add(new Device {Id = 23, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var deviceService = new DeviceService(new EFUnitOfWork(context));
            DeviceDTO deviceDto = new DeviceDTO
            {
                Id = 22,
                Name = "device1",
                Quantity = 10
            };
            
            // Assert
            Assert.ThrowsAsync<NotImplementedException>(async () => await deviceService.UpdateDeviceAsync(deviceDto));
        }
        
        [Test]
        public async Task DeleteDeviceTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Devices.Add(new Device {Id = 5, Name = "name", Quantity = 10});
            context.Devices.Add(new Device {Id = 10, Name = "name2", Quantity = 20});
            context.Devices.Add(new Device {Id = 23, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var deviceService = new DeviceService(new EFUnitOfWork(context));
            await deviceService.DeleteDeviceAsync(23);
            var devices = await deviceService.GetDevicesAsync();
            
            // Assert
            Assert.NotNull(devices);
            Assert.AreEqual(2, devices.Count());
        }
        
        [Test]
        public async Task DeleteDeviceThrowExceptionTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Devices.Add(new Device {Id = 5, Name = "name", Quantity = 10});
            context.Devices.Add(new Device {Id = 10, Name = "name2", Quantity = 20});
            context.Devices.Add(new Device {Id = 23, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var deviceService = new DeviceService(new EFUnitOfWork(context));
            
            // Assert
            Assert.ThrowsAsync<NotImplementedException>(async () => await deviceService.DeleteDeviceAsync(22));
        }
        
        [Test]
        public async Task DeleteDeviceThrowIntersectionOfEntitiesException()
        {
            // Arrange
            var context = GetDbContext();
            context.Devices.Add(new Device {Id = 23, Name = "name3", Quantity = 30});
            context.Tasks.Add(new DAL.Entities.Task {Id = 1, DeviceId = 23});
            await context.SaveChangesAsync();
            
            // Act
            var deviceService = new DeviceService(new EFUnitOfWork(context));
            
            // Assert
            Assert.ThrowsAsync<IntersectionOfEntitiesException>(async () => await deviceService.DeleteDeviceAsync(23));
        }
        
        [Test]
        public async Task GetNameDevicesTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Devices.Add(new Device {Id = 5, Name = "name", Quantity = 10});
            context.Devices.Add(new Device {Id = 10, Name = "name2", Quantity = 20});
            context.Devices.Add(new Device {Id = 23, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var deviceService = new DeviceService(new EFUnitOfWork(context));
            List<string> names = (await deviceService.GetNamesAsync()).ToList();
            
            // Assert
            Assert.AreEqual(new List<string>{"name", "name2", "name3"}, names);
        }
        
        [Test]
        public async Task AddDeviceTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Devices.Add(new Device {Id = 5, Name = "name", Quantity = 10});
            await context.SaveChangesAsync();
            
            // Act
            var deviceService = new DeviceService(new EFUnitOfWork(context));
            await deviceService.AddDeviceAsync(5);
            var device = await deviceService.GetDeviceAsync(5);
            
            // Assert
            Assert.NotNull(device);
            Assert.AreEqual(11, device.Quantity);
        }
        
        [Test]
        public async Task ReceivedDeviceTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Devices.Add(new Device {Id = 5, Name = "name", Quantity = 10});
            await context.SaveChangesAsync();
            
            // Act
            var deviceService = new DeviceService(new EFUnitOfWork(context));
            await deviceService.ReceiveDeviceAsync(5);
            var device = await deviceService.GetDeviceAsync(5);
            
            // Assert
            Assert.NotNull(device);
            Assert.AreEqual(9, device.Quantity);
        }

        [Test]
        public async Task GetDesignsTemplatesByDeviceId()
        {
            // Arrange
            var context = GetDbContext();
            context.Designs.Add(new Design {Id = 1, Type = "type1", Name = "name1"});
            context.Designs.Add(new Design {Id = 2, Type = "type1", Name = "name1"});
            context.Designs.Add(new Design {Id = 3, Type = "type1", Name = "name1"});
            context.Designs.Add(new Design {Id = 4, Type = "type1", Name = "name1"});
            context.Devices.Add(new Device
            {
                Id = 1, Name = "123",
                DeviceDesignTemplate = new List<DeviceDesignTemplate>
                {
                    new DeviceDesignTemplate {DesignId = 1, DeviceId = 1, Quantity = 1},
                    new DeviceDesignTemplate {DesignId = 2, DeviceId = 1, Quantity = 1},
                    new DeviceDesignTemplate {DesignId = 3, DeviceId = 1, Quantity = 1},
                    new DeviceDesignTemplate {DesignId = 4, DeviceId = 1, Quantity = 1}
                }
            });
            await context.SaveChangesAsync();

            // Act
            var deviceService = new DeviceService(new EFUnitOfWork(context));
            var result = await deviceService.GetDesignTemplatesAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(4, result.Count());
        }
        
        [Test]
        public async Task GetComponentsTemplatesByDeviceId()
        {
            // Arrange
            var context = GetDbContext();
            context.Components.Add(new Component {Id = 1, Type = "type1", Name = "name1"});
            context.Components.Add(new Component {Id = 2, Type = "type1", Name = "name1"});
            context.Components.Add(new Component {Id = 3, Type = "type1", Name = "name1"});
            context.Components.Add(new Component {Id = 4, Type = "type1", Name = "name1"});
            context.Devices.Add(new Device
            {
                Id = 1, Name = "123",
                DeviceComponentsTemplate = new List<DeviceComponentsTemplate>
                {
                    new DeviceComponentsTemplate {ComponentId = 1, DeviceId = 1, Quantity = 1},
                    new DeviceComponentsTemplate {ComponentId = 2, DeviceId = 1, Quantity = 1},
                    new DeviceComponentsTemplate {ComponentId = 3, DeviceId = 1, Quantity = 1},
                    new DeviceComponentsTemplate {ComponentId = 4, DeviceId = 1, Quantity = 1}
                }
            });
            await context.SaveChangesAsync();

            // Act
            var deviceService = new DeviceService(new EFUnitOfWork(context));
            var result = await deviceService.GetComponentsTemplatesAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(4, result.Count());
        }
    }
}
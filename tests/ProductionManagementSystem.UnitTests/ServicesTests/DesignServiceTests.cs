using System;
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
    public class DesignServiceTests : TestBase
    {
        [Test]
        public async Task GetAllDesignsTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Designs.Add(new Design {Id = 1, Name = "name", Quantity = 10});
            context.Designs.Add(new Design {Id = 2, Name = "name2", Quantity = 20});
            context.Designs.Add(new Design {Id = 3, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var designService = new DesignService(new EFUnitOfWork(context));
            var result = await designService.GetDesignsAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(3, result.Count());
        }
        
        [Test]
        public async Task GetAllDesignsEmptyContextTest()
        {
            // Arrange
            var context = GetDbContext();
            
            // Act
            var designService = new DesignService(new EFUnitOfWork(context));
            var result = await designService.GetDesignsAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count());
        }
        
        [Test]
        public async Task GetDesignByIdTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Designs.Add(new Design {Id = 2, Name = "name", Quantity = 10});
            context.Designs.Add(new Design {Id = 5, Name = "name2", Quantity = 20});
            context.Designs.Add(new Design {Id = 10, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var designService = new DesignService(new EFUnitOfWork(context));
            var result = await designService.GetDesignAsync(2);
            
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Id);
        }
        
        [Test]
        public async Task DesignUpdateTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Designs.Add(new Design {Id = 2, Name = "name", Quantity = 10});
            context.Designs.Add(new Design {Id = 5, Name = "name2", Quantity = 20});
            context.Designs.Add(new Design {Id = 10, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var designService = new DesignService(new EFUnitOfWork(context));
            var design = await designService.GetDesignAsync(2);
            design.Quantity = 1;
            design.Name = "designId2";
            await designService.UpdateDesignAsync(design);
            var result = await designService.GetDesignAsync(2);
            
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Id);
            Assert.AreEqual("designId2", result.Name);
            Assert.AreEqual(1, result.Quantity);
        }
        
        [Test]
        public async Task DesignDeleteTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Designs.Add(new Design {Id = 2, Name = "name", Quantity = 10});
            context.Designs.Add(new Design {Id = 5, Name = "name2", Quantity = 20});
            context.Designs.Add(new Design {Id = 10, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var designService = new DesignService(new EFUnitOfWork(context));
            await designService.DeleteDesignAsync(2);
            
            // Assert
            Assert.ThrowsAsync<NotImplementedException>(async () => await designService.GetDesignAsync(2));
        }
        
        [Test]
        public async Task DesignDeleteExceptionTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Designs.Add(new Design {Id = 2, Name = "name", Quantity = 10});
            context.Designs.Add(new Design {Id = 5, Name = "name2", Quantity = 20});
            context.Designs.Add(new Design {Id = 10, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var designService = new DesignService(new EFUnitOfWork(context));
            
            // Assert
            Assert.ThrowsAsync<NotImplementedException>(async () => await designService.DeleteDesignAsync(1));
        }
        
        [Test]
        public async Task GetDesignGetByIdExceptionTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Designs.Add(new Design {Id = 2, Name = "name", Quantity = 10});
            context.Designs.Add(new Design {Id = 5, Name = "name2", Quantity = 20});
            context.Designs.Add(new Design {Id = 10, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var designService = new DesignService(new EFUnitOfWork(context));
            
            // Assert
            Assert.ThrowsAsync<NotImplementedException>(async () => await designService.GetDesignAsync(1));
        }

        [Test]
        public async Task CreateDesign()
        {
            // Arrange
            var context = GetDbContext();
            context.Designs.Add(new Design {Id = 2, Name = "name", Quantity = 10});
            context.Designs.Add(new Design {Id = 5, Name = "name2", Quantity = 20});
            context.Designs.Add(new Design {Id = 10, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var designService = new DesignService(new EFUnitOfWork(context));
            var design = new DesignDTO
            {
                Quantity = 1,
                Name = "designId2"
            };
            
            await designService.CreateDesignAsync(design);
            var result = await designService.GetDesignAsync(11);
            
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(11, result.Id);
            Assert.AreEqual("designId2", result.Name);
            Assert.AreEqual(1, result.Quantity);
        }
        
        [Test]
        public async Task GetAllTypesOfDesignsTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Designs.Add(new Design {Id = 1, Type = "t1", Name = "name", Quantity = 10});
            context.Designs.Add(new Design {Id = 2, Type = "t1", Name = "name2", Quantity = 20});
            context.Designs.Add(new Design {Id = 3, Type = "t2", Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var designService = new DesignService(new EFUnitOfWork(context));
            var result = await designService.GetTypesAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());
        }
        
        [Test]
        public async Task AddDesignsTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Designs.Add(new Design {Id = 1, Type = "t1", Name = "name", Quantity = 10});
            context.Designs.Add(new Design {Id = 2, Type = "t1", Name = "name2", Quantity = 20});
            context.Designs.Add(new Design {Id = 3, Type = "t2", Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var designService = new DesignService(new EFUnitOfWork(context));
            await designService.AddDesignAsync(1, 5);
            var design = await designService.GetDesignAsync(1);
            
            // Assert
            Assert.NotNull(design);
            Assert.AreEqual(15, design.Quantity);
        }
        
        [Test]
        public async Task ReceiveDesignsTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Designs.Add(new Design {Id = 1, Type = "t1", Name = "name", Quantity = 10});
            context.Designs.Add(new Design {Id = 2, Type = "t1", Name = "name2", Quantity = 20});
            context.Designs.Add(new Design {Id = 3, Type = "t2", Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var designService = new DesignService(new EFUnitOfWork(context));
            await designService.ReceiveDesignAsync(1, 5);
            var design = await designService.GetDesignAsync(1);
            
            // Assert
            Assert.NotNull(design);
            Assert.AreEqual(5, design.Quantity);
        }

        [Test]
        public async Task DeleteUsedDesignThrowIntersectionOfEntitiesExceptionTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Designs.Add(new Design {Id = 1, Type = "t1", Name = "name", Quantity = 10});
            context.Devices.Add(new Device
            {
                Id = 1, Name = "123",
                DeviceDesignTemplate = new List<DeviceDesignTemplate>
                    {new DeviceDesignTemplate {DesignId = 1, DeviceId = 1, Quantity = 1}}
            });
            await context.SaveChangesAsync();
            
            // Act
            var designService = new DesignService(new EFUnitOfWork(context));
            
            // Assert
            Assert.ThrowsAsync<IntersectionOfEntitiesException>(async () => await designService.DeleteDesignAsync(1));
        }
    }
}
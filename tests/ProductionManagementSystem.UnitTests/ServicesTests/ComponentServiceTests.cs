using System;
using System.Linq;
using NUnit.Framework;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Services;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Repositories;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.UnitTests.ServicesTests
{
    public class ComponentServiceTests : TestBase
    {
        [Test]
        public async Task GetAllComponentsTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Components.Add(new Component {Id = 1, Name = "name", Quantity = 10});
            context.Components.Add(new Component {Id = 2, Name = "name2", Quantity = 20});
            context.Components.Add(new Component {Id = 3, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var componentService = new ComponentService(new EFUnitOfWork(context));
            var result = await componentService.GetComponentsAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(3, result.Count());
        }
        
        [Test]
        public async Task GetAllComponentsEmptyContextTest()
        {
            // Arrange
            var context = GetDbContext();
            
            // Act
            var componentService = new ComponentService(new EFUnitOfWork(context));
            var result = await componentService.GetComponentsAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count());
        }
        
        [Test]
        public async Task GetComponentByIdTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Components.Add(new Component {Id = 2, Name = "name", Quantity = 10});
            context.Components.Add(new Component {Id = 5, Name = "name2", Quantity = 20});
            context.Components.Add(new Component {Id = 10, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var componentService = new ComponentService(new EFUnitOfWork(context));
            var result = await componentService.GetComponentAsync(2);
            
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Id);
        }
        
        [Test]
        public async Task ComponentUpdateTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Components.Add(new Component {Id = 2, Name = "name", Quantity = 10});
            context.Components.Add(new Component {Id = 5, Name = "name2", Quantity = 20});
            context.Components.Add(new Component {Id = 10, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var componentService = new ComponentService(new EFUnitOfWork(context));
            var component = await componentService.GetComponentAsync(2);
            component.Corpus = "c";
            component.Quantity = 1;
            component.Name = "componentId2";
            await componentService.UpdateComponentAsync(component);
            var result = await componentService.GetComponentAsync(2);
            
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Id);
            Assert.AreEqual("c", result.Corpus);
            Assert.AreEqual("componentId2", result.Name);
            Assert.AreEqual(1, result.Quantity);
        }
        
        [Test]
        public async Task ComponentDeleteTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Components.Add(new Component {Id = 2, Name = "name", Quantity = 10});
            context.Components.Add(new Component {Id = 5, Name = "name2", Quantity = 20});
            context.Components.Add(new Component {Id = 10, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var componentService = new ComponentService(new EFUnitOfWork(context));
            await componentService.DeleteComponentAsync(2);
            
            // Assert
            Assert.ThrowsAsync<NotImplementedException>(async () => await componentService.GetComponentAsync(2));
        }
        
        [Test]
        public async Task ComponentDeleteExceptionTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Components.Add(new Component {Id = 2, Name = "name", Quantity = 10});
            context.Components.Add(new Component {Id = 5, Name = "name2", Quantity = 20});
            context.Components.Add(new Component {Id = 10, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var componentService = new ComponentService(new EFUnitOfWork(context));
            
            // Assert
            Assert.ThrowsAsync<NotImplementedException>(async () => await componentService.DeleteComponentAsync(1));
        }
        
        [Test]
        public async Task GetComponentGetByIdExceptionTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Components.Add(new Component {Id = 2, Name = "name", Quantity = 10});
            context.Components.Add(new Component {Id = 5, Name = "name2", Quantity = 20});
            context.Components.Add(new Component {Id = 10, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var componentService = new ComponentService(new EFUnitOfWork(context));
            
            // Assert
            Assert.ThrowsAsync<NotImplementedException>(async () => await componentService.GetComponentAsync(1));
        }

        [Test]
        public async Task CreateComponent()
        {
            // Arrange
            var context = GetDbContext();
            context.Components.Add(new Component {Id = 2, Name = "name", Quantity = 10});
            context.Components.Add(new Component {Id = 5, Name = "name2", Quantity = 20});
            context.Components.Add(new Component {Id = 10, Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var componentService = new ComponentService(new EFUnitOfWork(context));
            var component = new ComponentDTO
            {
                Corpus = "c",
                Quantity = 1,
                Name = "componentId2"
            };
            
            await componentService.CreateComponentAsync(component);
            var result = await componentService.GetComponentAsync(11);
            
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(11, result.Id);
            Assert.AreEqual("c", result.Corpus);
            Assert.AreEqual("componentId2", result.Name);
            Assert.AreEqual(1, result.Quantity);
        }
        
        [Test]
        public async Task GetAllTypesOfComponentsTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Components.Add(new Component {Id = 1, Type = "t1", Name = "name", Quantity = 10});
            context.Components.Add(new Component {Id = 2, Type = "t1", Name = "name2", Quantity = 20});
            context.Components.Add(new Component {Id = 3, Type = "t2", Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var componentService = new ComponentService(new EFUnitOfWork(context));
            var result = await componentService.GetTypesAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());
        }
        
        [Test]
        public async Task AddComponentsTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Components.Add(new Component {Id = 1, Type = "t1", Name = "name", Quantity = 10});
            context.Components.Add(new Component {Id = 2, Type = "t1", Name = "name2", Quantity = 20});
            context.Components.Add(new Component {Id = 3, Type = "t2", Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var componentService = new ComponentService(new EFUnitOfWork(context));
            await componentService.AddComponentAsync(1, 5);
            var component = await componentService.GetComponentAsync(1);
            
            // Assert
            Assert.NotNull(component);
            Assert.AreEqual(15, component.Quantity);
        }
        
        [Test]
        public async Task ReceiveComponentsTest()
        {
            // Arrange
            var context = GetDbContext();
            context.Components.Add(new Component {Id = 1, Type = "t1", Name = "name", Quantity = 10});
            context.Components.Add(new Component {Id = 2, Type = "t1", Name = "name2", Quantity = 20});
            context.Components.Add(new Component {Id = 3, Type = "t2", Name = "name3", Quantity = 30});
            await context.SaveChangesAsync();
            
            // Act
            var componentService = new ComponentService(new EFUnitOfWork(context));
            await componentService.ReceiveComponentAsync(1, 5);
            var component = await componentService.GetComponentAsync(1);
            
            // Assert
            Assert.NotNull(component);
            Assert.AreEqual(5, component.Quantity);
        }
    }
}
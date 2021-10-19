using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Services;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Enums;
using ProductionManagementSystem.DAL.Repositories;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.UnitTests.ServicesTests
{
    public class ComponentsSupplyRequestServiceTests : TestBase
    {
        [Test]
        public async Task GetAllComponentsSupplyRequestsTest()
        {
            // Arrange
            var context = await GetInitDbContext();
            
            // Act
            var componentService = new ComponentsSupplyRequestService(new EFUnitOfWork(context));
            var result = await componentService.GetComponentSupplyRequestsAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(3, result.Count());
        }
        
        [Test]
        public async Task GetComponentSupplyRequestsTest()
        {
            // Arrange
            var context = await GetInitDbContext();
            
            // Act
            var componentService = new ComponentsSupplyRequestService(new EFUnitOfWork(context));
            var result = await componentService.GetComponentSupplyRequestAsync(1);
            
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.ComponentId);
        }
        
        [Test]
        public async Task DeleteComponentSupplyRequestTest()
        {
            // Arrange
            var context = await GetInitDbContext();
            
            // Act
            var componentService = new ComponentsSupplyRequestService(new EFUnitOfWork(context));
            await componentService.DeleteComponentSupplyRequestAsync(1);
            
            // Assert
            Assert.AreEqual(2, context.ComponentsSupplyRequests.Count());
        }
        
        [Test]
        public async Task UpdateComponentSupplyRequestTest()
        {
            // Arrange
            var context = await GetInitDbContext();
            
            // Act
            var componentService = new ComponentsSupplyRequestService(new EFUnitOfWork(context));
            await componentService.UpdateComponentSupplyRequestAsync(new ComponentsSupplyRequestDTO {Id = 3, TaskId = 1, ComponentId = 3, Comment = "5", Quantity = 30, StatusSupply = StatusSupplyEnumDTO.Ready});
            
            // Assert
            var updatedItem = context.ComponentsSupplyRequests.FirstOrDefault(c => c.Id == 3) ?? throw new NullReferenceException("updateItem");
            Assert.AreEqual("5", updatedItem.Comment);
            Assert.AreEqual(StatusSupplyEnum.Ready, updatedItem.StatusSupply);
        }
        
        [Test]
        public async Task CreateComponentSupplyRequestTest()
        {
            // Arrange
            var context = await GetInitDbContext();
            
            // Act
            var componentService = new ComponentsSupplyRequestService(new EFUnitOfWork(context));
            await componentService.CreateComponentSupplyRequestAsync(new ComponentsSupplyRequestDTO {TaskId = 1, ComponentId = 4, Comment = "5", Quantity = 30, StatusSupply = StatusSupplyEnumDTO.Ready});
            
            // Assert
            var createdItem = context.ComponentsSupplyRequests.Last() ?? throw new NullReferenceException("updateItem");
            Assert.AreEqual(4, context.ComponentsSupplyRequests.Count());
            Assert.AreEqual(4, createdItem.Id);
        }

        private async Task<ApplicationContext> GetInitDbContext()
        {
            var context = GetDbContext();
            
            context.Components.Add(new Component {Id = 1, Type = "t1", Name = "component1", Quantity = 10});
            context.Components.Add(new Component {Id = 2, Type = "t1", Name = "component2", Quantity = 20});
            context.Components.Add(new Component {Id = 3, Type = "t2", Name = "component3", Quantity = 30});
            context.Components.Add(new Component {Id = 4, Type = "t4", Name = "component4", Quantity = 40});
            
            context.Designs.Add(new Design {Id = 1, Name = "design1", Quantity = 11});
            context.Designs.Add(new Design {Id = 2, Name = "design1", Quantity = 22});
            context.Designs.Add(new Design {Id = 3, Name = "design1", Quantity = 33});
            context.Designs.Add(new Design {Id = 4, Name = "design1", Quantity = 44});
            
            context.Devices.Add(new Device
            {
                Id = 1, Name = "device1",
                DeviceDesignTemplate = new List<DeviceDesignTemplate>
                {
                    new DeviceDesignTemplate {DesignId = 1, DeviceId = 1, Quantity = 1},
                    new DeviceDesignTemplate {DesignId = 2, DeviceId = 1, Quantity = 1},
                    new DeviceDesignTemplate {DesignId = 3, DeviceId = 1, Quantity = 1},
                    new DeviceDesignTemplate {DesignId = 4, DeviceId = 1, Quantity = 1}
                },
                DeviceComponentsTemplate = new List<DeviceComponentsTemplate>
                {
                    new DeviceComponentsTemplate {ComponentId = 1, DeviceId = 1, Quantity = 4},
                    new DeviceComponentsTemplate {ComponentId = 2, DeviceId = 1, Quantity = 3},
                    new DeviceComponentsTemplate {ComponentId = 3, DeviceId = 1, Quantity = 2},
                    new DeviceComponentsTemplate {ComponentId = 4, DeviceId = 1, Quantity = 1}
                }
            });
            context.Devices.Add(new Device
            {
                Id = 2, Name = "device2",
                DeviceDesignTemplate = new List<DeviceDesignTemplate>
                {
                    new DeviceDesignTemplate {DesignId = 1, DeviceId = 2, Quantity = 1},
                    new DeviceDesignTemplate {DesignId = 4, DeviceId = 2, Quantity = 1}
                },
                DeviceComponentsTemplate = new List<DeviceComponentsTemplate>
                {
                    new DeviceComponentsTemplate {ComponentId = 1, DeviceId = 2, Quantity = 1},
                }
            });
            context.Devices.Add(new Device
            {
                Id = 3, Name = "device3",
                DeviceDesignTemplate = new List<DeviceDesignTemplate>
                {
                    new DeviceDesignTemplate {DesignId = 1, DeviceId = 3, Quantity = 1},
                },
                DeviceComponentsTemplate = new List<DeviceComponentsTemplate>
                {
                    new DeviceComponentsTemplate {ComponentId = 1, DeviceId = 3, Quantity = 1},
                }
            });
            
            context.Tasks.Add(new DAL.Entities.Task
            {
                Id = 1, 
                DeviceId = 1, 
                Description = "device1",
                Status = StatusEnum.Assembly | StatusEnum.Validate | StatusEnum.Equipment,
                ObtainedComponents = new []
                {
                    new ObtainedComponent {Id = 1, ComponentId = 1, TaskId = 1, Obtained = 0},
                    new ObtainedComponent {Id = 2, ComponentId = 2, TaskId = 1, Obtained = 0},
                    new ObtainedComponent {Id = 3, ComponentId = 3, TaskId = 1, Obtained = 0},
                    new ObtainedComponent {Id = 4, ComponentId = 4, TaskId = 1, Obtained = 1}
                },
                ObtainedDesigns = new []
                {
                    new ObtainedDesign {Id = 1, DesignId = 1, TaskId = 1, Obtained = 0},
                    new ObtainedDesign {Id = 2, DesignId = 2, TaskId = 1, Obtained = 0},
                    new ObtainedDesign {Id = 3, DesignId = 3, TaskId = 1, Obtained = 0},
                    new ObtainedDesign {Id = 4, DesignId = 4, TaskId = 1, Obtained = 1}
                }
            });
            context.Tasks.Add(new DAL.Entities.Task
            {
                Id = 2, 
                DeviceId = 3, 
                Description = "device3",
                Status = StatusEnum.Customization | StatusEnum.Assembly,
                ObtainedComponents = new []
                {
                    new ObtainedComponent {Id = 5, ComponentId = 1, TaskId = 2, Obtained = 0},
                },
                ObtainedDesigns = new []
                {
                    new ObtainedDesign {Id = 5, DesignId = 1, TaskId = 2, Obtained = 0},
                }
            });
            context.Tasks.Add(new DAL.Entities.Task
            {
                Id = 3, 
                DeviceId = 2, 
                Description = "device2",
                Status = StatusEnum.Montage | StatusEnum.Equipment,
                ObtainedComponents = new []
                {
                    new ObtainedComponent {Id = 6, ComponentId = 1, TaskId = 3, Obtained = 0},
                },
                ObtainedDesigns = new []
                {
                    new ObtainedDesign {Id = 6, DesignId = 1, TaskId = 3, Obtained = 0},
                    new ObtainedDesign {Id = 7, DesignId = 4, TaskId = 3, Obtained = 0},
                }
            });
            context.Tasks.Add(new DAL.Entities.Task
            {
                Id = 24, 
                DeviceId = 3, 
                Description = "device3",
                Status = StatusEnum.Warehouse,
                ObtainedComponents = new []
                {
                    new ObtainedComponent {Id = 7, ComponentId = 1, TaskId = 24, Obtained = 0},
                },
                ObtainedDesigns = new []
                {
                    new ObtainedDesign {Id = 8, DesignId = 1, TaskId = 24, Obtained = 0},
                }
            });
            
            context.ComponentsSupplyRequests.Add(new ComponentsSupplyRequest {Id = 1, TaskId = 1, ComponentId = 1, Comment = "1", Quantity = 10, StatusSupply = StatusSupplyEnum.Ordered});
            context.ComponentsSupplyRequests.Add(new ComponentsSupplyRequest {Id = 2, TaskId = 1, ComponentId = 2, Comment = "2", Quantity = 20, StatusSupply = StatusSupplyEnum.NotAccepted});
            context.ComponentsSupplyRequests.Add(new ComponentsSupplyRequest {Id = 3, TaskId = 1, ComponentId = 3, Comment = "3", Quantity = 30, StatusSupply = StatusSupplyEnum.NotAccepted});
            
            await context.SaveChangesAsync();
            return context;
        }
    }
}
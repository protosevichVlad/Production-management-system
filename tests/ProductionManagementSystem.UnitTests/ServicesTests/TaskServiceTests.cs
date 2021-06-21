using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
    public class TaskServiceTests : TestBase
    {
        [Test]
        public async Task CreateTaskTest()
        {
            // Arrange
            var context = await GetInitDbContext();

            // Act
            var taskService = new TaskService(new EFUnitOfWork(context));
            var taskDto = new TaskDTO
            {
                DeviceId = 2,
                Description = "device2",
            };
            
            var countTasksBefore = context.Tasks.Count();
            var countObtainedComponentsBefore = context.ObtainedComponents.Count();
            var countObtainedDesignsBefore = context.ObtainedDesigns.Count();
            
            await taskService.CreateTaskAsync(taskDto);
            
            var countTasksAfter = context.Tasks.Count();
            var countObtainedComponentsAfter = context.ObtainedComponents.Count();
            var countObtainedDesignsAfter = context.ObtainedDesigns.Count();

            // Assert
            Assert.AreEqual(1, countTasksAfter - countTasksBefore);
            Assert.AreEqual(1, countObtainedComponentsAfter - countObtainedComponentsBefore);
            Assert.AreEqual(2, countObtainedDesignsAfter - countObtainedDesignsBefore);
        }
        
        [Test]
        public async Task UpdateTaskTest()
        {
            // Arrange
            var context = await GetInitDbContext();

            // Act
            var taskService = new TaskService(new EFUnitOfWork(context));
            var taskDto = new TaskDTO
            {
                Id = 3,
                DeviceId = 2,
                Description = "device2 updated",
                Deadline = new DateTime(2022,1, 1)
            };
            
            var countTasksBefore = context.Tasks.Count();
            var countObtainedComponentsBefore = context.ObtainedComponents.Count();
            var countObtainedDesignsBefore = context.ObtainedDesigns.Count();
            
            await taskService.UpdateTaskAsync(taskDto);
            
            var countTasksAfter = context.Tasks.Count();
            var countObtainedComponentsAfter = context.ObtainedComponents.Count();
            var countObtainedDesignsAfter = context.ObtainedDesigns.Count();

            // Assert
            Assert.AreEqual(0, countTasksAfter - countTasksBefore);
            Assert.AreEqual(0, countObtainedComponentsAfter - countObtainedComponentsBefore);
            Assert.AreEqual(0, countObtainedDesignsAfter - countObtainedDesignsBefore);
            Assert.AreEqual("device2 updated", (await context.Tasks.FirstOrDefaultAsync(t => t.Id == 3)).Description);
        }
        
        [Test]
        public async Task GetAllTasksTest()
        {
            // Arrange
            var context = await GetInitDbContext();

            // Act
            var taskService = new TaskService(new EFUnitOfWork(context));
            var tasks = await taskService.GetTasksAsync();

            // Assert
            Assert.NotNull(tasks);
            Assert.AreEqual(context.Tasks.Count(), tasks.Count());
        }
        
        [Test]
        public async Task GetTaskByIdTest()
        {
            // Arrange
            var context = await GetInitDbContext();

            // Act
            var taskService = new TaskService(new EFUnitOfWork(context));
            var task = await taskService.GetTaskAsync(1);

            // Assert
            Assert.NotNull(task);
            Assert.AreEqual(1, task.Id);
            Assert.AreEqual("device1", task.Description);
        }
        
        [TestCaseSource(typeof(TaskServiceDataSource), nameof(TaskServiceDataSource.TestRoles))]
        public async Task GetTasksByRolesTest(string[] roles, StatusEnum status)
        {
            // Arrange
            var context = await GetInitDbContext();

            // Act
            var taskService = new TaskService(new EFUnitOfWork(context));
            var tasks = (await taskService.GetTasksAsync(roles)).ToList();

            // Assert
            Assert.NotNull(tasks);
            Assert.AreEqual(tasks.Count(), tasks.Where(t => (t.Status & status) != 0).Count());
        }
        
        [Test]
        public async Task DeleteTaskTest()
        {
            // Arrange
            var context = await GetInitDbContext();

            // Act
            var taskService = new TaskService(new EFUnitOfWork(context));
            
            var countTasksBefore = context.Tasks.Count();
            var countObtainedComponentsBefore = context.ObtainedComponents.Count();
            var countObtainedDesignsBefore = context.ObtainedDesigns.Count();
            
            await taskService.DeleteTaskAsync(3); // deviceId 2
            
            var countTasksAfter = context.Tasks.Count();
            var countObtainedComponentsAfter = context.ObtainedComponents.Count();
            var countObtainedDesignsAfter = context.ObtainedDesigns.Count();

            // Assert
            Assert.AreEqual(1, countTasksBefore - countTasksAfter);
            Assert.AreEqual(1, countObtainedComponentsBefore - countObtainedComponentsAfter);
            Assert.AreEqual(2, countObtainedDesignsBefore - countObtainedDesignsAfter);
        }
        
        [Test]
        public async Task GetDeviceDesignTemplateFromTaskTest()
        {
            // Arrange
            var context = await GetInitDbContext();

            // Act
            var taskService = new TaskService(new EFUnitOfWork(context));


            var deviceDesignTemplate = await taskService.GetDeviceDesignTemplateFromTaskAsync(3);


            // Assert
            Assert.NotNull(deviceDesignTemplate);
            Assert.AreEqual(2, deviceDesignTemplate.Count());
        }
        
        [Test]
        public async Task GetDeviceComponentsTemplateFromTaskTest()
        {
            // Arrange
            var context = await GetInitDbContext();

            // Act
            var taskService = new TaskService(new EFUnitOfWork(context));


            var deviceComponentsTemplate = await taskService.GetDeviceComponentsTemplatesFromTaskAsync(3);


            // Assert
            Assert.NotNull(deviceComponentsTemplate);
            Assert.AreEqual(1, deviceComponentsTemplate.Count());
        }
        
        [Test]
        public async Task GetObtainedDesignsTest()
        {
            // Arrange
            var context = await GetInitDbContext();

            // Act
            var taskService = new TaskService(new EFUnitOfWork(context));
            var obtainedDesigns =  taskService.GetObtainedDesigns(1).ToList();


            // Assert
            Assert.NotNull(obtainedDesigns);
            Assert.AreEqual(4, obtainedDesigns.Count);
            Assert.AreEqual(1, obtainedDesigns[^1].Obtained);
        }
        
        [Test]
        public async Task GetObtainedComponentsTest()
        {
            // Arrange
            var context = await GetInitDbContext();

            // Act
            var taskService = new TaskService(new EFUnitOfWork(context));
            var obtainedComponents = taskService.GetObtainedComponents(1).ToList();


            // Assert
            Assert.NotNull(obtainedComponents);
            Assert.AreEqual(4, obtainedComponents.Count);
            Assert.AreEqual(1, obtainedComponents[^1].Obtained);
        }
        
        [TestCaseSource(typeof(TaskServiceDataSource), nameof(TaskServiceDataSource.StatusName))]
        public async Task GetTaskStatusNameTest(string status_string, StatusEnum status)
        {
            // Arrange
            var context = await GetInitDbContext();

            // Act
            var taskService = new TaskService(new EFUnitOfWork(context));
            var statusName = taskService.GetTaskStatusName(status);

            // Assert
            Assert.AreEqual(statusName, status_string);
        }

        [Test]
        public async Task ReceiveComponentsAsyncTest()
        {
            // Arrange
            var context = await GetInitDbContext();

            // Act
            var taskService = new TaskService(new EFUnitOfWork(context));
            await taskService.ReceiveComponentsAsync(1, new[] {1, 2}, new []{1, 2});

            // Assert
            Assert.AreEqual(9, (await context.Components.FirstOrDefaultAsync(c => c.Id == 1)).Quantity);
            Assert.AreEqual(18, (await context.Components.FirstOrDefaultAsync(c => c.Id == 2)).Quantity);
            Assert.AreEqual(1, (await context.ObtainedComponents.FirstOrDefaultAsync(o => o.ComponentId == 1)).Obtained);
            Assert.AreEqual(2, (await context.ObtainedComponents.FirstOrDefaultAsync(o => o.ComponentId == 2)).Obtained);
        }
        
        [Test]
        public async Task ReceiveComponentAsyncTest()
        {
            // Arrange
            var context = await GetInitDbContext();

            // Act
            var taskService = new TaskService(new EFUnitOfWork(context));
            await taskService.ReceiveComponentAsync(1, 1, 1);

            // Assert
            Assert.AreEqual(9, (await context.Components.FirstOrDefaultAsync(c => c.Id == 1)).Quantity);
            Assert.AreEqual(1, (await context.ObtainedComponents.FirstOrDefaultAsync(o => o.ComponentId == 1)).Obtained);
        }
        
        [Test]
        public async Task ReceiveDesignsAsyncTest()
        {
            // Arrange
            var context = await GetInitDbContext();

            // Act
            var taskService = new TaskService(new EFUnitOfWork(context));
            await taskService.ReceiveDesignsAsync(1, new[] {1, 2}, new []{1, 2});

            // Assert
            Assert.AreEqual(10, (await context.Designs.FirstOrDefaultAsync(d => d.Id == 1)).Quantity);
            Assert.AreEqual(20, (await context.Designs.FirstOrDefaultAsync(d => d.Id == 2)).Quantity);
            Assert.AreEqual(1, (await context.ObtainedDesigns.FirstOrDefaultAsync(o => o.DesignId == 1)).Obtained);
            Assert.AreEqual(2, (await context.ObtainedDesigns.FirstOrDefaultAsync(o => o.DesignId == 2)).Obtained);
        }
        
        [Test]
        public async Task ReceiveDesignAsyncTest()
        {
            // Arrange
            var context = await GetInitDbContext();

            // Act
            var taskService = new TaskService(new EFUnitOfWork(context));
            await taskService.ReceiveDesignAsync(1, 1, 1);

            // Assert
            Assert.AreEqual(10, (await context.Designs.FirstOrDefaultAsync(c => c.Id == 1)).Quantity);
            Assert.AreEqual(1, (await context.ObtainedDesigns.FirstOrDefaultAsync(o => o.DesignId == 1)).Obtained);
        }

        [TestCase(StatusEnum.Assembly | StatusEnum.Customization, true)]
        [TestCase(StatusEnum.Warehouse, true)]
        [TestCase(StatusEnum.Done, true)]
        [TestCase(StatusEnum.Equipment | StatusEnum.Validate | StatusEnum.Montage, true)]
        [TestCase(StatusEnum.Assembly | StatusEnum.Customization, false)]
        [TestCase(StatusEnum.Warehouse, false)]
        [TestCase(StatusEnum.Done, false)]
        [TestCase(StatusEnum.Equipment | StatusEnum.Validate | StatusEnum.Montage, false)]
        public async Task TransferTest(StatusEnum status, bool full)
        {
            // Arrange
            var context = await GetInitDbContext();

            // Act
            var taskService = new TaskService(new EFUnitOfWork(context));
            var currentStatus = (await context.Tasks.FirstOrDefaultAsync(c => c.Id == 1)).Status;
            await taskService.TransferAsync(1, full, (int)status, "empty");

            // Assert
            if (full)
            {
                Assert.AreEqual((int)status, (int)(await context.Tasks.FirstOrDefaultAsync(c => c.Id == 1)).Status);
            }
            else
            {
                Assert.AreEqual((int)(status | currentStatus), (int)(await context.Tasks.FirstOrDefaultAsync(c => c.Id == 1)).Status);
            }
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
            await context.SaveChangesAsync();
            return context;
        }
    }
}
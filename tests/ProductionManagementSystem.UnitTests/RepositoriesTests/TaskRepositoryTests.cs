using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Tasks;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Components;
using ProductionManagementSystem.Models.Devices;

namespace ProductionManagementSystem.UnitTests.RepositoriesTests
{
    public class TaskRepositoryTests : TestBase
    {
        [Test]
        public async System.Threading.Tasks.Task TaskRepository__GetTaskByIdAsync__GetResult()
        {
            // Arrange
            var options = GetDbContextOptions();

            await using (var context = GetDdContext(options))
            {
                context.Tasks.Add(new Task {Id = 1, Description = "name"});
                context.Tasks.Add(new Task {Id = 2, Description = "name2"});
                context.Tasks.Add(new Task {Id = 3, Description = "name3"});
                await context.SaveChangesAsync();
            }

            await using (var context = GetDdContext(options))
            {
                // Act
                var repository = new TaskRepository(context);
                var result = await repository.GetByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.AreEqual(1, result.Id);
                Assert.AreEqual("name", result.Description);
                Assert.AreEqual(null, result.OrderId);
            }


        }

        [Test]
        public async System.Threading.Tasks.Task TaskRepository__GetTaskByIdAsync__NotFound()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Tasks.Add(new Task {Id = 1, Description = "name"});
                context.Tasks.Add(new Task {Id = 2, Description = "name2"});
                context.Tasks.Add(new Task {Id = 3, Description = "name3"});
                await context.SaveChangesAsync();
            }

            await using (var context = GetDdContext(options))
            {
                // Act
                var repository = new TaskRepository(context);
                var result = await repository.GetByIdAsync(5);

                // Assert
                Assert.Null(result);
            }
        }

        [Test]
        public async System.Threading.Tasks.Task TaskRepository__GetTasks__GetResult()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Tasks.Add(new Task {Id = 1, Description = "name"});
                context.Tasks.Add(new Task {Id = 2, Description = "name2"});
                context.Tasks.Add(new Task {Id = 3, Description = "name3"});
                await context.SaveChangesAsync();
            }

            await using (var context = GetDdContext(options))
            {
                // Act
                var repository = new TaskRepository(context);
                var result = repository.GetAll().ToList();

                // Assert
                Assert.NotNull(result);
                Assert.AreEqual(3, result.Count());
                Assert.AreEqual(1, result[0].Id);
                Assert.AreEqual("name", result[0].Description);
                Assert.AreEqual(null, result[0].OrderId);
            }
        }

        [Test]
        public async System.Threading.Tasks.Task TaskRepository__GetTasks__EmptyDB()
        {
            var options = GetDbContextOptions();
            // Arrange

            await using var context = GetDdContext(options);
            // Act
            var repository = new TaskRepository(context);
            var result = repository.GetAll();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public async System.Threading.Tasks.Task TaskRepository__CreateTask__WithoutObtainedComponents()
        {
            var options = GetDbContextOptions();
            // Arrange

            await using var context = GetDdContext(options);
            // Act
            var repository = new TaskRepository(context);
            await repository.CreateAsync(new Task {Id = 1, Description = "name"});
            await repository.SaveAsync();

            // Assert
            Assert.IsNotEmpty(context.Tasks);
            var task = await context.Tasks.FirstOrDefaultAsync();
            Assert.NotNull(task);
            Assert.AreEqual(1, task.Id);
            Assert.AreEqual("name", task.Description);
            Assert.AreEqual(null, task.OrderId);
        }
        
        [Test]
        public async System.Threading.Tasks.Task TaskRepository__CreateTask__WithObtainedComponents()
        {
            var options = GetDbContextOptions();
            // Arrange

            await using var context = GetDdContext(options);
            await InitDbContext(context);
            // Act
            var repository = new TaskRepository(context);
            await repository.CreateAsync(new Task {Id = 2, Description = "name", DeviceId = 1,
                ObtainedDesigns = new ObtainedDesign[1]
                {
                    new ObtainedDesign() {ComponentId = 1, TaskId = 2, Obtained = 10}
                }, 
                ObtainedMontages = new ObtainedMontage[1]
                {
                    new ObtainedMontage() {ComponentId = 1, TaskId = 2}
                }});
            await repository.SaveAsync();

            // Assert
            Assert.IsNotEmpty(context.Tasks);
            var task = await context.Tasks.FindAsync(2);
            Assert.NotNull(task);
            Assert.AreEqual(2, task.Id);
            Assert.AreEqual("name", task.Description);
            Assert.AreEqual(1, task.ObtainedMontages.Count());
            Assert.AreEqual(10, task.ObtainedDesigns.First().Obtained);
        }

        [Test]
        public async System.Threading.Tasks.Task TaskRepository__UpdateTask__WithoutObtainedComponents()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Tasks.Add(new Task {Id = 1, Description = "name"});
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = GetDdContext(options))
            {
                var repository = new TaskRepository(context);
                repository.Update(new Task {Id = 1, Description = "name123", Status = TaskStatusEnum.Done});
                await repository.SaveAsync();
                var montage = await context.Tasks.FirstOrDefaultAsync();

                // Assert
                Assert.NotNull(montage);
                Assert.AreEqual(1, montage.Id);
                Assert.AreEqual("name123", montage.Description);
                Assert.AreEqual(TaskStatusEnum.Done, montage.Status);
            }
        }
        
        [Test]
        public async System.Threading.Tasks.Task TaskRepository__UpdateTask__WithObtainedComponents()
        {
            var options = GetDbContextOptions();
            // Arrange

            await using (var context = GetDdContext(options))
            {
                await InitDbContext(context);
            }
            
            // Act
            await using (var context = GetDdContext(options))
            {
                var repository = new TaskRepository(context);
                repository.Update(new Task
                {
                    Id = 1, Description = "newName", DeviceId = 1,
                    ObtainedDesigns = new ObtainedDesign[1]
                    {
                        new ObtainedDesign() {ComponentId = 1, TaskId = 2, Obtained = 10}
                    },
                    ObtainedMontages = new ObtainedMontage[1]
                    {
                        new ObtainedMontage() {ComponentId = 1, TaskId = 2}
                    }
                });
                await repository.SaveAsync();

                // Assert
                Assert.IsNotEmpty(context.Tasks);
                var task = await context.Tasks.FindAsync(1);
                Assert.NotNull(task);
                Assert.AreEqual(1, task.Id);
                Assert.AreEqual("newName", task.Description);
                Assert.AreEqual(1, task.ObtainedMontages.Count());
                Assert.AreEqual(10, task.ObtainedDesigns.First().Obtained);
            }
        }

        [Test]
        public async System.Threading.Tasks.Task TaskRepository__DeleteTask__WithoutObtainedComponents()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Tasks.Add(new Task {Id = 1, Description = "name"});
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = GetDdContext(options))
            {
                var repository = new TaskRepository(context);
                repository.Delete(new Task {Id = 1});
                await repository.SaveAsync();
                var montage = await context.Tasks.FirstOrDefaultAsync();

                // Assert
                Assert.Null(montage);
            }
        }
        
        [Test]
        public async System.Threading.Tasks.Task TaskRepository__DeleteTask__WithObtainedComponents()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                await InitDbContext(context);
            }

            // Act
            await using (var context = GetDdContext(options))
            {
                var repository = new TaskRepository(context);
                repository.Delete(new Task {Id = 1});
                await repository.SaveAsync();
                var task = await context.Tasks.FirstOrDefaultAsync();
                var obtainedMontage = await context.ObtainedMontages.FirstOrDefaultAsync();
                var obtainedDesign = await context.ObtainedDesigns.FirstOrDefaultAsync();

                // Assert
                Assert.Null(task);
                Assert.Null(obtainedDesign);
                Assert.Null(obtainedMontage);
            }
        }
        
        private async System.Threading.Tasks.Task InitDbContext(ApplicationContext context)
        {
            await context.Designs.AddAsync(new Design {Id = 1, Name = "name", Quantity = 10});
            await context.Designs.AddAsync(new Design {Id = 2, Name = "name2", Quantity = 20});
            await context.Designs.AddAsync(new Design {Id = 3, Name = "name3", Quantity = 30});
            await context.Montages.AddAsync(new Montage {Id = 1, Name = "mname", Quantity = 10});
            await context.Montages.AddAsync(new Montage {Id = 2, Name = "mname2", Quantity = 20});
            await context.Montages.AddAsync(new Montage {Id = 3, Name = "mname3", Quantity = 30});
            await context.Devices.AddAsync(new Device {Id = 1, Name = "name", Quantity = 1});
            await context.DesignInDevices.AddAsync(new DesignInDevice() {DeviceId = 1, ComponentId = 1, Quantity = 5});
            await context.DesignInDevices.AddAsync(new DesignInDevice() {DeviceId = 1, ComponentId = 2, Quantity = 10});
            await context.MontageInDevices.AddAsync(new MontageInDevice() {DeviceId = 1, ComponentId = 1, Quantity = 15});
            await context.Tasks.AddAsync(new Task()
                {Id = 1, DeviceId = 1, Deadline = DateTime.Now, StartTime = DateTime.Now, EndTime = DateTime.Now});
            await context.ObtainedMontages.AddAsync(new ObtainedMontage() {TaskId = 1, ComponentId = 1, Obtained = 0});
            await context.ObtainedDesigns.AddAsync(new ObtainedDesign() {TaskId = 1, ComponentId = 1, Obtained = 0});
            await context.ObtainedDesigns.AddAsync(new ObtainedDesign() {TaskId = 1, ComponentId = 2, Obtained = 0});
            await context.SaveChangesAsync();
        }
    }
}
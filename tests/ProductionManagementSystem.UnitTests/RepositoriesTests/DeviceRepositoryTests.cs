using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Components;
using ProductionManagementSystem.Models.Devices;

namespace ProductionManagementSystem.UnitTests.RepositoriesTests
{
    public class DeviceRepositoryTests : TestBase
    {
        [Test]
        public async Task DeviceRepository__GetDeviceByIdAsync__GetResult()
        {
            // Arrange
            var options = GetDbContextOptions();

            await using (var context = GetDdContext(options))
            {
                context.Designs.Add(new Design {Id = 1, Name = "dname", Quantity = 11});
                context.Designs.Add(new Design {Id = 2, Name = "dname2", Quantity = 22});
                context.Designs.Add(new Design {Id = 3, Name = "dname3", Quantity = 33});
                context.Montages.Add(new Montage {Id = 1, Name = "mname", Quantity = 10});
                context.Montages.Add(new Montage {Id = 2, Name = "mname2", Quantity = 20});
                context.Montages.Add(new Montage {Id = 3, Name = "mname3", Quantity = 30});
                context.Devices.Add(new Device {Id = 1, Name = "name", Quantity = 1});
                context.Devices.Add(new Device {Id = 2, Name = "name2", Quantity = 2});
                context.DesignInDevices.Add(new DesignInDevice() {DeviceId = 1, ComponentId = 1, Quantity = 5});
                context.DesignInDevices.Add(new DesignInDevice() {DeviceId = 1, ComponentId = 2, Quantity = 10});
                context.MontageInDevices.Add(new MontageInDevice() {DeviceId = 1, ComponentId = 1, Quantity = 15});
                context.MontageInDevices.Add(new MontageInDevice() {DeviceId = 1, ComponentId = 2, Quantity = 20});
                await context.SaveChangesAsync();
            }

            await using (var context = GetDdContext(options))
            {
                // Act
                var repository = new DeviceRepository(context);
                var result = await repository.GetByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.AreEqual(1, result.Id);
                Assert.AreEqual("name", result.Name);
                Assert.AreEqual(1, result.Quantity);
                Assert.AreEqual(null, result.Description);
                Assert.AreEqual(15, result.Montage.First().Quantity);
                Assert.AreEqual(2, result.Montage.Count());
                Assert.AreEqual(5, result.Designs.First().Quantity);
                Assert.AreEqual(2, result.Designs.Count());
            }


        }

        [Test]
        public async Task DeviceRepository__GetDeviceByIdAsync__NotFound()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Designs.Add(new Design {Id = 1, Name = "dname", Quantity = 11});
                context.Designs.Add(new Design {Id = 2, Name = "dname2", Quantity = 22});
                context.Designs.Add(new Design {Id = 3, Name = "dname3", Quantity = 33});
                context.Montages.Add(new Montage {Id = 1, Name = "mname", Quantity = 10});
                context.Montages.Add(new Montage {Id = 2, Name = "mname2", Quantity = 20});
                context.Montages.Add(new Montage {Id = 3, Name = "mname3", Quantity = 30});
                context.Devices.Add(new Device {Id = 1, Name = "name", Quantity = 1});
                context.Devices.Add(new Device {Id = 2, Name = "name2", Quantity = 2});
                context.DesignInDevices.Add(new DesignInDevice() {DeviceId = 1, ComponentId = 1, Quantity = 5});
                context.DesignInDevices.Add(new DesignInDevice() {DeviceId = 1, ComponentId = 2, Quantity = 10});
                context.MontageInDevices.Add(new MontageInDevice() {DeviceId = 1, ComponentId = 1, Quantity = 15});
                context.MontageInDevices.Add(new MontageInDevice() {DeviceId = 1, ComponentId = 2, Quantity = 20});
                await context.SaveChangesAsync();
            }

            await using (var context = GetDdContext(options))
            {
                // Act
                var repository = new DeviceRepository(context);
                var result = await repository.GetByIdAsync(5);

                // Assert
                Assert.Null(result);
            }
        }

        [Test]
        public async Task DeviceRepository__GetDevices__GetResult()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Designs.Add(new Design {Id = 1, Name = "dname", Quantity = 11});
                context.Designs.Add(new Design {Id = 2, Name = "dname2", Quantity = 22});
                context.Designs.Add(new Design {Id = 3, Name = "dname3", Quantity = 33});
                context.Montages.Add(new Montage {Id = 1, Name = "mname", Quantity = 10});
                context.Montages.Add(new Montage {Id = 2, Name = "mname2", Quantity = 20});
                context.Montages.Add(new Montage {Id = 3, Name = "mname3", Quantity = 30});
                context.Devices.Add(new Device {Id = 1, Name = "name", Quantity = 1});
                context.Devices.Add(new Device {Id = 2, Name = "name2", Quantity = 2});
                context.DesignInDevices.Add(new DesignInDevice() {DeviceId = 1, ComponentId = 1, Quantity = 5});
                context.DesignInDevices.Add(new DesignInDevice() {DeviceId = 1, ComponentId = 2, Quantity = 10});
                context.MontageInDevices.Add(new MontageInDevice() {DeviceId = 1, ComponentId = 1, Quantity = 15});
                context.MontageInDevices.Add(new MontageInDevice() {DeviceId = 1, ComponentId = 2, Quantity = 20});
                await context.SaveChangesAsync();
            }

            await using (var context = GetDdContext(options))
            {
                // Act
                var repository = new DeviceRepository(context);
                var result = repository.GetAll().ToList();

                // Assert
                Assert.NotNull(result);
                Assert.AreEqual(2, result.Count());
                Assert.AreEqual("name", result[0].Name);
                Assert.AreEqual(1, result[0].Quantity);
                Assert.AreEqual(null, result[0].Description);
                Assert.AreEqual(15, result[0].Montage.First().Quantity);
                Assert.AreEqual(2, result[0].Montage.Count());
                Assert.AreEqual(5, result[0].Designs.First().Quantity);
                Assert.AreEqual(2, result[0].Designs.Count());
            }
        }

        [Test]
        public async Task DeviceRepository__GetDevices__EmptyDB()
        {
            var options = GetDbContextOptions();
            // Arrange

            await using var context = GetDdContext(options);
            // Act
            var repository = new DeviceRepository(context);
            var result = repository.GetAll();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task DeviceRepository__CreateDevice__WithoutComponents()
        {
            var options = GetDbContextOptions();
            // Arrange

            await using var context = GetDdContext(options);
            // Act
            var repository = new DeviceRepository(context);
            await repository.CreateAsync(new Device {Id = 1, Name = "name", Quantity = 10});
            await repository.SaveAsync();

            // Assert
            Assert.IsNotEmpty(context.Devices);
            var montage = await context.Devices.FirstOrDefaultAsync();
            Assert.NotNull(montage);
            Assert.AreEqual(1, montage.Id);
            Assert.AreEqual("name", montage.Name);
            Assert.AreEqual(10, montage.Quantity);
            Assert.AreEqual(null, montage.Description);
        }
        
        [Test]
        public async Task DeviceRepository__CreateDevice__WithComponents()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Designs.Add(new Design {Id = 1, Name = "name", Quantity = 10});
                context.Designs.Add(new Design {Id = 2, Name = "name2", Quantity = 20});
                context.Designs.Add(new Design {Id = 3, Name = "name3", Quantity = 30});
                context.Montages.Add(new Montage {Id = 1, Name = "mname", Quantity = 10});
                context.Montages.Add(new Montage {Id = 2, Name = "mname2", Quantity = 20});
                context.Montages.Add(new Montage {Id = 3, Name = "mname3", Quantity = 30});
                await context.SaveChangesAsync();
            }

            await using (var context = GetDdContext(options))
            {
                // Act
                var repository = new DeviceRepository(context);
                await repository.CreateAsync(new Device
                {
                    Id = 1, Name = "name", Quantity = 10,
                    Montage = new List<MontageInDevice>()
                    {
                        new MontageInDevice() {ComponentId = 1, Quantity = 2, DeviceId = 1},
                        new MontageInDevice() {ComponentId = 2, Quantity = 4, DeviceId = 1},
                    },
                    Designs = new List<DesignInDevice>()
                    {
                        new DesignInDevice() {ComponentId = 1, Quantity = 6, DeviceId = 1},
                    }
                });
                await repository.SaveAsync();

                // Assert
                Assert.IsNotEmpty(context.Devices);
                var device = await context.Devices.FirstOrDefaultAsync();
                var montageInDevice = await context.MontageInDevices.FirstOrDefaultAsync();
                var designInDevice = await context.DesignInDevices.FirstOrDefaultAsync();
                Assert.NotNull(device);
                Assert.AreEqual(1, device.Id);
                Assert.AreEqual("name", device.Name);
                Assert.AreEqual(10, device.Quantity);
                Assert.AreEqual(null, device.Description);
                Assert.AreEqual(1, montageInDevice.ComponentId);
                Assert.AreEqual(2, montageInDevice.Quantity);
                Assert.AreEqual(1, montageInDevice.DeviceId);
                Assert.AreEqual(1, designInDevice.ComponentId);
                Assert.AreEqual(6, designInDevice.Quantity);
                Assert.AreEqual(1, designInDevice.DeviceId);
                Assert.AreEqual(1, context.DesignInDevices.Count());
                Assert.AreEqual(2, context.MontageInDevices.Count());
            }
        }

        [Test]
        public async Task DeviceRepository__UpdateDevice__WithoutComponents()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Devices.Add(new Device {Id = 1, Name = "name", Quantity = 10});
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = GetDdContext(options))
            {
                var repository = new DeviceRepository(context);
                repository.Update(new Device {Id = 1, Name = "name123", Quantity = 15, Description = "nominal"});
                await repository.SaveAsync();
                var montage = await context.Devices.FirstOrDefaultAsync();

                // Assert
                Assert.NotNull(montage);
                Assert.AreEqual(1, montage.Id);
                Assert.AreEqual("name123", montage.Name);
                Assert.AreEqual(15, montage.Quantity);
                Assert.AreEqual("nominal", montage.Description);
            }
        }
        
        [Test]
        public async Task DeviceRepository__UpdateDevice__WithComponents()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Designs.Add(new Design {Id = 1, Name = "name", Quantity = 10});
                context.Designs.Add(new Design {Id = 2, Name = "name2", Quantity = 20});
                context.Designs.Add(new Design {Id = 3, Name = "name3", Quantity = 30});
                context.Montages.Add(new Montage {Id = 1, Name = "mname", Quantity = 10});
                context.Montages.Add(new Montage {Id = 2, Name = "mname2", Quantity = 20});
                context.Montages.Add(new Montage {Id = 3, Name = "mname3", Quantity = 30});
                context.Devices.Add(new Device {Id = 1, Name = "name", Quantity = 1});
                context.Devices.Add(new Device {Id = 2, Name = "name2", Quantity = 2});
                context.DesignInDevices.Add(new DesignInDevice() {DeviceId = 1, ComponentId = 1, Quantity = 5});
                context.DesignInDevices.Add(new DesignInDevice() {DeviceId = 1, ComponentId = 2, Quantity = 10});
                context.MontageInDevices.Add(new MontageInDevice() {DeviceId = 1, ComponentId = 1, Quantity = 15});
                context.MontageInDevices.Add(new MontageInDevice() {DeviceId = 1, ComponentId = 2, Quantity = 20});
                await context.SaveChangesAsync();
            }

            await using (var context = GetDdContext(options))
            {
                // Act
                var repository = new DeviceRepository(context);
                repository.Update(new Device
                {
                    Id = 1, Name = "name", Quantity = 10,
                    Montage = new List<MontageInDevice>()
                    {
                        new MontageInDevice() {Id = 1, ComponentId = 1, Quantity = 2, DeviceId = 1},
                        new MontageInDevice() {Id = 2, ComponentId = 2, Quantity = 4, DeviceId = 1},
                    },
                    Designs = new List<DesignInDevice>()
                    {
                        new DesignInDevice() {Id = 1, ComponentId = 1, Quantity = 6, DeviceId = 1},
                    }
                });
                await repository.SaveAsync();

                // Assert
                Assert.IsNotEmpty(context.Devices);
                var device = await context.Devices.FirstOrDefaultAsync();
                var montageInDevice = await context.MontageInDevices.FirstOrDefaultAsync();
                var designInDevice = await context.DesignInDevices.FirstOrDefaultAsync();
                Assert.NotNull(device);
                Assert.AreEqual(1, device.Id);
                Assert.AreEqual("name", device.Name);
                Assert.AreEqual(10, device.Quantity);
                Assert.AreEqual(null, device.Description);
                Assert.AreEqual(1, montageInDevice.ComponentId);
                Assert.AreEqual(2, montageInDevice.Quantity);
                Assert.AreEqual(1, montageInDevice.DeviceId);
                Assert.AreEqual(1, designInDevice.ComponentId);
                Assert.AreEqual(6, designInDevice.Quantity);
                Assert.AreEqual(1, designInDevice.DeviceId);
                Assert.AreEqual(1, context.DesignInDevices.Count());
                Assert.AreEqual(2, context.MontageInDevices.Count());
            }
        }

        [Test]
        public async Task DeviceRepository__DeleteDevice__WithoutComponents()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Devices.Add(new Device {Id = 1, Name = "name", Quantity = 10});
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = GetDdContext(options))
            {
                var repository = new DeviceRepository(context);
                repository.Delete(new Device {Id = 1});
                await repository.SaveAsync();
                var montage = await context.Devices.FirstOrDefaultAsync();

                // Assert
                Assert.Null(montage);
            }
        }
        
        [Test]
        public async Task DeviceRepository__DeleteDevice__WithComponents()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Designs.Add(new Design {Id = 1, Name = "name", Quantity = 10});
                context.Designs.Add(new Design {Id = 2, Name = "name2", Quantity = 20});
                context.Designs.Add(new Design {Id = 3, Name = "name3", Quantity = 30});
                context.Montages.Add(new Montage {Id = 1, Name = "mname", Quantity = 10});
                context.Montages.Add(new Montage {Id = 2, Name = "mname2", Quantity = 20});
                context.Montages.Add(new Montage {Id = 3, Name = "mname3", Quantity = 30});
                context.Devices.Add(new Device {Id = 1, Name = "name", Quantity = 1});
                context.DesignInDevices.Add(new DesignInDevice() {DeviceId = 1, ComponentId = 1, Quantity = 5});
                context.DesignInDevices.Add(new DesignInDevice() {DeviceId = 1, ComponentId = 2, Quantity = 10});
                context.MontageInDevices.Add(new MontageInDevice() {DeviceId = 1, ComponentId = 1, Quantity = 15});
                await context.SaveChangesAsync();
            }

            await using (var context = GetDdContext(options))
            {
                // Act
                var repository = new DeviceRepository(context);
                repository.Delete(new Device
                {
                    Id = 1
                });
                await repository.SaveAsync();

                // Assert
                var device = await context.Devices.FirstOrDefaultAsync();
                var montageInDevice = await context.MontageInDevices.FirstOrDefaultAsync();
                var designInDevice = await context.DesignInDevices.FirstOrDefaultAsync();
                Assert.Null(device);
                Assert.Null(montageInDevice);
                Assert.Null(designInDevice);
            }
        }
    }
}
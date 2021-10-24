using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.UnitTests.RepositoriesTests
{
    public class DesignRepositoryTests : TestBase
    {
        [Test]
        public async Task DesignRepository__GetDesignByIdAsync__GetResult()
        {
            // Arrange
            var options = GetDbContextOptions();

            await using (var context = GetDdContext(options))
            {
                context.Designs.Add(new Design {Id = 1, Name = "name", Quantity = 10});
                context.Designs.Add(new Design {Id = 2, Name = "name2", Quantity = 20});
                context.Designs.Add(new Design {Id = 3, Name = "name3", Quantity = 30});
                await context.SaveChangesAsync();
            }
            
            await using (var context = GetDdContext(options))
            {
                // Act
                var montageRepository = new DesignRepository(context);
                var result = await montageRepository.GetByIdAsync(1);
                
                // Assert
                Assert.NotNull(result);
                Assert.AreEqual(1, result.Id);
                Assert.AreEqual("name", result.Name);
                Assert.AreEqual(10, result.Quantity);
                Assert.AreEqual(null, result.Description);
            }

            
        }
        
        [Test]
        public async Task DesignRepository__GetDesignByIdAsync__NotFound()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Designs.Add(new Design {Id = 1, Name = "name", Quantity = 10});
                context.Designs.Add(new Design {Id = 2, Name = "name2", Quantity = 20});
                context.Designs.Add(new Design {Id = 3, Name = "name3", Quantity = 30});
                await context.SaveChangesAsync();
            }

            await using (var context = GetDdContext(options))
            {
                // Act
                var montageRepository = new DesignRepository(context);
                var result = await montageRepository.GetByIdAsync(5);

                // Assert
                Assert.Null(result);
            }
        }
        
        [Test]
        public async Task DesignRepository__GetDesigns__GetResult()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Designs.Add(new Design {Id = 1, Name = "name", Quantity = 10});
                context.Designs.Add(new Design {Id = 2, Name = "name2", Quantity = 20});
                context.Designs.Add(new Design {Id = 3, Name = "name3", Quantity = 30});
                await context.SaveChangesAsync();
            }

            await using (var context = GetDdContext(options))
            {
                // Act
                var montageRepository = new DesignRepository(context);
                var result = montageRepository.GetAll().ToList();

                // Assert
                Assert.NotNull(result);
                Assert.AreEqual(3, result.Count());
                Assert.AreEqual(1, result[0].Id);
                Assert.AreEqual("name", result[0].Name);
                Assert.AreEqual(10, result[0].Quantity);
                Assert.AreEqual(null, result[0].Description);
            }
        }
        
        [Test]
        public async Task DesignRepository__GetDesigns__EmptyDB()
        {
            var options = GetDbContextOptions();
            // Arrange

            await using var context = GetDdContext(options);
            // Act
            var montageRepository = new DesignRepository(context);
            var result = montageRepository.GetAll();

            // Assert
            Assert.IsEmpty(result);
        }
        
        [Test]
        public async Task DesignRepository__CreateDesign()
        {
            var options = GetDbContextOptions();
            // Arrange

            await using var context = GetDdContext(options);
            // Act
            var montageRepository = new DesignRepository(context);
            await montageRepository.CreateAsync(new Design {Id = 1, Name = "name", Quantity = 10});
            await montageRepository.SaveAsync();

            // Assert
            Assert.IsNotEmpty(context.Designs);
            var montage = await context.Designs.FirstOrDefaultAsync();
            Assert.NotNull(montage);
            Assert.AreEqual(1, montage.Id);
            Assert.AreEqual("name", montage.Name);
            Assert.AreEqual(10, montage.Quantity);
            Assert.AreEqual(null, montage.Description);
        }
        
        [Test]
        public async Task DesignRepository__UpdateDesign()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Designs.Add(new Design {Id = 1, Name = "name", Quantity = 10});
                await context.SaveChangesAsync();
            }
            
            // Act
            await using (var context = GetDdContext(options))
            {
                var montageRepository = new DesignRepository(context);
                montageRepository.Update(new Design {Id = 1, Name = "name123", Quantity = 15, Description = "nominal"});
                await montageRepository.SaveAsync();
                var montage = await context.Designs.FirstOrDefaultAsync();

                // Assert
                Assert.NotNull(montage);
                Assert.AreEqual(1, montage.Id);
                Assert.AreEqual("name123", montage.Name);
                Assert.AreEqual(15, montage.Quantity);
                Assert.AreEqual("nominal", montage.Description);
            }
        }
        
        [Test]
        public async Task DesignRepository__DeleteDesign()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Designs.Add(new Design {Id = 1, Name = "name", Quantity = 10});
                await context.SaveChangesAsync();
            }
            
            // Act
            await using (var context = GetDdContext(options))
            {
                var montageRepository = new DesignRepository(context);
                montageRepository.Delete(new Design {Id = 1});
                await montageRepository.SaveAsync();
                var montage = await context.Designs.FirstOrDefaultAsync();

                // Assert
                Assert.Null(montage);
            }
        }
    }
}
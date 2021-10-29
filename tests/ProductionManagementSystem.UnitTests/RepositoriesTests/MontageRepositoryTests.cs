using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.UnitTests.RepositoriesTests
{
    public class MontageRepositoryTests : TestBase
    {
        [Test]
        public async Task MontageRepository__GetMontageByIdAsync__GetResult()
        {
            // Arrange
            var options = GetDbContextOptions();

            await using (var context = GetDdContext(options))
            {
                context.Montages.Add(new Montage {Id = 1, Name = "name", Quantity = 10});
                context.Montages.Add(new Montage {Id = 2, Name = "name2", Quantity = 20});
                context.Montages.Add(new Montage {Id = 3, Name = "name3", Quantity = 30});
                await context.SaveChangesAsync();
            }
            
            await using (var context = GetDdContext(options))
            {
                // Act
                var montageRepository = new MontageRepository(context);
                var result = await montageRepository.GetByIdAsync(1);
                
                // Assert
                Assert.NotNull(result);
                Assert.AreEqual(1, result.Id);
                Assert.AreEqual("name", result.Name);
                Assert.AreEqual(10, result.Quantity);
                Assert.AreEqual(null, result.Nominal);
            }

            
        }
        
        [Test]
        public async Task MontageRepository__GetMontageByIdAsync__NotFound()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Montages.Add(new Montage {Id = 1, Name = "name", Quantity = 10});
                context.Montages.Add(new Montage {Id = 2, Name = "name2", Quantity = 20});
                context.Montages.Add(new Montage {Id = 3, Name = "name3", Quantity = 30});
                await context.SaveChangesAsync();
            }

            await using (var context = GetDdContext(options))
            {
                // Act
                var montageRepository = new MontageRepository(context);
                var result = await montageRepository.GetByIdAsync(5);

                // Assert
                Assert.Null(result);
            }
        }
        
        [Test]
        public async Task MontageRepository__GetMontages__GetResult()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Montages.Add(new Montage {Id = 1, Name = "name", Quantity = 10});
                context.Montages.Add(new Montage {Id = 2, Name = "name2", Quantity = 20});
                context.Montages.Add(new Montage {Id = 3, Name = "name3", Quantity = 30});
                await context.SaveChangesAsync();
            }

            await using (var context = GetDdContext(options))
            {
                // Act
                var repository = new MontageRepository(context);
                var result = (await repository.GetAllAsync()).ToList();

                // Assert
                Assert.NotNull(result);
                Assert.AreEqual(3, result.Count());
                Assert.AreEqual(1, result[0].Id);
                Assert.AreEqual("name", result[0].Name);
                Assert.AreEqual(10, result[0].Quantity);
                Assert.AreEqual(null, result[0].Nominal);
            }
        }
        
        [Test]
        public async Task MontageRepository__GetMontages__EmptyDB()
        {
            var options = GetDbContextOptions();
            // Arrange

            await using var context = GetDdContext(options);
            // Act
            var montageRepository = new MontageRepository(context);
            var result = await montageRepository.GetAllAsync();

            // Assert
            Assert.IsEmpty(result);
        }
        
        [Test]
        public async Task MontageRepository__CreateMontage()
        {
            var options = GetDbContextOptions();
            // Arrange

            await using var context = GetDdContext(options);
            // Act
            var montageRepository = new MontageRepository(context);
            await montageRepository.CreateAsync(new Montage {Id = 1, Name = "name", Quantity = 10});
            await montageRepository.SaveAsync();

            // Assert
            Assert.IsNotEmpty(context.Montages);
            var montage = await context.Montages.FirstOrDefaultAsync();
            Assert.NotNull(montage);
            Assert.AreEqual(1, montage.Id);
            Assert.AreEqual("name", montage.Name);
            Assert.AreEqual(10, montage.Quantity);
            Assert.AreEqual(null, montage.Nominal);
        }
        
        [Test]
        public async Task MontageRepository__UpdateMontage()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Montages.Add(new Montage {Id = 1, Name = "name", Quantity = 10});
                await context.SaveChangesAsync();
            }
            
            // Act
            await using (var context = GetDdContext(options))
            {
                var montageRepository = new MontageRepository(context);
                montageRepository.Update(new Montage {Id = 1, Name = "name123", Quantity = 15, Nominal = "nominal"});
                await montageRepository.SaveAsync();
                var montage = await context.Montages.FirstOrDefaultAsync();

                // Assert
                Assert.NotNull(montage);
                Assert.AreEqual(1, montage.Id);
                Assert.AreEqual("name123", montage.Name);
                Assert.AreEqual(15, montage.Quantity);
                Assert.AreEqual("nominal", montage.Nominal);
            }
        }
        
        [Test]
        public async Task MontageRepository__DeleteMontage()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Montages.Add(new Montage {Id = 1, Name = "name", Quantity = 10});
                await context.SaveChangesAsync();
            }
            
            // Act
            await using (var context = GetDdContext(options))
            {
                var montageRepository = new MontageRepository(context);
                montageRepository.Delete(new Montage {Id = 1});
                await montageRepository.SaveAsync();
                var montage = await context.Montages.FirstOrDefaultAsync();

                // Assert
                Assert.Null(montage);
            }
        }
    }
}
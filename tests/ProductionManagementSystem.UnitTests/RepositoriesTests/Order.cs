using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Orders;

namespace ProductionManagementSystem.UnitTests.RepositoriesTests
{
    public class OrderRepositoryTests : TestBase
    {
        [Test]
        public async Task OrderRepository__GetOrderByIdAsync__GetResult()
        {
            // Arrange
            var options = GetDbContextOptions();

            await using (var context = GetDdContext(options))
            {
                context.Orders.Add(new Order {Id = 1, Customer = "name"});
                context.Orders.Add(new Order {Id = 2, Customer = "name2"});
                context.Orders.Add(new Order {Id = 3, Customer = "name3"});
                await context.SaveChangesAsync();
            }

            await using (var context = GetDdContext(options))
            {
                // Act
                var repository = new OrderRepository(context);
                var result = await repository.GetByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.AreEqual(1, result.Id);
                Assert.AreEqual("name", result.Customer);
                Assert.AreEqual(null, result.Description);
            }


        }

        [Test]
        public async Task OrderRepository__GetOrderByIdAsync__NotFound()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Orders.Add(new Order {Id = 1, Customer = "name"});
                context.Orders.Add(new Order {Id = 2, Customer = "name2"});
                context.Orders.Add(new Order {Id = 3, Customer = "name3"});
                await context.SaveChangesAsync();
            }

            await using (var context = GetDdContext(options))
            {
                // Act
                var repository = new OrderRepository(context);
                var result = await repository.GetByIdAsync(5);

                // Assert
                Assert.Null(result);
            }
        }

        [Test]
        public async Task OrderRepository__GetOrders__GetResult()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Orders.Add(new Order {Id = 1, Customer = "name"});
                context.Orders.Add(new Order {Id = 2, Customer = "name2"});
                context.Orders.Add(new Order {Id = 3, Customer = "name3"});
                await context.SaveChangesAsync();
            }

            await using (var context = GetDdContext(options))
            {
                // Act
                var repository = new OrderRepository(context);
                var result = repository.GetAll().ToList();

                // Assert
                Assert.NotNull(result);
                Assert.AreEqual(3, result.Count());
                Assert.AreEqual(1, result[0].Id);
                Assert.AreEqual("name", result[0].Customer);
                Assert.AreEqual(null, result[0].Description);
            }
        }

        [Test]
        public async Task OrderRepository__GetOrders__EmptyDB()
        {
            var options = GetDbContextOptions();
            // Arrange

            await using var context = GetDdContext(options);
            // Act
            var repository = new OrderRepository(context);
            var result = repository.GetAll();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task OrderRepository__CreateOrder()
        {
            var options = GetDbContextOptions();
            // Arrange

            await using var context = GetDdContext(options);
            // Act
            var repository = new OrderRepository(context);
            await repository.CreateAsync(new Order {Id = 1, Customer = "name"});
            await repository.SaveAsync();

            // Assert
            Assert.IsNotEmpty(context.Orders);
            var order = await context.Orders.FirstOrDefaultAsync();
            Assert.NotNull(order);
            Assert.AreEqual(1, order.Id);
            Assert.AreEqual("name", order.Customer);
            Assert.AreEqual(null, order.Description);
        }

        [Test]
        public async Task OrderRepository__UpdateOrder()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Orders.Add(new Order {Id = 1, Customer = "name"});
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = GetDdContext(options))
            {
                var repository = new OrderRepository(context);
                repository.Update(new Order {Id = 1, Customer = "name123", Description = "nominal"});
                await repository.SaveAsync();
                var order = await context.Orders.FirstOrDefaultAsync();

                // Assert
                Assert.NotNull(order);
                Assert.AreEqual(1, order.Id);
                Assert.AreEqual("name123", order.Customer);
                Assert.AreEqual("nominal", order.Description);
            }
        }

        [Test]
        public async Task OrderRepository__DeleteOrder()
        {
            var options = GetDbContextOptions();
            // Arrange
            await using (var context = GetDdContext(options))
            {
                context.Orders.Add(new Order {Id = 1, Customer = "name"});
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = GetDdContext(options))
            {
                var repository = new OrderRepository(context);
                repository.Delete(new Order {Id = 1});
                await repository.SaveAsync();
                var order = await context.Orders.FirstOrDefaultAsync();

                // Assert
                Assert.Null(order);
            }
        }
    }
}
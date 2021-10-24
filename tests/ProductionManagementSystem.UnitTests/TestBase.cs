using System;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;

namespace ProductionManagementSystem.UnitTests
{
    public abstract class TestBase
    {
        protected DbContextOptions<ApplicationContext> GetDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }
        
        protected ApplicationContext GetDdContext(DbContextOptions<ApplicationContext> options) => new ApplicationContext(options);
    }   
}
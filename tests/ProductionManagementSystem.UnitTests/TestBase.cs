using System;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;

namespace ProductionManagementSystem.UnitTests
{
    public abstract class TestBase
    {
        protected ApplicationContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationContext(options);
        }
    }   
}
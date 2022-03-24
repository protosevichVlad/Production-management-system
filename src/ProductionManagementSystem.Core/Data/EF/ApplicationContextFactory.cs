using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProductionManagementSystem.Core.Data.EF
{
    public class ApplicationContextFactory: IDesignTimeDbContextFactory<ApplicationContext>
    {
        public ApplicationContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            if (args.Length == 1)
            {
                optionsBuilder.UseMySql(args[0], new MySqlServerVersion(new Version(8, 0, 24)));
            }
            else
            {
                optionsBuilder.UseMySql("Server=localhost;Database=master;User=user1;Password=123Pass123;", new MySqlServerVersion(new Version(8, 0, 24)));
            }

            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}
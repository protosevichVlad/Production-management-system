using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProductionManagementSystem.DAL.EF
{
    public class ApplicationContextFactory: IDesignTimeDbContextFactory<ApplicationContext>
    {
        public ApplicationContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            if (args.Length == 1)
            {
                // optionsBuilder.UseSqlServer(args[0]);
                optionsBuilder.UseMySql(args[0], new MySqlServerVersion(new Version(8, 0, 24)));
            }
            else
            {
                optionsBuilder.UseMySql("Server=127.0.0.1:3306;Database=master;User=user1;Password=123Pass123;", new MySqlServerVersion(new Version(8, 0, 24)));
                // optionsBuilder.UseSqlServer("Server=tcp:productionmanagementsystem.database.windows.net,1433;Initial Catalog=productionmanagementsystem;Persist Security Info=False;User ID=user1;Password=123Pass123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }

            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}
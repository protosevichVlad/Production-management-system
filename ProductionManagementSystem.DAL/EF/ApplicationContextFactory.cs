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
                optionsBuilder.UseSqlServer(args[0]);
            }
            else
            {
                optionsBuilder.UseSqlServer("Server=tcp:productionmanagementsystem.database.windows.net,1433;Initial Catalog=productionmanagementsystem;Persist Security Info=False;User ID=user1;Password=123Pass123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }

            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}
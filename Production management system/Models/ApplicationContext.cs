using Microsoft.EntityFrameworkCore;

namespace ProductionManagementSystem.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Device> Devices { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<Design> Designs { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ObtainedСomponent> ObtainedСomponents { get; set; }
        public DbSet<ObtainedDesign> ObtainedDesigns { get; set; }
        
        public DbSet<OrderComponent> OrderComponents { get; set; }
        public DbSet<OrderDesign> OrderDesign { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<LogDesign> LogsDesign { get; set; }
        public DbSet<LogComponent> LogsComponent { get; set; }

        public ApplicationContext()
        {
            
        }

        public void ResetDatabase()
        {
            // Database.EnsureDeleted();   // удаляем бд со старой схемой
            Database.EnsureCreated();   // создаем бд с новой схемой
        }
                
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseMySQL("server = db; UserId = root; Password = 123PassWord123; database = new_schema;");
            // optionsBuilder.UseMySQL("server = localhost; UserId = user1; Password = 123PassWord; database = production-management-system;");
            optionsBuilder.UseSqlServer("Server=tcp:productionmanagementsystem.database.windows.net,1433;Initial Catalog=productionmanagementsystem;Persist Security Info=False;User ID=user1;Password=123Pass123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }
    }
}

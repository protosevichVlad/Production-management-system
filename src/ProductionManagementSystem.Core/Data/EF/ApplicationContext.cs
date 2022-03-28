using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Models.Components;
using ProductionManagementSystem.Core.Models.Devices;
using ProductionManagementSystem.Core.Models.ElementsDifference;
using ProductionManagementSystem.Core.Models.Logs;
using ProductionManagementSystem.Core.Models.Orders;
using ProductionManagementSystem.Core.Models.SupplyRequests;
using ProductionManagementSystem.Core.Models.Tasks;
using ProductionManagementSystem.Core.Models.Users;

namespace ProductionManagementSystem.Core.Data.EF
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Montage> Montages { get; set; }
        public DbSet<Design> Designs { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<MontageInDevice> MontageInDevices { get; set; }
        public DbSet<DesignInDevice> DesignInDevices { get; set; }
        
        public DbSet<Task> Tasks { get; set; }
        public DbSet<ObtainedMontage> ObtainedMontages { get; set; }
        public DbSet<ObtainedDesign> ObtainedDesigns { get; set; }
        
        public DbSet<Log> Logs { get; set; }
        public DbSet<Order> Orders { get; set; }
        
        public DbSet<MontageSupplyRequest> ComponentSupplyRequests { get; set; }
        public DbSet<DesignSupplyRequest> DesignSupplyRequests { get; set; }
        
        public DbSet<ElementDifference> ElementDifferences { get; set; }
        public DbSet<DatabaseTable> DatabaseTables { get; set; }
        public DbSet<TableColumn> TableColumns { get; set; }
        public DbSet<ToDoNote> ToDoNotes { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public void ResetDatabase()
        {
            Database.EnsureCreated(); 
        }
                
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().HasMany<Log>().WithOne(x => x.User).OnDelete(DeleteBehavior.SetNull);
        }
    }
}

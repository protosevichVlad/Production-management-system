using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ProductionManagementSystem.Models.Components;
using ProductionManagementSystem.Models.Devices;
using ProductionManagementSystem.Models.Logs;
using ProductionManagementSystem.Models.Orders;
using ProductionManagementSystem.Models.SupplyRequests;
using ProductionManagementSystem.Models.Tasks;
using ProductionManagementSystem.Models.Users;

namespace ProductionManagementSystem.DAL.EF
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
    }
}

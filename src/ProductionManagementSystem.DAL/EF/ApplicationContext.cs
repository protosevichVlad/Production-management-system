using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ProductionManagementSystem.DAL.Entities;
using System;

namespace ProductionManagementSystem.DAL.EF
{
    public class ApplicationContext : IdentityDbContext<ProductionManagementSystemUser>
    {
        public DbSet<Device> Devices { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<DeviceComponentsTemplate> DeviceComponentsTemplates { get; set; }
        public DbSet<DeviceDesignTemplate> DeviceDesignTemplates { get; set; }
        
        public DbSet<Design> Designs { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<ObtainedComponent> ObtainedComponents { get; set; }
        public DbSet<ObtainedDesign> ObtainedDesigns { get; set; }
        
        public DbSet<Log> Logs { get; set; }
        public DbSet<Order> Orders { get; set; }

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

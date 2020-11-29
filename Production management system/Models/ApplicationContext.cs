﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public ApplicationContext()
        {
            
        }

        public void ResetDatabase()
        {
            Database.EnsureDeleted();   // удаляем бд со старой схемой
            Database.EnsureCreated();   // создаем бд с новой схемой
        }
                
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseMySQL("server = db; UserId = root; Password = 123PassWord123; database = new_schema;");
            optionsBuilder.UseMySQL("server = localhost; UserId = user1; Password = 123Pass123; database = new_schema;");
        }
    }
}
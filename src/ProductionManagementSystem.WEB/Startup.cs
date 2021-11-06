using System;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Services;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Users;

namespace ProductionManagementSystem.WEB
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<ApplicationContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 24)))
            );
            

            services.AddIdentity<User, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    
                    options.User.RequireUniqueEmail = false;
                })
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultUI();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            services.AddUoWService(Configuration.GetConnectionString("DefaultConnection"));
            services.AddScoped<IMontageService, MontageService>();
            services.AddScoped<IDesignService, DesignService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IMontageSupplyRequestService, MontageSupplyRequestService>();
            services.AddScoped<IDesignSupplyRequestService, DesignSupplyRequestService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var cultureInfo = new CultureInfo("ru-RU");

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<GetCurrentUserMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}

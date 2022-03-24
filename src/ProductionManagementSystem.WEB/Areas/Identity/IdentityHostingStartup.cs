
// using ProductionManagementSystem.Data;
//
// [assembly: HostingStartup(typeof(ProductionManagementSystem.Areas.Identity.IdentityHostingStartup))]
// namespace ProductionManagementSystem.Areas.Identity
// {
//     public class IdentityHostingStartup : IHostingStartup
//     {
//         public void Configure(IWebHostBuilder builder)
//         {
//             builder.ConfigureServices((context, services) => {
//                 services.AddDbContext<ProductionManagementSystemContext>(options =>
//                     options.UseSqlServer(
//                         context.Configuration.GetConnectionString("ProductionManagementSystemContextConnection")));
//
//                 services.AddDefaultIdentity<ProductionManagementSystemUser>(options => options.SignIn.RequireConfirmedAccount = true)
//                     .AddEntityFrameworkStores<ProductionManagementSystemContext>();
//             });
//         }
//     }
// }
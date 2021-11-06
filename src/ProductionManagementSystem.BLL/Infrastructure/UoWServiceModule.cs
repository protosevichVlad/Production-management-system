using Microsoft.Extensions.DependencyInjection;
using ProductionManagementSystem.DAL.Repositories;

namespace ProductionManagementSystem.BLL.Infrastructure
{
    public static class UoWServiceModule
    {
        public static void AddUoWService(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IUnitOfWork>(_ => new EFUnitOfWork(connectionString));
        }
    }
}
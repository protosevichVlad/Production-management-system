using Ninject.Modules;
using ProductionManagementSystem.DAL.Interfaces;
using ProductionManagementSystem.DAL.Repositories;

namespace ProductionManagementSystem.BLL.Infrastructure
{
    public class ServiceModule : NinjectModule
    {
        private string _connectionString;
        public ServiceModule(string connection)
        {
            _connectionString = connection;
        }
        public override void Load()
        {
            Bind<IUnitOfWork>().To<EFUnitOfWork>().WithConstructorArgument(_connectionString);
        }
    }
}
using Business.Services.Entities.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Factory
{
    /// <summary>
    /// Implementación concreta de la factoría de servicios, que utiliza el contenedor de inyección de dependencias
    /// (a través de <see cref="IServiceProvider"/>) para resolver las dependencias de los servicios solicitados en tiempo de ejecución.
    /// </summary>
    public class BusinessServiceFactory : IBusinessServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public BusinessServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Resuelve y devuelve una instancia del servicio de registro de Sucursales desde el contenedor IoC.
        /// </summary>
        public IBranchRegistrationService CreateBranchRegistrationService()
        {
            // El contenedor resuelve todo automáticamente
            return _serviceProvider.GetRequiredService<IBranchRegistrationService>();
        }

        /// <summary>
        /// Resuelve y devuelve una instancia del servicio de registro de Zonas desde el contenedor IoC.
        /// </summary>
        public IZoneRegistrationService CreateZoneRegistrationService()
        {
            // El contenedor resuelve todo automáticamente
            return _serviceProvider.GetRequiredService<IZoneRegistrationService>();
        }

        /// <summary>
        /// Resuelve y devuelve una instancia del servicio de registro de Operativos desde el contenedor IoC.
        /// </summary>
        public IOperativeRegistrationService CreateOperativeRegistrationService()
        {
            // El contenedor resuelve todo automáticamente
            return _serviceProvider.GetRequiredService<IOperativeRegistrationService>();
        }

        /// <summary>
        /// Resuelve y devuelve una instancia del servicio de registro de Verificadores desde el contenedor IoC.
        /// </summary>
        public ICheckerRegistrationService CreateCheckerRegistrationService()
        {
            // El contenedor resuelve todo automáticamente
            return _serviceProvider.GetRequiredService<ICheckerRegistrationService>();
        }
    }
}

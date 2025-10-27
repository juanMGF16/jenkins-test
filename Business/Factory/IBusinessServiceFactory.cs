using Business.Services.Entities.Interfaces;

namespace Business.Factory
{
    /// <summary>
    /// Define una factoría para la creación (resolución) de instancias de servicios de registro de entidades del dominio.
    /// Esto permite desacoplar la creación de servicios de sus consumidores.
    /// </summary>
    public interface IBusinessServiceFactory
    {
        /// <summary>
        /// Crea y devuelve una instancia del servicio de registro de Sucursales.
        /// </summary>
        /// <returns>Una instancia de <see cref="IBranchRegistrationService"/>.</returns>
        IBranchRegistrationService CreateBranchRegistrationService();

        /// <summary>
        /// Crea y devuelve una instancia del servicio de registro de Zonas.
        /// </summary>
        /// <returns>Una instancia de <see cref="IZoneRegistrationService"/>.</returns>
        IZoneRegistrationService CreateZoneRegistrationService();

        /// <summary>
        /// Crea y devuelve una instancia del servicio de registro de Operativos.
        /// </summary>
        /// <returns>Una instancia de <see cref="IOperativeRegistrationService"/>.</returns>
        IOperativeRegistrationService CreateOperativeRegistrationService();

        /// <summary>
        /// Crea y devuelve una instancia del servicio de registro de Verificadores.
        /// </summary>
        /// <returns>Una instancia de <see cref="ICheckerRegistrationService"/>.</returns>
        ICheckerRegistrationService CreateCheckerRegistrationService();
    }
}

using Business.Factory;
using Business.Repository.Implementations.Specific.ScanItem;
using Business.Repository.Implementations.Specific.System.Others;
using Business.Repository.Interfaces.Specific.ScanItem;
using Business.Repository.Interfaces.Specific.System.Others;
using Business.Services.CacheItem;
using Business.Services.CargaMasiva.Implementations;
using Business.Services.CargaMasiva.Interfaces;
using Business.Services.CredentialGenerator;
using Business.Services.CredentialGenerator.Interfaces;
using Business.Services.Entities.Implementations;
using Business.Services.Entities.Interfaces;
using Business.Services.GenerarQR;
using Business.Services.InventaryItem;
using Business.Services.Jwt;
using Business.Services.Jwt.Interfaces;
using Business.Services.NITValidation;
using Business.Services.NITValidation.Interfaces;
using Business.Services.PaswordRecovery;
using Business.Services.PaswordRecovery.Interfaces;
using Business.Services.Reports.Implementations;
using Business.Services.Reports.Interfaces;
using Business.Services.SendEmail;
using Business.Services.SendEmail.Interfaces;
using Business.Strategy.Implementations.BulkUpload;
using Business.Strategy.Interfaces.BulkUpload;
using Data.Factory;
using Data.Repository.Implementations.Specific.System.Others;
using Data.Repository.Implementations.Strategy.Delete;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Specific.System.Others;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.Models.ParametersModule;
using Utilities.Common;
using Utilities.Helpers;

namespace Web.Extensions
{
    /// <summary>
    /// Extensiones para registro de servicios de la capa Business
    /// </summary>
    public static class BusinessServiceExtensions
    {
        /// <summary>
        /// Registra todos los servicios de negocio, factories, estrategias y servicios externos
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configuration">Configuración de la aplicación</param>
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            // =============== [ Email Service ] ===============
            var emailSettings = configuration.GetSection("EmailSettings");
            services.Configure<EmailSettings>(emailSettings);
            services.AddScoped<IEmailService, EmailService>();

            // =============== [ JWT Service ] ===============
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<AuthService>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<CookieTokenHelper>();

            // =============== [ Factory ] ===============
            services.AddScoped<IDataFactoryGlobal, GlobalFactory>(); // Data
            services.AddScoped<IBusinessServiceFactory, BusinessServiceFactory>(); // Business
            services.AddScoped<IItemBulkStrategyFactory, ItemBulkStrategyFactory>(); // Bulk Upload Strategy
            services.AddScoped<IOperativeBulkStrategyFactory, OperativeBulkStrategyFactory>(); // Bulk Upload Strategy

            // =============== [ Factory Entities Bussiness Service ] ===============
            services.AddScoped<IBranchRegistrationService, BranchRegistrationService>();
            services.AddScoped<IZoneRegistrationService, ZoneRegistrationService>();
            services.AddScoped<IOperativeRegistrationService, OperativeRegistrationService>();
            services.AddScoped<ICheckerRegistrationService, CheckerRegistrationService>();

            // =============== [ Strategy Services ] ===============
            services.AddScoped(typeof(LogicalDeleteStrategy<>));
            services.AddScoped(typeof(PermanentDeleteStrategy<>));
            services.AddScoped(typeof(CascadeDeleteStrategy<>));
            services.AddScoped(typeof(IDeleteStrategyResolver<>), typeof(DeleteStrategyResolver<>));

            services.AddScoped<IItemBulkUploadStrategy, ItemExcelBulkUploadStrategy>();
            services.AddScoped<IOperativeBulkUploadStrategy, OperativeExcelBulkUploadStrategy>();

            // =============== [ Extra Services ] ===============
            services.AddScoped<IQrCodeService, QrCodeService>();
            services.AddScoped<IPasswordRecoveryService, PasswordRecoveryService>();
            services.AddHttpClient<INitValidationService, NitValidationService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<ICredentialGeneratorService, CredentialGeneratorService>();
            services.AddScoped<IItemBulkUploadService, ItemBulkUploadService>(); // Carga Masiva
            services.AddScoped<IOperativeBulkUploadService, OperativeBulkUploadService>(); // Carga Masiva

            // =============== [ Export Services Config ] ===============
            services.AddScoped<IExportService, ExportService>(); // Exportacion de Reportes Zona
            services.Configure<Business.Services.Reports.Configuration.ExportOptions>(configuration.GetSection("Export"));

            // =============== [ Others Entity] ===============
            services.AddScoped<IDashboardData, DashboardData>();
            services.AddScoped<IDashboardBusiness, DashboardBusiness>();

            // Inventory Repository
            services.AddScoped<IInventoryRepository, InventoryRepository>();

            // Inventory Services
            services.AddScoped<IInventoryStartService, InventoryStartService>();
            services.AddScoped<IInventoryScanService, InventoryScanService>();
            services.AddScoped<IInventoryFinishService, InventoryFinishService>();
            services.AddScoped<IInventoryVerificationService, InventoryVerificationService>();

            // Validator
            services.AddScoped<IInventoryValidator, InventoryValidator>();

            // Cache service (usa IMemoryCache internamente)
            services.AddMemoryCache();
            services.AddScoped<IInventoryCacheService, InventoryCacheService>();

            services.AddScoped<IZoneReportsData, ZoneReportsData>();
            services.AddScoped<IZoneReportsBusiness, ZoneReportsBusiness>();

            return services;
        }
    }
}

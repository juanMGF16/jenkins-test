using Data.Repository.Implementations.Specific.ParametersModule;
using Data.Repository.Implementations.Specific.SecurityModule;
using Data.Repository.Implementations.Specific.System;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Specific.ParametersModule;
using Data.Repository.Interfaces.Specific.SecurityModule;
using Data.Repository.Interfaces.Specific.System;
using Entity.Context;
using Entity.Models.ParametersModule;
using Entity.Models.SecurityModule;
using Entity.Models.System;
using Microsoft.Extensions.Logging;

namespace Data.Factory
{
    /// <summary>
    /// Implementación de la fábrica global que centraliza la creación de repositorios de datos.
    /// Configura cada repositorio con su contexto, logger y servicios necesarios.
    /// </summary>
    public class GlobalFactory : IDataFactoryGlobal
    {
        private readonly AppDbContext _context;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IQrCodeService _qrService;

        public GlobalFactory(AppDbContext context, ILoggerFactory loggerFactory, IQrCodeService qrService)
        {
            _context = context;
            _loggerFactory = loggerFactory;
            _qrService = qrService;
        }

        // Cada método Create instancia un repositorio específico con sus dependencias configuradas

        // -----------------------
        // SecurityModule
        // -----------------------
        public IPersonData CreatePersonData()
        {
            var logger = _loggerFactory.CreateLogger<Person>();
            return new PersonData(_context, logger);
        }

        public IUserData CreateUserData()
        {
            var logger = _loggerFactory.CreateLogger<User>();
            return new UserData(_context, logger);
        }

        public IRoleData CreateRoleData()
        {
            var logger = _loggerFactory.CreateLogger<Role>();
            return new RoleData(_context, logger);
        }

        public IFormData CreateFormData()
        {
            var logger = _loggerFactory.CreateLogger<Form>();
            return new FormData(_context, logger);
        }

        public IModuleData CreateModuleData()
        {
            var logger = _loggerFactory.CreateLogger<Module>();
            return new ModuleData(_context, logger);
        }

        public IPermissionData CreatePermissionData()
        {
            var logger = _loggerFactory.CreateLogger<Permission>();
            return new PermissionData(_context, logger);
        }

        public IUserRoleData CreateUserRoleData()
        {
            var logger = _loggerFactory.CreateLogger<UserRole>();
            return new UserRoleData(_context, logger);
        }

        public IFormModuleData CreateFormModuleData()
        {
            var logger = _loggerFactory.CreateLogger<FormModule>();
            return new FormModuleData(_context, logger);
        }

        public IRoleFormPermissionData CreateRoleFormPermissionData()
        {
            var logger = _loggerFactory.CreateLogger<RoleFormPermission>();
            return new RoleFormPermissionData(_context, logger);
        }

        // -----------------------
        // ParametersModule
        // -----------------------
        public ICategoryData CreateCategoryData()
        {
            var logger = _loggerFactory.CreateLogger<CategoryItem>();
            return new CategoryItemData(_context, logger);
        }
        public IStateItemData CreateStateItemData()
        {
            var logger = _loggerFactory.CreateLogger<StateItem>();
            return new StateItemData(_context, logger);
        }
        public INotificationData CreateNotificationData()
        {
            var logger = _loggerFactory.CreateLogger<Notification>();
            return new NotificationData(_context, logger);
        }

        // -----------------------
        // System
        // -----------------------
        public IItem CreateItem()
        {
            var logger = _loggerFactory.CreateLogger<Item>();
            return new ItemData(_context, logger, _qrService);
        }
        public ICompany CreateCompanyData()
        {
            var logger = _loggerFactory.CreateLogger<Company>();
            return new CompanyData(_context, logger);
        }
        public IBranch CreateBranchData()
        {
            var logger = _loggerFactory.CreateLogger<Branch>();
            return new BranchData(_context, logger);
        }
        public IZone CreateZoneData()
        {
            var logger = _loggerFactory.CreateLogger<Zone>();
            return new ZoneData(_context, logger);
        }
        public IInventary CreateInventaryData()
        {
            var logger = _loggerFactory.CreateLogger<Inventary>();
            return new InventaryData(_context, logger);
        }
        public IInventaryDetail CreateInventaryDetailData()
        {
            var logger = _loggerFactory.CreateLogger<InventaryDetail>();
            return new InventaryDetailData(_context, logger);
        }
        public IOperating CreateOperatingData()
        {
            var logger = _loggerFactory.CreateLogger<Operating>();
            return new OperatingData(_context, logger);
        }
        public IOperatingGroup CreateOperatingGroupData()
        {
            var logger = _loggerFactory.CreateLogger<OperatingGroup>();
            return new OperatingGroupData(_context, logger);
        }
        public ICheckerData CreateCheckerData()
        {
            var logger = _loggerFactory.CreateLogger<Checker>();
            return new CheckerData(_context, logger);
        }
        public IVerification CreateVerificationData()
        {
            var logger = _loggerFactory.CreateLogger<Verification>();
            return new VerificationData(_context, logger);
        }

        // Ejemplo de Generico
        //public IGenericData<RoleFormPermission> CreateRoleFormPermissionData()
        //{
        //    var logger = _loggerFactory.CreateLogger<RoleFormPermission>();
        //    return new RoleFormPermissionData(_context, logger);
        //}
    }
}
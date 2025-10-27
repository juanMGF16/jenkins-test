using Business.Repository.Implementations.Specific.ParametersModule;
using Business.Repository.Implementations.Specific.SecurityModule;
using Business.Repository.Implementations.Specific.System;
using Business.Repository.Interfaces.Specific.ParametersModule;
using Business.Repository.Interfaces.Specific.SecurityModule;
using Business.Repository.Interfaces.Specific.System;
using Data.Repository.Implementations.Specific.ParametersModule;
using Data.Repository.Implementations.Specific.SecurityModule;
using Data.Repository.Implementations.Specific.System;
using Data.Repository.Interfaces;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Specific.ParametersModule;
using Data.Repository.Interfaces.Specific.SecurityModule;
using Data.Repository.Interfaces.Specific.System;
using Entity.Models.ParametersModule;
using Entity.Models.SecurityModule;
using Entity.Models.System;

namespace Web.Extensions
{
    /// <summary>
    /// Extensiones para registro de servicios de entidades del sistema
    /// </summary>
    public static class EntitiesServiceExtensions
    {
        /// <summary>
        /// Registra servicios Business y Data de todas las entidades (Security, Parameters, System)
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        public static IServiceCollection AddEntitiesServices(this IServiceCollection services)
        {
            // -----------------------
            // SecurityModule
            // -----------------------
            services.AddScoped<IPersonBusiness, PersonBusiness>();
            services.AddScoped<IGeneral<Person>, PersonData>();
            services.AddScoped<IPersonData, PersonData>();

            services.AddScoped<IUserBusiness, UserBusiness>();
            services.AddScoped<IGeneral<User>, UserData>();
            services.AddScoped<IUserData, UserData>();

            services.AddScoped<IRoleBusiness, RoleBusiness>();
            services.AddScoped<IGeneral<Role>, RoleData>();
            services.AddScoped<IRoleData, RoleData>();

            services.AddScoped<IFormBusiness, FormBusiness>();
            services.AddScoped<IGeneral<Form>, FormData>();

            services.AddScoped<IModuleBusiness, ModuleBusiness>();
            services.AddScoped<IGeneral<Module>, ModuleData>();

            services.AddScoped<IPermissionBusiness, PermissionBusiness>();
            services.AddScoped<IGeneral<Permission>, PermissionData>();

            services.AddScoped<IUserRoleBusiness, UserRoleBusiness>();
            services.AddScoped<IGeneral<UserRole>, UserRoleData>();
            services.AddScoped<IUserRoleData, UserRoleData>();


            services.AddScoped<IFormModuleBusiness, FormModuleBusiness>();
            services.AddScoped<IGeneral<FormModule>, FormModuleData>();

            services.AddScoped<IRoleFormPermissionBusiness, RoleFormPermissionBusiness>();
            services.AddScoped<IGeneral<RoleFormPermission>, RoleFormPermissionData>();

            // -----------------------
            // ParametersModule
            // -----------------------
            services.AddScoped<ICategoryBusiness, CategoryItemBusiness>();
            services.AddScoped<IGeneral<CategoryItem>, CategoryItemData>();
            services.AddScoped<ICategoryData, CategoryItemData>();

            services.AddScoped<IStateItemBusiness, StateItemBusiness>();
            services.AddScoped<IGeneral<StateItem>, StateItemData>();
            services.AddScoped<IStateItemData, StateItemData>();

            services.AddScoped<INotificationBusiness, NotificationBusiness>();
            services.AddScoped<IGeneral<Notification>, NotificationData>();

            // -----------------------
            // System
            // -----------------------
            services.AddScoped<IItemBusiness, ItemBusiness>();
            services.AddScoped<IGeneral<Item>, ItemData>();
            services.AddScoped<IGenericData<Item>, ItemData>();
            services.AddScoped<IItem, ItemData>();

            services.AddScoped<ICompanyBusiness, CompanyBusiness>();
            services.AddScoped<IGeneral<Company>, CompanyData>();
            services.AddScoped<ICompany, CompanyData>();

            services.AddScoped<IBranchBusiness, BranchBusiness>();
            services.AddScoped<IGeneral<Branch>, BranchData>();
            services.AddScoped<IBranch, BranchData>();

            services.AddScoped<IZoneBusiness, ZoneBusiness>();
            services.AddScoped<IGeneral<Zone>, ZoneData>();
            services.AddScoped<IZone, ZoneData>();

            services.AddScoped<IInventaryBusiness, InventaryBusiness>();
            services.AddScoped<IGeneral<Inventary>, InventaryData>();
            services.AddScoped<IInventary, InventaryData>();

            services.AddScoped<IInventaryDetailBusiness, InventaryDetailBusiness>();
            services.AddScoped<IGeneral<InventaryDetail>, InventaryDetailData>();

            services.AddScoped<IOperatingBusiness, OperatingBusiness>();
            services.AddScoped<IGeneral<Operating>, OperatingData>();
            services.AddScoped<IOperating, OperatingData>();

            services.AddScoped<IOperatingGroupBusiness, OperatingGroupBusiness>();
            services.AddScoped<IGeneral<OperatingGroup>, OperatingGroupData>();
            services.AddScoped<IOperatingGroup, OperatingGroupData>();

            services.AddScoped<ICheckerBusiness, CheckerBusiness>();
            services.AddScoped<IGeneral<Checker>, CheckerData>();
            services.AddScoped<ICheckerData, CheckerData>();

            services.AddScoped<IVerificationBusiness, VerificationBusiness>();
            services.AddScoped<IGeneral<Verification>, VerificationData>();
            services.AddScoped<IVerification, VerificationData>();

            return services;
        }
    }
}

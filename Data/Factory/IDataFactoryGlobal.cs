using Data.Repository.Interfaces.Specific.ParametersModule;
using Data.Repository.Interfaces.Specific.SecurityModule;
using Data.Repository.Interfaces.Specific.System;

namespace Data.Factory
{
    /// <summary>
    /// Fábrica para crear instancias de repositorios de datos con sus dependencias configuradas
    /// </summary>
    public interface IDataFactoryGlobal
    {
        // Los métodos crean instancias de repositorios con logger y contexto inyectados


        // -----------------------
        // SecurityModule
        // -----------------------
        IPersonData CreatePersonData();
        IUserData CreateUserData();
        IRoleData CreateRoleData();
        IFormData CreateFormData();
        IModuleData CreateModuleData();
        IPermissionData CreatePermissionData();
        IUserRoleData CreateUserRoleData();
        IFormModuleData CreateFormModuleData();
        IRoleFormPermissionData CreateRoleFormPermissionData();

        // -----------------------
        // ParametersModule
        // -----------------------
        ICategoryData CreateCategoryData();
        IStateItemData CreateStateItemData();
        INotificationData CreateNotificationData();

        // -----------------------
        // System
        // -----------------------
        IItem CreateItem();
        ICompany CreateCompanyData();
        IBranch CreateBranchData();
        IZone CreateZoneData();
        IInventary CreateInventaryData();
        IInventaryDetail CreateInventaryDetailData();
        IOperating CreateOperatingData();
        IOperatingGroup CreateOperatingGroupData();
        ICheckerData CreateCheckerData();
        IVerification CreateVerificationData();

        // Ejemplo de Generico
        // IGenericData<Comment> CreateCommentData();
    }
}
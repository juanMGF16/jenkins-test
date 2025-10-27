using AutoMapper;
using Entity.DTOs.ParametersModels.Category;
using Entity.DTOs.ParametersModels.Notification;
using Entity.DTOs.SecurityModule;
using Entity.DTOs.SecurityModule.FormModule;
using Entity.DTOs.SecurityModule.Person;
using Entity.DTOs.SecurityModule.RoleFormPermission;
using Entity.DTOs.SecurityModule.User;
using Entity.DTOs.SecurityModule.UserRole;
using Entity.DTOs.System;
using Entity.DTOs.System.Branch;
using Entity.DTOs.System.Checker;
using Entity.DTOs.System.Company;
using Entity.DTOs.System.Inventary;
using Entity.DTOs.System.Inventary.AreaManager.InventoryDetail;
using Entity.DTOs.System.Inventary.AreaManager.InventorySummary;
using Entity.DTOs.System.InventaryDetail;
using Entity.DTOs.System.Item;
using Entity.DTOs.System.Operating;
using Entity.DTOs.System.OperatingGroup;
using Entity.DTOs.System.Verification;
using Entity.DTOs.System.Verification.AreaManager;
using Entity.DTOs.System.Zone;
using Entity.Models.ParametersModule;
using Entity.Models.SecurityModule;
using Entity.Models.System;
using Utilities.Enums.Models;
using Utilities.Helpers;

namespace Business.AutoMapper
{
    public class GeneralMapper : Profile
    {
        public GeneralMapper()
        {

            // --------- Converters globales ---------
            // Entity (DateTime UTC) -> DTO (DateTimeOffset Bogotá)
            CreateMap<DateTime, DateTimeOffset>()
                .ConvertUsing(src => TimeHelper.ToBogotaOffset(src));

            CreateMap<DateTime?, DateTimeOffset?>()
                .ConvertUsing(src => src.HasValue ? TimeHelper.ToBogotaOffset(src.Value) : (DateTimeOffset?)null);

            // DTO (DateTimeOffset cualquiera) -> Entity (DateTime UTC)
            CreateMap<DateTimeOffset, DateTime>()
                .ConvertUsing(src => TimeHelper.ToUtcDateTime(src));

            CreateMap<DateTimeOffset?, DateTime?>()
                .ConvertUsing(src => src.HasValue ? TimeHelper.ToUtcDateTime(src.Value) : (DateTime?)null);

            // -----------------------
            // SecurityModule
            // -----------------------
            CreateMap<Person, PersonDTO>().ReverseMap();
            CreateMap<Person, PersonAvailableDTO>().ReverseMap();


            CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<Form, FormDTO>().ReverseMap();
            CreateMap<Module, ModuleDTO>().ReverseMap();
            CreateMap<Permission, PermissionDTO>().ReverseMap();


            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserOptionsDTO>().ReverseMap();

            CreateMap<UserRole, UserRoleDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
                .ReverseMap();
            CreateMap<UserRole, UserRoleOptionsDTO>().ReverseMap();


            CreateMap<FormModule, FormModuleDTO>()
                .ReverseMap();
            CreateMap<FormModule, FormModuleOptionsDTO>().ReverseMap();


            CreateMap<RoleFormPermission, RoleFormPermissionDTO>()
                .ReverseMap();
            CreateMap<RoleFormPermission, RoleFormPermissionOptionsDTO>().ReverseMap();

            // -----------------------
            // ParametersModule
            // -----------------------
            CreateMap<CategoryItem, CategoryItemDTO>().ReverseMap();


            CreateMap<StateItem, StateItemDTO>().ReverseMap();

            CreateMap<Notification, NotificationDTO>().ReverseMap();
            CreateMap<Notification, NotificationOptionsDTO>().ReverseMap();

            // -----------------------
            // System
            // -----------------------
            CreateMap<Item, ItemDTO>().ReverseMap();
            CreateMap<Item, ItemConsultDTO>()
                .ForMember(dest => dest.StateItemName, opt => opt.MapFrom(src => src.StateItem.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryItem.Name))
                .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => src.Zone.Name))
                .ReverseMap();
            CreateMap<Item, ItemConsultCategoryDTO>();
            CreateMap<Item, ZoneItemDTO>()
                .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryItem.Id))
                .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.StateItem.Id));
            CreateMap<Item, ItemInventorieBaseSimpleDTO>()
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.StateItem.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryItem.Name))
                .ReverseMap();


            CreateMap<Branch, BranchDTO>().ReverseMap();
            CreateMap<Branch, BranchConsultDTO>()
                .ForMember(dest => dest.InChargeId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.InChargeName, opt => opt.MapFrom(src => src.User.Username))
                .ReverseMap();
            CreateMap<Branch, BranchSimpleDTO>().ReverseMap();
            CreateMap<Branch, BranchDetailsDTO>()
                .ForMember(dest => dest.ZonesCount, opt => opt.MapFrom(src => src.Zones.Count))
                .ForMember(dest => dest.InventoriesCount, opt => opt.MapFrom(src => src.Zones.SelectMany(z => z.Inventories).Count()))
                .ForMember(dest => dest.Zones, opt => opt.MapFrom(src => src.Zones));
            CreateMap<Branch, BranchInChargeDTO>()
                .ForMember(dest => dest.InChargeFullName, opt => opt.MapFrom(src => src.User.Person.Name + " " + src.User.Person.LastName))
                .ForMember(dest => dest.InChargePhone, opt => opt.MapFrom(src => src.User.Person.Phone))
                .ForMember(dest => dest.InChargeEmail, opt => opt.MapFrom(src => src.User.Person.Email))
                .ReverseMap();
            CreateMap<Branch, BranchInChargeListDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.Person.Name + " " + src.User.Person.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Person.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Person.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Person.Phone))
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.User.Person.DocumentType))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.User.Person.DocumentNumber))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();


            CreateMap<Company, CompanyDTO>()
            .ForMember(dest => dest.IndustryId, opt => opt.MapFrom(src => (int)src.Industry))
            .ReverseMap()
            .ForMember(dest => dest.Industry, opt => opt.MapFrom(src => (TypeIndustry)src.IndustryId));
            CreateMap<Company, CompanyConsultDTO>()
                .ForMember(dest => dest.IndustryId, opt => opt.MapFrom(src => (int)src.Industry))
                .ForMember(dest => dest.IndustryName, opt => opt.MapFrom(src => src.Industry.ToString()))
                .ForMember(dest => dest.OwnerUserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.OwnerUserName, opt => opt.MapFrom(src => src.User.Username))
                .ReverseMap()
                .ForMember(dest => dest.Industry, opt => opt.MapFrom(src => (TypeIndustry)src.IndustryId));


            CreateMap<Zone, ZoneDTO>().ReverseMap();
            CreateMap<Zone, ZoneConsultDTO>()
                .ForMember(dest => dest.InChargeName, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.InChargeId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Name))
                .ReverseMap();
            CreateMap<Zone, ZoneSimpleDTO>().ReverseMap();
            CreateMap<Zone, ZoneSummaryDTO>()
                .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.Items.Count))
                .ForMember(dest => dest.InChargeFullName,
                    opt => opt.MapFrom(src => src.User.Person.Name + " " + src.User.Person.LastName));
            CreateMap<Zone, ZoneDetailsDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.StateZone.ToString()))
                .ForMember(dest => dest.InChargeUserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.InChargeFullName,
                    opt => opt.MapFrom(src => src.User.Person.Name + " " + src.User.Person.LastName))
                .ForMember(dest => dest.InChargeEmail, opt => opt.MapFrom(src => src.User.Person.Email))
                .ForMember(dest => dest.InChargePhone, opt => opt.MapFrom(src => src.User.Person.Phone))
                .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.Items.Count))
                .ForMember(dest => dest.InventoriesCount, opt => opt.MapFrom(src => src.Inventories.Count))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            CreateMap<Zone, ZoneInChargeListDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.Person.Name + " " + src.User.Person.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Person.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Person.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Person.Phone))
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.User.Person.DocumentType))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.User.Person.DocumentNumber))
                .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();


            CreateMap<Inventary, InventaryDTO>().ReverseMap();
            CreateMap<Inventary, InventaryConsultDTO>()
                .ForMember(dest => dest.OperatingGroupName, opt => opt.MapFrom(src => src.OperatingGroup.Name))
                .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => src.Zone.Name))
                .ReverseMap();
            CreateMap<Inventary, InventoryHistoryDTO>()
                .ForMember(dest => dest.InventoryId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => src.Zone.Name))
                .ForMember(dest => dest.Verification, opt => opt.MapFrom(src => src.Verification!.Result))
                .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.InventaryDetails.Count))
                .ForMember(dest => dest.CompletedItems, opt => opt.MapFrom(src => src.InventaryDetails.Count))
                .ReverseMap();
            CreateMap<Inventary, InventorySummaryResponseDTO>().ReverseMap();
            CreateMap<Inventary, InventoryDetailResponseDTO>().ReverseMap();



            CreateMap<InventaryDetail, InventaryDetailDTO>().ReverseMap();
            CreateMap<InventaryDetail, InventaryDetailConsultDTO>()
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item.Name))
                .ForMember(dest => dest.StateItemName, opt => opt.MapFrom(src => src.StateItem.Name))
                .ForMember(dest => dest.InventaryObservations, opt => opt.MapFrom(src => src.Inventary.Observations))
            .ReverseMap();


            CreateMap<Operating, OperatingDTO>()
                .ForMember(dest => dest.OperatingId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.CreatedByUserId, opt => opt.MapFrom(src => src.CreatedByUserId))
                .ForMember(dest => dest.OperationalGroupId, opt => opt.MapFrom(src => src.OperationalGroupId))
                .ReverseMap();
            CreateMap<Operating, OperatingConsultDTO>()
                .ForMember(dest => dest.OperatingId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.OperatingName, opt =>opt.MapFrom(src => src.User.Person.Name + " " + src.User.Person.LastName)) // Nombre completo del operativo
                .ForMember(dest => dest.CreatedByUserId, opt => opt.MapFrom(src => src.CreatedByUserId))
                .ForMember(dest => dest.CreatedByUserName, opt =>opt.MapFrom(src => src.CreatedByUser.Person.Name + " " + src.CreatedByUser.Person.LastName)) // Nombre del encargado
                .ForMember(dest => dest.OperatingGroupId, opt => opt.MapFrom(src => src.OperationalGroupId))
                .ForMember(dest => dest.OperatingGroupName, opt => opt.MapFrom(src => src.OperationalGroup != null ? src.OperationalGroup.Name : string.Empty))
                .ReverseMap();
            CreateMap<Operating, OperativeDetailsDTO>()
                .ForMember(dest => dest.OperativeId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.Person.Name + " " + src.User.Person.LastName)) // Nombre completo del operativo
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Person.Email))
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.User.Person.DocumentType))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.User.Person.DocumentNumber))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Person.Phone))
                .ForMember(dest => dest.CreatedByUserId, opt => opt.MapFrom(src => src.CreatedByUserId))
                .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => src.CreatedByUser.Person.Name + " " + src.CreatedByUser.Person.LastName)) // Nombre del encargado
                .ForMember(dest => dest.OperativeGroupId, opt => opt.MapFrom(src => src.OperationalGroupId))
                .ForMember(dest => dest.OperativeGroupName, opt => opt.MapFrom(src => src.OperationalGroup != null ? src.OperationalGroup.Name : string.Empty))
                .ReverseMap();
            CreateMap<Operating, OperativeAvailableDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.Person.Name + " " + src.User.Person.LastName))
                .ReverseMap();
            CreateMap<Operating, OperativeAssignmentDTO>()
                .ForMember(dest => dest.OperativeId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.OperativeName, opt => opt.MapFrom(src => src.User.Person.Name + " " + src.User.Person.LastName))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.User.Person.DocumentNumber))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Person.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Person.Phone))
                .ReverseMap();


            CreateMap<OperatingGroup, OperatingGroupDTO>()
                .ForMember(dest => dest.AreaManagerId, opt => opt.MapFrom(src => src.UserId))
                .ReverseMap();
            CreateMap<OperatingGroup, OperatingGroupConsultDTO>()
                .ForMember(dest => dest.AreaManagerName, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.AreaManagerId, opt => opt.MapFrom(src => src.UserId))
                .ReverseMap();
            CreateMap<OperatingGroup, OperativeGroupSimpleDTO>().ReverseMap();


            CreateMap<Checker, CheckerDTO>().ReverseMap();
            CreateMap<Checker, CheckerConsultDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Person.Name + " " + src.User.Person.LastName))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Name))
                .ReverseMap();
            CreateMap<Checker, CheckerByBranchListDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.Person.Name + " " + src.User.Person.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Person.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Person.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Person.Phone))
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.User.Person.DocumentType))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.User.Person.DocumentNumber))
                .ReverseMap();


            CreateMap<Verification, VerificationDTO>().ReverseMap();
            CreateMap<Verification, VerificationConsultDTO>()
                .ForMember(dest => dest.InventaryObservations, opt => opt.MapFrom(src => src.Inventary.Observations))
                .ForMember(dest => dest.CheckerName, opt => opt.MapFrom(src => src.Checker.User.Person.Name + " " + src.Checker.User.Person.LastName))
                .ReverseMap();
            CreateMap<Verification, VerificationDetailResponseDTO>().ReverseMap();
            CreateMap<Verification, VerificationsByCheckerDTO>()
                .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => src.Inventary.Zone.Name))
                .ForMember(dest => dest.OperatingGroupName, opt => opt.MapFrom(src => src.Inventary.OperatingGroup.Name))
                .ReverseMap();

        }
    }
}
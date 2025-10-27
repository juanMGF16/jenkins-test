namespace Entity.DTOs.SecurityModule.RoleFormPermission
{
    public class RoleFormPermissionDTO
    {
        public int Id { get; set; }
        public bool Active { get; set; }

        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;

        public int FormId { get; set; }
        public string FormName { get; set; } = string.Empty;
    }
}
namespace Entity.DTOs.SecurityModule.RoleFormPermission
{
    public class RoleFormPermissionOptionsDTO
    {
        public int Id { get; set; }
        public bool Active { get; set; }

        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public int FormId { get; set; }
    }
}
namespace Entity.DTOs.SecurityModule.UserRole
{
    public class UserRoleDTO
    {
        public int Id { get; set; }
        public bool Active { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
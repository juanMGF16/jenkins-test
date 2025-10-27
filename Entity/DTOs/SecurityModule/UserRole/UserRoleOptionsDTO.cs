namespace Entity.DTOs.SecurityModule.UserRole
{
    public class UserRoleOptionsDTO
    {
        public int Id { get; set; }
        public bool Active { get; set; }

        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}
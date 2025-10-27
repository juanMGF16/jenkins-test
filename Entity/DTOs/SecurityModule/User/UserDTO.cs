namespace Entity.DTOs.SecurityModule.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool Active { get; set; }

        public int PersonId { get; set; }
        public string PersonName { get; set; } = string.Empty;
    }
}
namespace Entity.DTOs.SecurityModule.User
{
    public class UserOptionsDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Active { get; set; }

        public int PersonId { get; set; }
    }
}
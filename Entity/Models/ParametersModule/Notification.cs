using Entity.Models.Base;
using Entity.Models.SecurityModule;
using Utilities.Enums.Models;

namespace Entity.Models.ParametersModule
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public TypeNotification Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool Read { get; set; } = false;

        // Claves Foraneas
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
namespace Entity.Models.Base
{
    public class BaseEntity : AuditableEntity
    {
        public int Id { get; set; }
        public bool Active { get; set; } = true;
    }
}
namespace Entity.DTOs.System.Operating
{
    public class OperatingDTO
    {
        public int Id { get; set; }
        public int OperatingId { get; set; }
        public int CreatedByUserId { get; set; }
        public int? OperationalGroupId { get; set; }
    }
}

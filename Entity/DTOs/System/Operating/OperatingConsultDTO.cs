namespace Entity.DTOs.System.Operating
{
    public class OperatingConsultDTO
    {
        public int Id { get; set; }
        public int OperatingId { get; set; }
        public string OperatingName { get; set; } = string.Empty;

        public int CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = null!;

        public int? OperatingGroupId { get; set; }
        public string? OperatingGroupName { get; set; } = string.Empty;
    }
}

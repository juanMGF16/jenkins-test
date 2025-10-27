namespace Entity.DTOs.System.Verification.AreaManager
{
    public class VerificationDetailResponseDTO
    {
        public int Id { get; set; }
        public bool Result { get; set; }
        public DateTimeOffset Date { get; set; }
        public string? Observations { get; set; }
        public CheckerDetailDTO Checker { get; set; } = null!;
        public InventoryVerificationInfoDTO Inventory { get; set; } = null!;
    }
}

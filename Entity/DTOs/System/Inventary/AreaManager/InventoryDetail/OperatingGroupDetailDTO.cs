namespace Entity.DTOs.System.Inventary.AreaManager.InventoryDetail
{
    public class OperatingGroupDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset DateStart { get; set; }
        public DateTimeOffset? DateEnd { get; set; }
        public List<OperativeDetailDTO> Operatings { get; set; } = new();
    }
}

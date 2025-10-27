namespace Entity.DTOs.ScanItem
{
    public class VerificationComparisonDto
    {
        public int InventaryId { get; set; }

        // Ítems faltantes
        public List<MissingItemDto> MissingItems { get; set; } = new();

        // Ítems inesperados (no pertenecen al inventario base)
        public List<UnexpectedItemDto> UnexpectedItems { get; set; } = new();

        // Ítems con discrepancias de estado
        public List<StateMismatchDto> StateMismatches { get; set; } = new();

        // Resumen rápido
        public string ShortSummary { get; set; } = string.Empty;
    }

    public class MissingItemDto
    {
        public int ItemId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string Reason { get; set; } = "No escaneado";
        public string SuggestedAction { get; set; } = "Revisar en la zona o con responsable";
    }

    public class UnexpectedItemDto
    {
        public int ItemId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string Reason { get; set; } = "No pertenece a esta zona";
        public string SuggestedAction { get; set; } = "Mover a la zona correcta o poner en cuarentena";
    }

    public class StateMismatchDto
    {
        public int ItemId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string ExpectedState { get; set; } = string.Empty;
        public string ScannedState { get; set; } = string.Empty;

        public string Reason { get; set; } = "Cambio de estado";
        public string SuggestedAction { get; set; } = "Revisar físicamente o actualizar registro";
    }
}

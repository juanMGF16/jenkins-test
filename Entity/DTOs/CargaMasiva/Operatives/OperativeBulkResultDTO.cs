namespace Entity.DTOs.CargaMasiva.Operatives
{
    public class OperativeBulkResultDTO : BulkUploadResultDTO
    {
        public List<OperativeBulkDetailDTO> ProcessedOperatives { get; set; } = [];
        public int EmailsSent { get; set; } 
        public int EmailsFailed { get; set; } 
        public int GeneratedPasswords { get; set; }

        public bool AllEmailsSent => EmailsSent > 0 && EmailsFailed == 0;
        public double EmailSuccessRate => GeneratedPasswords > 0 ? (double)EmailsSent / GeneratedPasswords * 100 : 0;
    }
}

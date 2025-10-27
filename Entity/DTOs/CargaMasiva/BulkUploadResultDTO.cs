namespace Entity.DTOs.CargaMasiva
{
    public class BulkUploadResultDTO
    {
        public int TotalRows { get; set; } // Usado en el Process del Controller
        public int Successful { get; set; } // Usado en el Process del Controller
        public int Failed { get; set; }  // Usado en el Process del Controller
        public int ValidRows { get; set; } // Usado en el Validate del Controller
        public List<BulkUploadErrorDTO> Errors { get; set; } = [];
        public TimeSpan ProcessingTime { get; set; }
    }
}

namespace Business.Services.Reports.Configuration
{
    /// <summary>
    /// Opciones de configuración generales para el servicio de exportación de reportes.
    /// Define límites, directorios temporales y ajustes de logging.
    /// </summary>
    public class ExportOptions
    {
        /// <summary>
        /// Obtiene o establece el número máximo de exportaciones que pueden ejecutarse concurrentemente.
        /// Valor por defecto: 3.
        /// </summary>
        public int MaxConcurrentExports { get; set; } = 3;

        /// <summary>
        /// Obtiene o establece el tiempo de espera máximo (en minutos) antes de cancelar una operación de exportación.
        /// Valor por defecto: 10.
        /// </summary>
        public int ExportTimeoutMinutes { get; set; } = 10;

        /// <summary>
        /// Obtiene o establece el número máximo de filas permitidas por hoja en la exportación a Excel.
        /// Valor por defecto: 65000 (cerca del límite de Excel para compatibilidad).
        /// </summary>
        public int MaxRowsPerSheet { get; set; } = 65000;

        /// <summary>
        /// Obtiene o establece el número máximo de ítems a incluir en una exportación a PDF.
        /// Los documentos PDF detallados suelen ser menos eficientes para grandes volúmenes de datos.
        /// Valor por defecto: 1000.
        /// </summary>
        public int MaxItemsForPdf { get; set; } = 1000;

        /// <summary>
        /// Obtiene o establece un valor que indica si se debe habilitar el registro detallado (logging) de las operaciones de exportación.
        /// Valor por defecto: <c>false</c>.
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;

        /// <summary>
        /// Obtiene o establece la ruta del directorio utilizado para almacenar temporalmente los archivos exportados.
        /// Valor por defecto: El directorio temporal del sistema operativo.
        /// </summary>
        public string TempDirectory { get; set; } = Path.GetTempPath();
    }
}

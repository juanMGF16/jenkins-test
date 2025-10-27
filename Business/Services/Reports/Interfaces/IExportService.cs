using Entity.DTOs.System.Zone.Reports;
using Utilities.Enums.Reports;

namespace Business.Services.Reports.Interfaces
{
    /// <summary>
    /// Define las operaciones de servicio para la exportación de datos de la Zona a diferentes formatos de archivo.
    /// Este servicio gestiona la lógica de generación de reportes, aplicando filtros opcionales.
    /// </summary>
    public interface IExportService
    {
        /// <summary>
        /// Exporta los datos de un reporte de la Zona (incluyendo sus Ítems y detalles) a un archivo Excel.
        /// </summary>
        /// <param name="zoneId">El ID de la Zona cuyos datos deben ser exportados.</param>
        /// <param name="filters">Filtros opcionales a aplicar a los datos del reporte.</param>
        /// <returns>Una tarea que retorna un DTO de respuesta que contiene la URL o referencia al archivo exportado.</returns>
        Task<ExportResponseDTO> ExportToExcelAsync(int zoneId, ZoneReportFiltersDTO? filters = null);

        /// <summary>
        /// Exporta los datos de un reporte de la Zona (incluyendo sus Ítems y detalles) a un archivo PDF.
        /// </summary>
        /// <param name="zoneId">El ID de la Zona cuyos datos deben ser exportados.</param>
        /// <param name="filters">Filtros opcionales a aplicar a los datos del reporte.</param>
        /// <returns>Una tarea que retorna un DTO de respuesta que contiene la URL o referencia al archivo exportado.</returns>
        Task<ExportResponseDTO> ExportToPdfAsync(int zoneId, ZoneReportFiltersDTO? filters = null);
    }
}

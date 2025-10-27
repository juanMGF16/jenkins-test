using Business.Repository.Interfaces.Specific.System.Others;
using Business.Services.Reports.Interfaces;
using Entity.DTOs.System.Inventary.Reports;
using Entity.DTOs.System.Item.Reports;
using Entity.DTOs.System.Verification.Reports;
using Entity.DTOs.System.Zone.Reports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Utilities.Enums.Reports;
using System.Drawing;

namespace Business.Services.Reports.Implementations
{
    /// <summary>
    /// Implementación de <see cref="IExportService"/> responsable de generar reportes detallados
    /// de la Zona en formatos Excel (.xlsx) y PDF (.pdf).
    /// Utiliza QuestPDF para PDF y EPPlus (OfficeOpenXml) para Excel.
    /// </summary>
    public class ExportService : IExportService
    {
        private readonly IZoneReportsBusiness _zoneReportsBusiness;
        private readonly ILogger<ExportService> _logger;
        private readonly Business.Services.Reports.Configuration.ExportOptions _exportOptions;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ExportService"/>.
        /// Configura las licencias de QuestPDF y EPPlus y obtiene las opciones de exportación.
        /// </summary>
        /// <param name="zoneReportsBusiness">Interfaz para obtener los datos de reportes de la Zona.</param>
        /// <param name="logger">Servicio de logging para registrar eventos y errores.</param>
        /// <param name="exportOptions">Opciones de configuración de exportación inyectadas.</param>/// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ExportService"/>.
        /// Configura las licencias de QuestPDF y EPPlus y obtiene las opciones de exportación.
        /// </summary>
        /// <param name="zoneReportsBusiness">Interfaz para obtener los datos de reportes de la Zona.</param>
        /// <param name="logger">Servicio de logging para registrar eventos y errores.</param>
        /// <param name="exportOptions">Opciones de configuración de exportación inyectadas.</param>
        public ExportService(
            IZoneReportsBusiness zoneReportsBusiness,
            ILogger<ExportService> logger,
            IOptions<Business.Services.Reports.Configuration.ExportOptions> exportOptions)
        {
            _zoneReportsBusiness = zoneReportsBusiness;
            _logger = logger;
            _exportOptions = exportOptions.Value;

            // Configurar licencias
            QuestPDF.Settings.License = LicenseType.Community;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        #region Exportación a Excel

        /// <summary>
        /// Exporta un reporte detallado de la Zona (incluyendo inventarios, ítems y verificaciones) a un archivo Excel.
        /// </summary>
        /// <param name="zoneId">El ID de la Zona para la cual se generará el reporte.</param>
        /// <param name="filters">Filtros opcionales para acotar los datos del reporte.</param>
        /// <returns>Un DTO que contiene el contenido del archivo Excel como un array de bytes, el tipo de contenido y el nombre del archivo.</returns>
        /// <exception cref="InvalidOperationException">Lanzada si ocurre un error durante la generación del reporte.</exception>
        public async Task<ExportResponseDTO> ExportToExcelAsync(int zoneId, ZoneReportFiltersDTO? filters = null)
        {
            try
            {
                _logger.LogInformation("Iniciando exportación Excel para ZoneId: {ZoneId}", zoneId);

                // Obtener datos de forma secuencial (para evitar bloqueos de EF)
                var reportData = await GetReportDataSequentialAsync(zoneId, filters);

                using var package = new ExcelPackage();

                // Crear las hojas
                await GenerateExcelSheetsAsync(package, reportData);

                _logger.LogInformation("Exportación Excel completada para ZoneId: {ZoneId}", zoneId);

                return new ExportResponseDTO
                {
                    Content = package.GetAsByteArray(),
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = GenerateFileName(zoneId, "xlsx")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando exportación Excel para ZoneId: {ZoneId}", zoneId);
                throw new InvalidOperationException($"Error al generar el reporte Excel para la zona {zoneId}", ex);
            }
        }

        #endregion

        #region Exportación a PDF

        /// <summary>
        /// Exporta un reporte detallado de la Zona a un archivo PDF.
        /// </summary>
        /// <param name="zoneId">El ID de la Zona para la cual se generará el reporte.</param>
        /// <param name="filters">Filtros opcionales para acotar los datos del reporte.</param>
        /// <returns>Un DTO que contiene el contenido del archivo PDF como un array de bytes, el tipo de contenido y el nombre del archivo.</returns>
        /// <exception cref="InvalidOperationException">Lanzada si ocurre un error durante la generación del reporte.</exception>
        public async Task<ExportResponseDTO> ExportToPdfAsync(int zoneId, ZoneReportFiltersDTO? filters = null)
        {
            try
            {
                _logger.LogInformation("Iniciando exportación PDF para ZoneId: {ZoneId}", zoneId);

                var reportData = await GetReportDataSequentialAsync(zoneId, filters);

                var document = Document.Create(container =>
                {
                    container.Page(page => ConfigurePdfPage(page, reportData));
                });

                _logger.LogInformation("Exportación PDF completada para ZoneId: {ZoneId}", zoneId);

                return new ExportResponseDTO
                {
                    Content = document.GeneratePdf(),
                    ContentType = "application/pdf",
                    FileName = GenerateFileName(zoneId, "pdf")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando exportación PDF para ZoneId: {ZoneId}", zoneId);
                throw new InvalidOperationException($"Error al generar el reporte PDF para la zona {zoneId}", ex);
            }
        }

        #endregion

        #region Obtención de datos

        /// <summary>
        /// Obtiene de forma secuencial todos los sub-reportes necesarios para la exportación (Zona, Inventarios, Evolución de Ítems, Verificaciones).
        /// </summary>
        /// <param name="zoneId">El ID de la Zona.</param>
        /// <param name="filters">Filtros a aplicar a los reportes.</param>
        /// <returns>Un contenedor con todos los datos de los reportes.</returns>
        /// <exception cref="Exception">Propaga cualquier error de la capa de negocio.</exception>
        private async Task<ReportDataContainer> GetReportDataSequentialAsync(int zoneId, ZoneReportFiltersDTO? filters)
        {
            try
            {
                _logger.LogDebug("Obteniendo datos del reporte para ZoneId: {ZoneId}", zoneId);

                var zoneReport = await _zoneReportsBusiness.GetZoneReportAsync(zoneId, filters);
                var inventories = await _zoneReportsBusiness.GetInventoryReportsAsync(zoneId, filters);
                var itemsEvolution = await _zoneReportsBusiness.GetItemsEvolutionAsync(zoneId, filters);
                var verifications = await _zoneReportsBusiness.GetVerificationReportsAsync(zoneId, filters);

                return new ReportDataContainer
                {
                    ZoneReport = zoneReport,
                    Inventories = inventories ?? Enumerable.Empty<InventoryReportDTO>(),
                    ItemsEvolution = itemsEvolution ?? Enumerable.Empty<ItemEvolutionReportDTO>(),
                    Verifications = verifications ?? Enumerable.Empty<VerificationReportDTO>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo datos para ZoneId: {ZoneId}", zoneId);
                throw;
            }
        }

        #endregion

        #region Generación Excel

        /// <summary>
        /// Genera las diferentes hojas de cálculo dentro del paquete Excel (<see cref="ExcelPackage"/>) a partir de los datos del reporte.
        /// </summary>
        /// <param name="package">El paquete Excel actual.</param>
        /// <param name="reportData">El contenedor con todos los datos del reporte.</param>
        /// <returns>Una tarea completada.</returns>
        private Task GenerateExcelSheetsAsync(ExcelPackage package, ReportDataContainer reportData)
        {
            GenerateSummarySheet(package.Workbook.Worksheets.Add("Resumen Zona"), reportData.ZoneReport);
            GenerateStatusDistributionSheet(package.Workbook.Worksheets.Add("Distribución Estados"), reportData.ZoneReport);
            GenerateInventoriesSheet(package.Workbook.Worksheets.Add("Inventarios"), reportData.Inventories);
            GenerateItemsEvolutionSheet(package.Workbook.Worksheets.Add("Evolución Ítems"), reportData.ItemsEvolution);
            GenerateVerificationsSheet(package.Workbook.Worksheets.Add("Verificaciones"), reportData.Verifications);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Genera la hoja de Resumen de Zona con información clave (ID, nombre, totales, últimas fechas, etc.).
        /// </summary>
        /// <param name="sheet">La hoja de cálculo donde se insertarán los datos.</param>
        /// <param name="report">El DTO del reporte de la Zona.</param>
        private void GenerateSummarySheet(ExcelWorksheet sheet, ZoneReportDTO report)
        {
            if (report?.ZoneInfo == null)
            {
                sheet.Cells["A1"].Value = "No hay datos disponibles";
                return;
            }

            sheet.Cells["A1"].Value = "RESUMEN DE ZONA";
            sheet.Cells["A1:D1"].Merge = true;
            FormatTitle(sheet.Cells["A1:D1"]);

            var data = new (string Label, object Value)[]
            {
                ("ID de Zona:", report.ZoneInfo.Id),
                ("Nombre:", report.ZoneInfo.Name ?? "N/A"),
                ("Total de Ítems:", report.ZoneInfo.TotalItems),
                ("Último Inventario:", report.ZoneInfo.LastInventoryDate?.ToString("dd/MM/yyyy HH:mm") ?? "N/A"),
                ("Última Verificación:", report.ZoneInfo.LastVerificationDate?.ToString("dd/MM/yyyy HH:mm") ?? "N/A"),
                ("Resultado Verificación:", GetVerificationResultText(report.ZoneInfo.LastVerificationResult))
            };

            for (int i = 0; i < data.Length; i++)
            {
                int row = i + 3;
                sheet.Cells[$"A{row}"].Value = data[i].Label;
                sheet.Cells[$"B{row}"].Value = data[i].Value;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
            }

            ApplyAutoFitAndBorders(sheet);
        }

        /// <summary>
        /// Genera la hoja con la distribución de ítems por estado (cantidad y porcentaje).
        /// </summary>
        /// <param name="sheet">La hoja de cálculo donde se insertarán los datos.</param>
        /// <param name="report">El DTO del reporte de la Zona.</param>
        private void GenerateStatusDistributionSheet(ExcelWorksheet sheet, ZoneReportDTO report)
        {
            sheet.Cells["A1"].Value = "DISTRIBUCIÓN DE ESTADOS";
            sheet.Cells["A1:C1"].Merge = true;
            FormatTitle(sheet.Cells["A1:C1"]);

            var headers = new[] { "Estado", "Cantidad", "Porcentaje" };
            for (int i = 0; i < headers.Length; i++)
                sheet.Cells[2, i + 1].Value = headers[i];

            FormatHeader(sheet.Cells["A2:C2"]);

            if (report?.ItemsByStatus != null)
            {
                int row = 3;
                foreach (var item in report.ItemsByStatus)
                {
                    sheet.Cells[$"A{row}"].Value = item.Status ?? "N/A";
                    sheet.Cells[$"B{row}"].Value = item.Count;
                    sheet.Cells[$"C{row}"].Value = item.Percentage / 100.0;
                    sheet.Cells[$"C{row}"].Style.Numberformat.Format = "0.00%";
                    row++;
                }
            }

            ApplyAutoFitAndBorders(sheet);
        }

        /// <summary>
        /// Genera la hoja con el listado detallado de los Inventarios realizados en la Zona.
        /// </summary>
        /// <param name="sheet">La hoja de cálculo donde se insertarán los datos.</param>
        /// <param name="inventories">Colección de reportes de Inventario.</param>
        private void GenerateInventoriesSheet(ExcelWorksheet sheet, IEnumerable<InventoryReportDTO> inventories)
        {
            sheet.Cells["A1"].Value = "INVENTARIOS REALIZADOS";
            sheet.Cells["A1:G1"].Merge = true;
            FormatTitle(sheet.Cells["A1:G1"]);

            var headers = new[] { "Fecha", "Grupo Operativo", "Cantidad Ítems", "Observaciones", "Resultado Verificación", "Fecha Verificación", "Verificador" };
            for (int i = 0; i < headers.Length; i++)
                sheet.Cells[2, i + 1].Value = headers[i];

            FormatHeader(sheet.Cells["A2:G2"]);

            int row = 3;
            foreach (var inv in inventories ?? Enumerable.Empty<InventoryReportDTO>())
            {
                sheet.Cells[$"A{row}"].Value = inv.Date.ToString("dd/MM/yyyy HH:mm");
                sheet.Cells[$"B{row}"].Value = inv.OperatingGroupName ?? "N/A";
                sheet.Cells[$"C{row}"].Value = inv.ItemsCount;
                sheet.Cells[$"D{row}"].Value = inv.Observations ?? "N/A";
                sheet.Cells[$"E{row}"].Value = GetVerificationResultText(inv.VerificationResult);
                sheet.Cells[$"F{row}"].Value = inv.VerificationDate?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
                sheet.Cells[$"G{row}"].Value = inv.CheckerName ?? "N/A";
                row++;
            }

            ApplyAutoFitAndBorders(sheet);
        }

        /// <summary>
        /// Genera la hoja con el listado de Ítems y su evolución a lo largo del tiempo.
        /// </summary>
        /// <param name="sheet">La hoja de cálculo donde se insertarán los datos.</param>
        /// <param name="items">Colección de reportes de Evolución de Ítems.</param>
        private void GenerateItemsEvolutionSheet(ExcelWorksheet sheet, IEnumerable<ItemEvolutionReportDTO> items)
        {
            sheet.Cells["A1"].Value = "EVOLUCIÓN DE ÍTEMS";
            sheet.Cells["A1:H1"].Merge = true;
            FormatTitle(sheet.Cells["A1:H1"]);

            var headers = new[] { "Código", "Nombre", "Categoría", "Estado Base", "Estado Actual", "Cambios Totales", "Último Cambio", "Tendencia" };
            for (int i = 0; i < headers.Length; i++)
                sheet.Cells[2, i + 1].Value = headers[i];

            FormatHeader(sheet.Cells["A2:H2"]);

            int row = 3;
            foreach (var item in items ?? Enumerable.Empty<ItemEvolutionReportDTO>())
            {
                sheet.Cells[$"A{row}"].Value = item.Code ?? "N/A";
                sheet.Cells[$"B{row}"].Value = item.Name ?? "N/A";
                sheet.Cells[$"C{row}"].Value = item.Category ?? "N/A";
                sheet.Cells[$"D{row}"].Value = item.BaseInventoryStatus ?? "N/A";
                sheet.Cells[$"E{row}"].Value = item.CurrentStatus ?? "N/A";
                sheet.Cells[$"F{row}"].Value = item.TotalChanges;
                sheet.Cells[$"G{row}"].Value = item.LastChangeDate?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
                sheet.Cells[$"H{row}"].Value = item.Trend.ToString();
                row++;
            }

            ApplyAutoFitAndBorders(sheet);
        }

        /// <summary>
        /// Genera la hoja con el listado detallado de las Verificaciones realizadas.
        /// </summary>
        /// <param name="sheet">La hoja de cálculo donde se insertarán los datos.</param>
        /// <param name="verifications">Colección de reportes de Verificación.</param>
        private void GenerateVerificationsSheet(ExcelWorksheet sheet, IEnumerable<VerificationReportDTO> verifications)
        {
            sheet.Cells["A1"].Value = "VERIFICACIONES";
            sheet.Cells["A1:F1"].Merge = true;
            FormatTitle(sheet.Cells["A1:F1"]);

            var headers = new[] { "Fecha Inventario", "Grupo Operativo", "Verificador", "Resultado", "Fecha Verificación", "Observaciones" };
            for (int i = 0; i < headers.Length; i++)
                sheet.Cells[2, i + 1].Value = headers[i];

            FormatHeader(sheet.Cells["A2:F2"]);

            int row = 3;
            foreach (var v in verifications ?? Enumerable.Empty<VerificationReportDTO>())
            {
                sheet.Cells[$"A{row}"].Value = v.InventoryDate.ToString("dd/MM/yyyy HH:mm");
                sheet.Cells[$"B{row}"].Value = v.OperatingGroupName ?? "N/A";
                sheet.Cells[$"C{row}"].Value = v.CheckerName ?? "N/A";
                sheet.Cells[$"D{row}"].Value = v.Result ? "APROBADO" : "RECHAZADO";
                sheet.Cells[$"E{row}"].Value = v.VerificationDate.ToString("dd/MM/yyyy HH:mm");
                sheet.Cells[$"F{row}"].Value = v.Observations ?? "N/A";
                row++;
            }

            ApplyAutoFitAndBorders(sheet);
        }

        #endregion

        #region Generación PDF

        /// <summary>
        /// Configura la estructura general de las páginas del documento PDF, incluyendo el encabezado,
        /// el pie de página y la organización de los componentes de contenido (tablas y resúmenes) de QuestPDF.
        /// </summary>
        /// <param name="page">Descriptor de la página de QuestPDF.</param>
        /// <param name="reportData">El contenedor con todos los datos del reporte.</param>
        private void ConfigurePdfPage(PageDescriptor page, ReportDataContainer reportData)
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Header()
                .AlignCenter()
                .Text($"Reporte de Zona - {reportData.ZoneReport?.ZoneInfo?.Name ?? "N/A"}")
                .SemiBold().FontSize(16).FontColor(Colors.Blue.Medium);

            page.Content()
                .PaddingVertical(1, Unit.Centimetre)
                .Column(column =>
                {
                    column.Spacing(15);
                    if (reportData.ZoneReport != null)
                    {
                        column.Item().Component(new SummaryComponent(reportData.ZoneReport));
                        column.Item().Component(new StatusDistributionComponent(reportData.ZoneReport));
                    }

                    column.Item().Component(new InventoriesTableComponent(reportData.Inventories));
                    column.Item().Component(new ItemsEvolutionComponent(reportData.ItemsEvolution));
                    column.Item().Component(new VerificationsTableComponent(reportData.Verifications));
                });

            page.Footer()
                .AlignCenter()
                .Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                    x.Span($" - Generado el {DateTime.Now:dd/MM/yyyy HH:mm}");
                });
        }

        #endregion

        #region Utilidades

        /// <summary>
        /// Aplica formato de título a un rango de celdas en Excel (Negrita, Tamaño, Centrado, Fondo Azul Claro).
        /// </summary>
        /// <param name="range">El rango de celdas a formatear.</param>
        private static void FormatTitle(ExcelRange range)
        {
            range.Style.Font.Bold = true;
            range.Style.Font.Size = 16;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(color: System.Drawing.Color.LightBlue);
        }

        /// <summary>
        /// Aplica formato de encabezado a un rango de celdas en Excel (Negrita, Fondo Gris Claro, Borde).
        /// </summary>
        /// <param name="range">El rango de celdas a formatear.</param>
        private static void FormatHeader(ExcelRange range)
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
        }

        /// <summary>
        /// Ajusta automáticamente el ancho de las columnas de la hoja de cálculo.
        /// </summary>
        /// <param name="sheet">La hoja de cálculo a la que se le aplicará el formato.</param>
        private static void ApplyAutoFitAndBorders(ExcelWorksheet sheet)
        {
            if (sheet.Dimension != null)
            {
                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
                //sheet.Cells[sheet.Dimension.Address].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }
        }

        /// <summary>
        /// Convierte el resultado booleano de una verificación a una cadena de texto legible.
        /// </summary>
        /// <param name="result">El resultado booleano de la verificación (true = APROBADA, false = RECHAZADA).</param>
        /// <returns>La cadena de texto del resultado, o "N/A" si es nulo.</returns>
        private static string GetVerificationResultText(bool? result) =>
            result.HasValue ? (result.Value ? "APROBADA" : "RECHAZADA") : "N/A";

        /// <summary>
        /// Genera un nombre de archivo único utilizando el ID de la Zona y la marca de tiempo actual.
        /// </summary>
        /// <param name="zoneId">El ID de la Zona.</param>
        /// <param name="extension">La extensión del archivo (p. ej., "xlsx", "pdf").</param>
        /// <returns>El nombre del archivo generado.</returns>
        private static string GenerateFileName(int zoneId, string extension) =>
            $"Reporte_Zona_{zoneId}_{DateTime.Now:yyyyMMdd_HHmmss}.{extension}";

        #endregion

        #region DTO interno

        /// <summary>
        /// Contenedor de datos internos utilizado para agrupar todos los DTOs de reporte obtenidos
        /// de la capa de negocio antes de la generación del archivo final.
        /// </summary>
        private class ReportDataContainer
        {
            /// <summary>
            /// Reporte principal con el resumen e información de la Zona.
            /// </summary>
            public ZoneReportDTO ZoneReport { get; set; } = new();
            /// <summary>
            /// Colección de reportes detallados de los inventarios realizados.
            /// </summary>
            public IEnumerable<InventoryReportDTO> Inventories { get; set; } = Enumerable.Empty<InventoryReportDTO>();
            /// <summary>
            /// Colección que muestra el histórico y la tendencia de cambio de los ítems.
            /// </summary>
            public IEnumerable<ItemEvolutionReportDTO> ItemsEvolution { get; set; } = Enumerable.Empty<ItemEvolutionReportDTO>();
            /// <summary>
            /// Colección de reportes detallados de las verificaciones realizadas.
            /// </summary>
            public IEnumerable<VerificationReportDTO> Verifications { get; set; } = Enumerable.Empty<VerificationReportDTO>();
        }

        #endregion
    }

    #region Componentes QuestPDF (Actualizados con validaciones)

    /// <summary>
    /// Componente de QuestPDF para renderizar el resumen de la información principal de la Zona.
    /// </summary>
    public class SummaryComponent : IComponent
    {
        private readonly ZoneReportDTO _report;

        public SummaryComponent(ZoneReportDTO report)
        {
            _report = report ?? throw new ArgumentNullException(nameof(report));
        }

        /// <summary>
        /// Define el diseño y el contenido del componente de resumen.
        /// </summary>
        /// <param name="container">El contenedor de QuestPDF para organizar el contenido.</param>
        public void Compose(IContainer container)
        {
            container.Background(Colors.Grey.Lighten3)
                .Padding(10)
                .Column(column =>
                {
                    column.Spacing(5);

                    column.Item().Text("RESUMEN DE ZONA").SemiBold().FontSize(12);
                    column.Item().Text($"Nombre: {_report.ZoneInfo?.Name ?? "N/A"}");
                    column.Item().Text($"Total de ítems: {_report.ZoneInfo?.TotalItems ?? 0}");

                    if (_report.ZoneInfo?.LastInventoryDate.HasValue == true)
                        column.Item().Text($"Último inventario: {_report.ZoneInfo.LastInventoryDate.Value:dd/MM/yyyy HH:mm}");

                    if (_report.ZoneInfo?.LastVerificationDate.HasValue == true)
                    {
                        column.Item().Text($"Última verificación: {_report.ZoneInfo.LastVerificationDate.Value:dd/MM/yyyy HH:mm}");
                        var result = _report.ZoneInfo.LastVerificationResult.HasValue
                            ? (_report.ZoneInfo.LastVerificationResult.Value ? "APROBADA" : "RECHAZADA")
                            : "N/A";
                        column.Item().Text($"Resultado: {result}");
                    }
                });
        }
    }

    /// <summary>
    /// Componente de QuestPDF para renderizar la distribución de ítems por estado.
    /// </summary>
    public class StatusDistributionComponent : IComponent
    {
        private readonly ZoneReportDTO _report;

        public StatusDistributionComponent(ZoneReportDTO report)
        {
            _report = report ?? throw new ArgumentNullException(nameof(report));
        }

        /// <summary>
        /// Define el diseño y el contenido del componente de distribución de estados.
        /// </summary>
        /// <param name="container">El contenedor de QuestPDF para organizar el contenido.</param>
        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(5);
                column.Item().Text("DISTRIBUCIÓN DE ESTADOS").SemiBold().FontSize(12);

                if (_report.ItemsByStatus?.Any() == true)
                {
                    foreach (var item in _report.ItemsByStatus)
                    {
                        column.Item().Text($"{item.Status ?? "N/A"}: {item.Count} ítems ({item.Percentage}%)");
                    }
                }
                else
                {
                    column.Item().Text("No hay datos de distribución de estados").Italic();
                }
            });
        }
    }

    /// <summary>
    /// Componente de QuestPDF para renderizar la tabla con el listado de Inventarios realizados.
    /// </summary>
    public class InventoriesTableComponent : IComponent
    {
        private readonly IEnumerable<InventoryReportDTO> _inventories;

        public InventoriesTableComponent(IEnumerable<InventoryReportDTO> inventories)
        {
            _inventories = inventories ?? Enumerable.Empty<InventoryReportDTO>();
        }

        /// <summary>
        /// Define el diseño y el contenido del componente de tabla de inventarios.
        /// </summary>
        /// <param name="container">El contenedor de QuestPDF para organizar el contenido.</param>
        public void Compose(IContainer container)
        {
            if (!_inventories.Any())
            {
                container.Text("No hay inventarios registrados").Italic();
                return;
            }

            container.Column(column =>
            {
                column.Item().Text("INVENTARIOS").SemiBold().FontSize(12);
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2); // Fecha
                        columns.RelativeColumn(2); // Grupo
                        columns.RelativeColumn(1); // Ítems
                        columns.RelativeColumn(3); // Observaciones
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Fecha").SemiBold();
                        header.Cell().Text("Grupo Operativo").SemiBold();
                        header.Cell().Text("Ítems").SemiBold();
                        header.Cell().Text("Observaciones").SemiBold();
                    });

                    foreach (var inventory in _inventories)
                    {
                        table.Cell().Text(inventory.Date.ToString("dd/MM/yyyy HH:mm"));
                        table.Cell().Text(inventory.OperatingGroupName ?? "N/A");
                        table.Cell().Text(inventory.ItemsCount.ToString());
                        table.Cell().Text(inventory.Observations ?? "N/A");
                    }
                });
            });
        }
    }

    /// <summary>
    /// Componente de QuestPDF para renderizar la tabla con la evolución de los Ítems.
    /// Nota: Limita la visualización a los primeros 10 ítems para evitar saturación en el PDF.
    /// </summary>
    public class ItemsEvolutionComponent : IComponent
    {
        private readonly IEnumerable<ItemEvolutionReportDTO> _items;

        public ItemsEvolutionComponent(IEnumerable<ItemEvolutionReportDTO> items)
        {
            _items = items ?? Enumerable.Empty<ItemEvolutionReportDTO>();
        }

        /// <summary>
        /// Define el diseño y el contenido del componente de tabla de evolución de ítems.
        /// </summary>
        /// <param name="container">El contenedor de QuestPDF para organizar el contenido.</param>
        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text("EVOLUCIÓN DE ÍTEMS").SemiBold().FontSize(12);

                if (!_items.Any())
                {
                    column.Item().Text("No hay ítems para mostrar").Italic();
                    return;
                }

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1); // Código
                        columns.RelativeColumn(2); // Nombre
                        columns.RelativeColumn(1); // Estado Actual
                        columns.RelativeColumn(1); // Cambios
                        columns.RelativeColumn(1); // Tendencia
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Código").SemiBold();
                        header.Cell().Text("Nombre").SemiBold();
                        header.Cell().Text("Estado Actual").SemiBold();
                        header.Cell().Text("Cambios").SemiBold();
                        header.Cell().Text("Tendencia").SemiBold();
                    });

                    foreach (var item in _items.Take(10)) // Limitar para no saturar el PDF
                    {
                        table.Cell().Text(item.Code ?? "N/A");
                        table.Cell().Text(item.Name ?? "N/A");
                        table.Cell().Text(item.CurrentStatus ?? "N/A");
                        table.Cell().Text(item.TotalChanges.ToString());
                        table.Cell().Text(item.Trend.ToString());
                    }
                });

                if (_items.Count() > 10)
                {
                    column.Item().Text($"... y {_items.Count() - 10} ítems más").Italic().FontSize(8);
                }
            });
        }
    }

    /// <summary>
    /// Componente de QuestPDF para renderizar la tabla con el listado de Verificaciones.
    /// </summary>
    public class VerificationsTableComponent : IComponent
    {
        private readonly IEnumerable<VerificationReportDTO> _verifications;

        public VerificationsTableComponent(IEnumerable<VerificationReportDTO> verifications)
        {
            _verifications = verifications ?? Enumerable.Empty<VerificationReportDTO>();
        }

        /// <summary>
        /// Define el diseño y el contenido del componente de tabla de verificaciones.
        /// </summary>
        /// <param name="container">El contenedor de QuestPDF para organizar el contenido.</param>
        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text("VERIFICACIONES").SemiBold().FontSize(12);

                if (!_verifications.Any())
                {
                    column.Item().Text("No hay verificaciones registradas").Italic();
                    return;
                }

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2); // Fecha
                        columns.RelativeColumn(2); // Grupo
                        columns.RelativeColumn(1); // Verificador
                        columns.RelativeColumn(1); // Resultado
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Fecha Inventario").SemiBold();
                        header.Cell().Text("Grupo Operativo").SemiBold();
                        header.Cell().Text("Verificador").SemiBold();
                        header.Cell().Text("Resultado").SemiBold();
                    });

                    foreach (var verification in _verifications)
                    {
                        table.Cell().Text(verification.InventoryDate.ToString("dd/MM/yyyy HH:mm"));
                        table.Cell().Text(verification.OperatingGroupName ?? "N/A");
                        table.Cell().Text(verification.CheckerName ?? "N/A");
                        table.Cell().Text(verification.Result ? "APROBADO" : "RECHAZADO");
                    }
                });
            });
        }
    }

    #endregion
}

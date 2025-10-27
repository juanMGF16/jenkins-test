using ClosedXML.Excel;
using Utilities.Exceptions;

namespace Utilities.Helpers
{
    /// <summary>
    /// Helper para validación y mapeo de encabezados de archivos Excel
    /// </summary>
    public static class ExcelHeaderHelper
    {
        /// <summary>
        /// Valida que las columnas requeridas existan en el Excel y retorna un mapeo columna-índice
        /// </summary>
        /// <param name="headerRow">Fila de encabezados del Excel</param>
        /// <param name="expectedHeaders">Columnas esperadas</param>
        /// <returns>Diccionario con mapeo de nombre de columna a índice</returns>
        public static Dictionary<string, int> ValidateAndMapHeaders(
            IXLRow headerRow,
            IEnumerable<string> expectedHeaders)
        {
            var mapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (int i = 1; i <= headerRow.CellCount(); i++)
            {
                var header = headerRow.Cell(i).GetString().Trim();
                if (!string.IsNullOrWhiteSpace(header))
                {
                    mapping[header] = i; // Guarda el índice de la columna
                }
            }

            // Verificar que todos los esperados están presentes
            foreach (var expected in expectedHeaders)
            {
                if (!mapping.ContainsKey(expected))
                {
                    throw new ValidationException("Excel",
                        $"La columna requerida '{expected}' no fue encontrada en el archivo.");
                }
            }

            return mapping;
        }
    }
}

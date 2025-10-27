using Utilities.Enums;

namespace Entity.Models.ParametersModule
{
    /// <summary>
    /// Opciones de configuración para el naming de archivos QR.
    /// </summary>
    public class QrNamingOptions
    {
        public string ItemCode { get; set; } = string.Empty;
        public string Prefix { get; set; } = "item";
        public string Suffix { get; set; } = string.Empty;
        public string FolderName { get; set; } = "qr";
        public UniqueIdStrategy UniqueStrategy { get; set; } = UniqueIdStrategy.ShortId;
    }
}

namespace Data.Repository.Interfaces.General
{
    /// <summary>
    /// Servicio para generación y almacenamiento de códigos QR
    /// </summary>
    public interface IQrCodeService
    {
        /// <summary>
        /// Genera un código QR optimizado y lo guarda en Cloudinary.
        /// </summary>
        /// <param name="content">Contenido a codificar en el QR.</param>
        /// <param name="itemCode">Código del item.</param>
        /// <param name="useShortId">Si usar ID corto (true) o timestamp (false).</param>
        /// <returns>URL pública del QR generado.</returns>
        string GenerateAndSaveQrCode(string content, string itemCode, bool useShortId = true);

        /// <summary>
        /// Genera un código QR con opciones personalizadas de naming.
        /// </summary>
        /// <param name="content">Contenido a codificar en el QR.</param>
        /// <param name="customOptions">Opciones personalizadas.</param>
        /// <returns>URL pública del QR generado.</returns>
        string GenerateAndSaveQrCodeWithHierarchy(string content, int itemId, string itemCode, bool useShortId = true);
    }
}

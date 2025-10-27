using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Data.Repository.Interfaces.General;
using Entity.Models.ParametersModule;
using QRCoder;
using Utilities.Enums;

namespace Business.Services
{
    /// <summary>
    /// Implementación mejorada del servicio de generación de códigos QR.
    /// Esta clase utiliza la librería QRCoder para generar imágenes QR
    /// y las almacena en Cloudinary con URLs optimizadas.
    /// </summary>
    public class QrCodeService : IQrCodeService
    {
        private readonly Cloudinary _cloudinary;

        /// <summary>
        /// Constructor que recibe la instancia configurada de Cloudinary.
        /// </summary>
        /// <param name="cloudinary">Instancia de Cloudinary registrada en DI.</param>
        public QrCodeService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        /// <summary>
        /// Genera un código QR a partir del contenido proporcionado y lo sube a Cloudinary.
        /// </summary>
        /// <param name="content">Texto o datos a codificar dentro del código QR.</param>
        /// <param name="itemCode">Código del item para generar nombre único.</param>
        /// <param name="useShortId">Si true, usa un ID corto en lugar de GUID completo.</param>
        /// <returns>URL pública del archivo generado en Cloudinary.</returns>
        public string GenerateAndSaveQrCode(string content, string itemCode, bool useShortId = true)
        {
            // Generador de código QR con nivel de corrección de errores Q
            using var generator = new QRCodeGenerator();
            QRCodeData data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

            // Crear imagen a partir del QR generado (bytes en formato PNG)
            var qrCode = new PngByteQRCode(data);
            var pngBytes = qrCode.GetGraphic(20); // 20 = píxeles por módulo

            // Construir un nombre optimizado para el archivo
            string fileName = GenerateOptimizedFileName(itemCode, useShortId);

            // Subir el QR a Cloudinary usando un stream en memoria
            using var stream = new MemoryStream(pngBytes);
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                PublicId = $"{fileName}", // Carpeta "qr" más corta
                Overwrite = true,
                Folder = "CodexyQRs", // Nombre de carpeta más corto
                               // Optimizaciones adicionales de Cloudinary
                Transformation = new Transformation()
                    .Quality("auto:good") // Compresión automática optimizada
                    .FetchFormat("png")        // Formato específico
            };

            var uploadResult = _cloudinary.Upload(uploadParams);

            // Retorna la URL segura optimizada
            return uploadResult.SecureUrl.ToString();
        }

        /// <summary>
        /// Genera un nombre de archivo optimizado basado en diferentes estrategias.
        /// </summary>
        /// <param name="itemCode">Código del item.</param>
        /// <param name="useShortId">Si usar ID corto o timestamp.</param>
        /// <returns>Nombre de archivo optimizado.</returns>
        private string GenerateOptimizedFileName(string itemCode, bool useShortId)
        {
            if (useShortId)
            {
                // Opción 1: Usar un ID corto de 8 caracteres
                string shortId = GenerateShortId();
                return $"item_{itemCode}_{shortId}";
            }
            else
            {
                // Opción 2: Usar timestamp Unix para garantizar unicidad
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                return $"item_{itemCode}_{timestamp}";
            }
        }

        /// <summary>
        /// Genera un ID corto alfanumérico de 8 caracteres.
        /// </summary>
        /// <returns>String de 8 caracteres alfanuméricos.</returns>
        private string GenerateShortId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Versión alternativa que permite personalización completa del naming.
        /// </summary>
        /// <param name="content">Contenido del QR.</param>
        /// <param name="customOptions">Opciones personalizadas de naming.</param>
        /// <returns>URL del QR generado.</returns>
        public string GenerateAndSaveQrCodeWithOptions(string content, QrNamingOptions customOptions)
        {
            using var generator = new QRCodeGenerator();
            QRCodeData data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(data);
            var pngBytes = qrCode.GetGraphic(20);

            // Usar las opciones personalizadas
            string fileName = BuildCustomFileName(customOptions);

            using var stream = new MemoryStream(pngBytes);
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                PublicId = $"{customOptions.FolderName}/{fileName}",
                Overwrite = true,
                Folder = customOptions.FolderName
            };

            var uploadResult = _cloudinary.Upload(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }

        /// <summary>
        /// Construye el nombre del archivo basado en las opciones personalizadas.
        /// </summary>
        private string BuildCustomFileName(QrNamingOptions options)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(options.Prefix))
                parts.Add(options.Prefix);

            parts.Add(options.ItemCode);

            if (!string.IsNullOrEmpty(options.Suffix))
                parts.Add(options.Suffix);

            // Agregar identificador único según la estrategia elegida
            switch (options.UniqueStrategy)
            {
                case UniqueIdStrategy.ShortId:
                    parts.Add(GenerateShortId());
                    break;
                case UniqueIdStrategy.Timestamp:
                    parts.Add(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
                    break;
                case UniqueIdStrategy.DateTime:
                    parts.Add(DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                    break;
                case UniqueIdStrategy.None:
                    // No agregar identificador único (cuidado con duplicados)
                    break;
            }

            return string.Join("_", parts);
        }
    }

}

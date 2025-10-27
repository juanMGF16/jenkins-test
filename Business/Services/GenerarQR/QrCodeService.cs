using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Data.Repository.Interfaces.General;
using Entity.Context;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Text.RegularExpressions;

namespace Business.Services.GenerarQR
{
    /// <summary>
    /// Servicio de generación de códigos QR con almacenamiento en Cloudinary.
    /// Usa nombres completos tanto en URLs como en carpetas físicas.
    /// </summary>
    public class QrCodeService : IQrCodeService
    {
        private readonly Cloudinary _cloudinary;
        private readonly AppDbContext _context;

        public QrCodeService(Cloudinary cloudinary, AppDbContext context)
        {
            _cloudinary = cloudinary;
            _context = context;
        }

        /// <summary>
        /// Genera y guarda un código QR estructurado jerárquicamente (empresa/sucursal/zona)
        /// </summary>
        public string GenerateAndSaveQrCodeWithHierarchy(string content, int itemId, string itemCode, bool useShortId = true)
        {
            // Obtener jerarquia completa del ítem
            var hierarchy = GetItemHierarchy(itemId);

            // Generar el QR en memoria
            using var generator = new QRCodeGenerator();
            QRCodeData data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(data);
            var pngBytes = qrCode.GetGraphic(20);

            // Construir ruta única (nombres completos)
            string folderPath = BuildFolderPath(hierarchy);
            string fileName = GenerateOptimizedFileName(itemCode, useShortId);

            // Subir a Cloudinary
            using var stream = new MemoryStream(pngBytes);
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),

                // PublicId usando nombres completos (sin "CodexyQRs/" al inicio)
                PublicId = $"{folderPath.Replace("CodexyQRs/", string.Empty)}/{fileName}",

                // Folder físico con nombres completos
                Folder = folderPath,

                Overwrite = true,
                Transformation = new Transformation()
                    .Quality("auto:good")
                    .FetchFormat("png")
            };

            var uploadResult = _cloudinary.Upload(uploadParams);

            // Devolver la URL final segura
            return uploadResult.SecureUrl.ToString();
        }

        /// <summary>
        /// Obtiene la jerarquía completa del ítem (empresa, sucursal, zona)
        /// </summary>
        private ItemHierarchy GetItemHierarchy(int itemId)
        {
            var item = _context.Item
                .Include(i => i.Zone)
                    .ThenInclude(z => z.Branch)
                        .ThenInclude(b => b.Company)
                .FirstOrDefault(i => i.Id == itemId);

            if (item?.Zone?.Branch?.Company == null)
                throw new InvalidOperationException("No se pudo obtener la jerarquía completa del ítem");

            return new ItemHierarchy
            {
                CompanyId = item.Zone.Branch.Company.Id,
                CompanyName = SanitizeForFolder(item.Zone.Branch.Company.Name),
                BranchId = item.Zone.Branch.Id,
                BranchName = SanitizeForFolder(item.Zone.Branch.Name),
                ZoneId = item.Zone.Id,
                ZoneName = SanitizeForFolder(item.Zone.Name)
            };
        }

        /// <summary>
        /// Construye la ruta de carpeta usando nombres completos
        /// </summary>
        private string BuildFolderPath(ItemHierarchy hierarchy)
        {
            return $"CodexyQRs/{hierarchy.CompanyName}/{hierarchy.BranchName}/{hierarchy.ZoneName}";
        }

        /// <summary>
        /// Genera nombre de archivo optimizado
        /// </summary>
        private string GenerateOptimizedFileName(string itemCode, bool useShortId)
        {
            if (useShortId)
            {
                string shortId = GenerateShortId();
                return $"item_{itemCode}_{shortId}";
            }
            else
            {
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                return $"item_{itemCode}_{timestamp}";
            }
        }

        /// <summary>
        /// Genera un ID corto aleatorio
        /// </summary>
        private string GenerateShortId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Limpia nombres para usarlos como parte de una carpeta o ruta
        /// </summary>
        private string SanitizeForFolder(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "Unknown";

            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = string.Join("_", name.Split(invalidChars));
            sanitized = sanitized.Replace(" ", "_");
            sanitized = Regex.Replace(sanitized, @"_+", "_");
            sanitized = sanitized.Trim('_');

            return string.IsNullOrEmpty(sanitized) ? "Unknown" : sanitized;
        }

        /// <summary>
        /// Método original sin jerarquía (mantiene compatibilidad)
        /// </summary>
        public string GenerateAndSaveQrCode(string content, string itemCode, bool useShortId = true)
        {
            using var generator = new QRCodeGenerator();
            QRCodeData data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(data);
            var pngBytes = qrCode.GetGraphic(20);

            string fileName = GenerateOptimizedFileName(itemCode, useShortId);

            using var stream = new MemoryStream(pngBytes);
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                PublicId = $"CodexyQRs/Unorganized/{fileName}",
                Folder = "CodexyQRs/Unorganized",
                Overwrite = true,
                Transformation = new Transformation()
                    .Quality("auto:good")
                    .FetchFormat("png")
            };

            var uploadResult = _cloudinary.Upload(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }
    }

    /// <summary>
    /// DTO que representa la jerarquía de un ítem
    /// </summary>
    public class ItemHierarchy
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public int ZoneId { get; set; }
        public string ZoneName { get; set; } = string.Empty;
    }
}
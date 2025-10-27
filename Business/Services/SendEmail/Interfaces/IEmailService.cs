namespace Business.Services.SendEmail.Interfaces
{
    /// <summary>
    /// Define las operaciones para el envío de correos electrónicos a través del sistema.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envía un correo electrónico a un único destinatario.
        /// </summary>
        /// <param name="toEmail">La dirección de correo electrónico del destinatario.</param>
        /// <param name="subject">El asunto del correo.</param>
        /// <param name="body">El contenido del correo.</param>
        /// <param name="isHtml">Indica si el cuerpo del correo está en formato HTML (<c>true</c>) o texto plano (<c>false</c>).</param>
        /// <returns>Una tarea que retorna <c>true</c> si el envío fue exitoso; de lo contrario, <c>false</c>.</returns>
        Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false);

        /// <summary>
        /// Envía un correo electrónico a múltiples destinatarios.
        /// </summary>
        /// <param name="toEmails">Una lista de direcciones de correo electrónico de los destinatarios.</param>
        /// <param name="subject">El asunto del correo.</param>
        /// <param name="body">El contenido del correo.</param>
        /// <param name="isHtml">Indica si el cuerpo del correo está en formato HTML (<c>true</c>) o texto plano (<c>false</c>).</param>
        /// <returns>Una tarea que retorna <c>true</c> si el envío fue exitoso; de lo contrario, <c>false</c>.</returns>
        Task<bool> SendEmailAsync(List<string> toEmails, string subject, string body, bool isHtml = false);

        /// <summary>
        /// Envía un correo electrónico a un único destinatario con un archivo adjunto.
        /// </summary>
        /// <param name="toEmail">La dirección de correo electrónico del destinatario.</param>
        /// <param name="subject">El asunto del correo.</param>
        /// <param name="body">El contenido del correo.</param>
        /// <param name="attachment">El contenido del archivo adjunto como un array de bytes.</param>
        /// <param name="attachmentName">El nombre del archivo adjunto (ej. "reporte.pdf").</param>
        /// <param name="isHtml">Indica si el cuerpo del correo está en formato HTML (<c>true</c>) o texto plano (<c>false</c>).</param>
        /// <returns>Una tarea que retorna <c>true</c> si el envío fue exitoso; de lo contrario, <c>false</c>.</returns>
        Task<bool> SendEmailWithAttachmentAsync(string toEmail, string subject, string body, byte[] attachment, string attachmentName, bool isHtml = false);
    }
}

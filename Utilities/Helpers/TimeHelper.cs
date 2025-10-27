namespace Utilities.Helpers
{
    /// <summary>
    /// Helper para conversión de zonas horarias entre UTC y hora de Bogotá
    /// </summary>
    public static class TimeHelper
    {

        public static readonly TimeZoneInfo TzBogota =
            TimeZoneInfo.TryFindSystemTimeZoneById("America/Bogota", out var tz)
                ? tz
                : TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

        /// <summary>
        /// Convierte una fecha UTC a DateTimeOffset con zona horaria de Bogotá
        /// </summary>
        /// <param name="utc">Fecha en UTC</param>
        /// <returns>DateTimeOffset con offset de Bogotá (-05:00)</returns>
        public static DateTimeOffset ToBogotaOffset(DateTime utc)
        {
            var u = utc.Kind == DateTimeKind.Utc ? utc : DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            var local = TimeZoneInfo.ConvertTimeFromUtc(u, TzBogota);
            var offset = TzBogota.GetUtcOffset(u);
            return new DateTimeOffset(local, offset); // -05:00
        }

        /// <summary>
        /// Convierte un DateTimeOffset a DateTime en UTC
        /// </summary>
        /// <param name="dto">DateTimeOffset a convertir</param>
        /// <returns>DateTime en UTC</returns>
        public static DateTime ToUtcDateTime(DateTimeOffset dto)
            => dto.ToUniversalTime().UtcDateTime;
    }
}

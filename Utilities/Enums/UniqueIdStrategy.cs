namespace Utilities.Enums
{
    public enum UniqueIdStrategy
    {
        ShortId,    // ID alfanumérico de 8 caracteres: A1B2C3D4
        Timestamp,  // Unix timestamp: 1758171880
        DateTime,   // Fecha y hora: 20241218_143022
        None        // Sin identificador único (puede causar duplicados)
    }
}

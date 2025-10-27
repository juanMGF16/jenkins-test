namespace Business.Services.CredentialGenerator.Interfaces
{
    /// <summary>
    /// Define las operaciones para generar credenciales de acceso (nombre de usuario y contraseña)
    /// de manera predeterminada para nuevos usuarios.
    /// </summary>
    public interface ICredentialGeneratorService
    {
        /// <summary>
        /// Genera un nombre de usuario basado en el nombre y apellido, y una contraseña
        /// aleatoria con formato predefinido.
        /// </summary>
        /// <param name="name">El nombre de la persona.</param>
        /// <param name="lastName">El apellido de la persona.</param>
        /// <param name="email">El email de la persona (no usado en el algoritmo actual, pero mantenido como contexto).</param>
        /// <returns>Una tupla que contiene el nombre de usuario y la contraseña generados.</returns>
        (string username, string password) GenerateCredentials(string name, string lastName, string email);
    }
}

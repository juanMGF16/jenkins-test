namespace Data.Repository.Interfaces.General
{
    /// <summary>
    /// Interfaz para operaciones generales extendidas
    /// </summary>
    public interface IGeneral<T> : IGetTotalGeneral<T>
        where T : class
    {
    }
}
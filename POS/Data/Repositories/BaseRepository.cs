namespace POS.Data.Repositories
{
    /// <summary>
    /// Clase base para los repositorios de acceso a datos.
    /// Centraliza la cadena de conexión a SQLite para evitar su duplicación.
    /// Activa las llaves foráneas para garantizar la integridad referencial.
    /// </summary>
    public abstract class BaseRepository
    {
        public const string ConnectionString = "Data Source=POS.db;Foreign Keys=True";
    }
}
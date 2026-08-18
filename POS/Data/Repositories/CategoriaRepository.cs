using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Dapper;
using POS.Models;

namespace POS.Data.Repositories
{
    /// <summary>
    /// Repositorio para la gestión de acceso a datos de las Categorías.
    /// </summary>
    public class CategoriaRepository : BaseRepository
    {

        /// <summary>
        /// Inserta una nueva categoría en la base de datos de SQLite.
        /// </summary>
        /// <param name="categoria">El objeto categoría a insertar.</param>
        public void AgregarCategoria(Categoria categoria)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var sql = "INSERT INTO Categorias (id_categoria, nombre) VALUES (@IdCategoria, @Nombre)";
                connection.Execute(sql, categoria);
            }
        }

        /// <summary>
        /// Obtiene todas las categorías de la base de datos.
        /// </summary>
        public IEnumerable<Categoria> ObtenerCategorias()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var sql = "SELECT id_categoria AS IdCategoria, nombre FROM Categorias";
                return connection.Query<Categoria>(sql);
            }
        }

        /// <summary>
        /// Obtiene una categoría específica mediante su Id.
        /// </summary>
        public Categoria ObtenerCategoriaPorId(string idCategoria)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var sql = "SELECT id_categoria AS IdCategoria, nombre FROM Categorias WHERE id_categoria = @IdCategoria";
                return connection.QueryFirstOrDefault<Categoria>(sql, new { IdCategoria = idCategoria });
            }
        }

        /// <summary>
        /// Actualiza el nombre de una categoría existente.
        /// </summary>
        public void EditarCategoria(Categoria categoria)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var sql = "UPDATE Categorias SET nombre = @Nombre WHERE id_categoria = @IdCategoria";
                connection.Execute(sql, categoria);
            }
        }

        /// <summary>
        /// Elimina una categoría de la base de datos.
        /// </summary>
        public void EliminarCategoria(string idCategoria)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var sql = "DELETE FROM Categorias WHERE id_categoria = @IdCategoria";
                connection.Execute(sql, new { IdCategoria = idCategoria });
            }
        }
    }
}

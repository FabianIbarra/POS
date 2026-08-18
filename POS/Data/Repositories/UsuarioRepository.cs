using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Dapper;
using POS.Models;

namespace POS.Data.Repositories
{
    /// <summary>
    /// Repositorio para la gestión de acceso a datos de los Usuarios apoyándose de Dapper.
    /// </summary>
    public class UsuarioRepository : BaseRepository
    {

        /// <summary>
        /// Agrega un nuevo usuario a la base de datos.
        /// </summary>
        public void AgregarUsuario(Usuario usuario)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var sql = @"INSERT INTO Usuarios (id_usuario, username, password_hash, nombre_completo, rol) 
                            VALUES (@IdUsuario, @Username, @PasswordHash, @NombreCompleto, @Rol)";
                connection.Execute(sql, usuario);
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios.
        /// </summary>
        public IEnumerable<Usuario> ObtenerUsuarios()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var sql = "SELECT id_usuario AS IdUsuario, username, password_hash AS PasswordHash, nombre_completo AS NombreCompleto, rol FROM Usuarios";
                return connection.Query<Usuario>(sql);
            }
        }

        /// <summary>
        /// Obtiene un usuario mediante su Id.
        /// </summary>
        public Usuario ObtenerUsuarioPorId(string idUsuario)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var sql = "SELECT id_usuario AS IdUsuario, username, password_hash AS PasswordHash, nombre_completo AS NombreCompleto, rol FROM Usuarios WHERE id_usuario = @IdUsuario";
                return connection.QueryFirstOrDefault<Usuario>(sql, new { IdUsuario = idUsuario });
            }
        }

        /// <summary>
        /// Obtiene un usuario mediante su nombre de usuario.
        /// </summary>
        /// <param name="username">Nombre de usuario a buscar.</param>
        /// <returns>La entidad de usuario encontrada, o null si no existe.</returns>
        public Usuario ObtenerUsuarioPorUsername(string username)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var sql = "SELECT id_usuario AS IdUsuario, username, password_hash AS PasswordHash, nombre_completo AS NombreCompleto, rol FROM Usuarios WHERE username = @Username";
                return connection.QueryFirstOrDefault<Usuario>(sql, new { Username = username });
            }
        }

        /// <summary>
        /// Edita la información de un usuario existente.
        /// </summary>
        public void EditarUsuario(Usuario usuario)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var sql = @"UPDATE Usuarios SET 
                            username = @Username, 
                            password_hash = @PasswordHash, 
                            nombre_completo = @NombreCompleto, 
                            rol = @Rol 
                            WHERE id_usuario = @IdUsuario";
                connection.Execute(sql, usuario);
            }
        }

        /// <summary>
        /// Elimina un usuario de la base de datos de manera física.
        /// </summary>
        public void EliminarUsuario(string idUsuario)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var sql = "DELETE FROM Usuarios WHERE id_usuario = @IdUsuario";
                connection.Execute(sql, new { IdUsuario = idUsuario });
            }
        }
    }
}

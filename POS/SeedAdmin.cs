using POS.Data.Repositories;
using POS.Models;

namespace POS
{
    /// <summary>
    /// Utilidad que verifica si existe al menos un usuario en la base de datos.
    /// Si no existe ninguno, inserta un usuario administrador por defecto.
    /// </summary>
    public static class SeedAdmin
    {
        private const string Username = "admin";
        private const string Password = "admin123";
        private const string NombreCompleto = "Administrador del Sistema";
        private const string Rol = "Administrador";

        /// <summary>
        /// Verifica la tabla Usuarios y, si está vacía, inserta un usuario administrador por defecto.
        /// </summary>
        public static void VerificarYSembrar()
        {
            var repo = new UsuarioRepository();
            var usuarios = repo.ObtenerUsuarios();

            if (usuarios.Any()) return;

            var usuario = new Usuario
            {
                IdUsuario = Guid.NewGuid().ToString(),
                Username = Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password),
                NombreCompleto = NombreCompleto,
                Rol = Rol
            };

            repo.AgregarUsuario(usuario);
        }
    }
}

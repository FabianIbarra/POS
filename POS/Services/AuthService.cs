using System;
using POS.Models;
using POS.Data.Repositories;

namespace POS.Services
{
    /// <summary>
    /// Servicio responsable de gestionar la autenticación de usuarios.
    /// Contiene la lógica para validar credenciales y almacena el estado global de la sesión.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly UsuarioRepository _usuarioRepository;

        // Singleton que retiene la información del usuario autenticado en la sesión actual
        public static AuthSession? SesionActual { get; set; }

        public AuthService()
        {
            _usuarioRepository = new UsuarioRepository();
        }

        /// <summary>
        /// Intenta autenticar a un usuario cruzando sus credenciales contra la base de datos.
        /// Utiliza BCrypt para comparar el hash.
        /// </summary>
        /// <param name="username">El nombre de usuario ingresado.</param>
        /// <param name="passwordPlan">La contraseña en texto plano ingresada.</param>
        /// <returns>Verdadero si las credenciales son correctas; Falso en caso contrario.</returns>
        public bool IniciarSesion(string username, string passwordPlan)
        {
            var usuario = _usuarioRepository.ObtenerUsuarioPorUsername(username);

            if (usuario == null)
            {
                return false;
            }

            // Comparamos el hash de la BD con el texto plano proporcionado, usando BCrypt
            bool esValido = BCrypt.Net.BCrypt.Verify(passwordPlan, usuario.PasswordHash);

            if (esValido)
            {
                // Guardamos el estado global de la sesión
                SesionActual = new AuthSession
                {
                    IdUsuario = usuario.IdUsuario,
                    Username = usuario.Username,
                    NombreCompleto = usuario.NombreCompleto,
                    Rol = usuario.Rol
                };
                return true;
            }

            return false;
        }

        /// <summary>
        /// Limpia la sesión actual del sistema.
        /// </summary>
        public void CerrarSesion()
        {
            SesionActual = null;
        }
    }

    /// <summary>
    /// Estructura para almacenar de manera global y en memoria el usuario en turno.
    /// </summary>
    public class AuthSession
    {
        public string? IdUsuario { get; set; }
        public string? Username { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Rol { get; set; }
    }
}

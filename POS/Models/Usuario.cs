using System;

namespace POS.Models
{
    /// <summary>
    /// Modelo de dominio para la entidad Usuario.
    /// Mapea la tabla Usuarios en la base de datos SQLite.
    /// </summary>
    public class Usuario
    {
        public string? IdUsuario { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Rol { get; set; }
    }
}

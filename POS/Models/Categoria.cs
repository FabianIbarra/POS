using System;

namespace POS.Models
{
    /// <summary>
    /// Modelo de dominio para la entidad Categoría.
    /// Mapea la tabla Categorias en la base de datos SQLite.
    /// </summary>
    public class Categoria
    {
        public string IdCategoria { get; set; }
        public string Nombre { get; set; }
    }
}

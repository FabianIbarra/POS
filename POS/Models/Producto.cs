using System;

namespace POS.Models
{
    /// <summary>
    /// Modelo de dominio para la entidad Producto.
    /// Mapea la tabla Productos en la base de datos SQLite.
    /// </summary>
    public class Producto
    {
        public string IdProducto { get; set; }
        public string CodigoBarras { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Stock { get; set; }
        public int Disponible { get; set; } = 1;
        public string IdCategoria { get; set; }
    }
}

using System;

namespace POS.Models
{
    /// <summary>
    /// Modelo de dominio para la entidad Detalles_Venta.
    /// Mapea la tabla Detalles_Venta en la base de datos SQLite.
    /// </summary>
    public class DetalleVenta
    {
        public string? IdDetalle { get; set; }
        public string? IdVenta { get; set; }
        public string? IdProducto { get; set; }

        // Propiedad de apoyo exclusiva para mostrar en la interfaz (UI)
        public string? Descripcion { get; set; }

        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}

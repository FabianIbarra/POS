using System;

namespace POS.Models
{
    /// <summary>
    /// Modelo de dominio para la entidad Venta.
    /// Mapea la tabla Ventas en la base de datos SQLite.
    /// </summary>
    public class Venta
    {
        public string IdVenta { get; set; }
        public int Folio { get; set; }
        public string FechaHora { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }
        public string IdUsuario { get; set; }
    }
}

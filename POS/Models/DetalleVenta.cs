using CommunityToolkit.Mvvm.ComponentModel;

namespace POS.Models
{
    /// <summary>
    /// Modelo de dominio para la entidad Detalles_Venta.
    /// Mapea la tabla Detalles_Venta en la base de datos SQLite.
    /// Incluye notificación de cambios para actualizar la UI en tiempo real.
    /// </summary>
    public partial class DetalleVenta : ObservableObject
    {
        public string? IdDetalle { get; set; }
        public string? IdVenta { get; set; }
        public string? IdProducto { get; set; }

        public string? Descripcion { get; set; }

        [ObservableProperty]
        private decimal _cantidad;

        public decimal PrecioUnitario { get; set; }

        [ObservableProperty]
        private decimal _subtotal;

        partial void OnCantidadChanged(decimal value)
        {
            Subtotal = value * PrecioUnitario;
        }
    }
}

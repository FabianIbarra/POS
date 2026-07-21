using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Models;
using POS.Data.Repositories;
using POS.Services;

namespace POS.ViewModels
{
    public partial class POSViewModel : ObservableObject
    {
        private readonly ProductoRepository _productoRepo;
        private readonly VentaRepository _ventaRepo;

        [ObservableProperty]
        private ObservableCollection<DetalleVenta> _carrito;

        [ObservableProperty]
        private string _codigoEscaneado;

        [ObservableProperty]
        private decimal _totalVenta;

        public POSViewModel()
        {
            _productoRepo = new ProductoRepository();
            _ventaRepo = new VentaRepository();
            Carrito = new ObservableCollection<DetalleVenta>();
            CodigoEscaneado = string.Empty;
            CalcularTotal();
        }

        [RelayCommand]
        private void AgregarProducto()
        {
            if (string.IsNullOrWhiteSpace(CodigoEscaneado)) return;

            // Buscamos el producto por código de barras usando LINQ sobre los activos
            var producto = _productoRepo.ObtenerProductosActivos()
                            .FirstOrDefault(p => p.CodigoBarras == CodigoEscaneado);

            if (producto == null)
            {
                MessageBox.Show("Producto no encontrado.", "Búsqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                CodigoEscaneado = string.Empty;
                return;
            }

            // Verificamos si ya está en el carrito
            var detalleExistente = Carrito.FirstOrDefault(d => d.IdProducto == producto.IdProducto);

            if (detalleExistente != null)
            {
                detalleExistente.Cantidad += 1;
                detalleExistente.Subtotal = detalleExistente.Cantidad * detalleExistente.PrecioUnitario;

                if (detalleExistente.Cantidad > producto.Stock)
                {
                    MessageBox.Show($"¡Advertencia! El stock actual es de {producto.Stock}. La venta se permitirá dejando el inventario en negativo.", "Stock Insuficiente", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                if (producto.Stock < 1)
                {
                    MessageBox.Show($"¡Advertencia! Producto sin stock. La venta se permitirá dejando el inventario en negativo.", "Stock Insuficiente", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Carrito.Add(new DetalleVenta
                {
                    IdDetalle = Guid.NewGuid().ToString(),
                    IdProducto = producto.IdProducto,
                    Descripcion = producto.Descripcion,
                    Cantidad = 1,
                    PrecioUnitario = producto.PrecioVenta,
                    Subtotal = producto.PrecioVenta
                });
            }

            // Forzamos la actualización visual de la lista y el total
            var temp = Carrito.ToList();
            Carrito.Clear();
            foreach (var item in temp) Carrito.Add(item);

            CalcularTotal();
            CodigoEscaneado = string.Empty;
        }

        [RelayCommand]
        private void CobrarVenta()
        {
            if (!Carrito.Any())
            {
                MessageBox.Show("No se puede cobrar un carrito vacío.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"¿Desea procesar el cobro por {TotalVenta:C}?", "Confirmar Venta", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var nuevaVenta = new Venta
                    {
                        IdVenta = Guid.NewGuid().ToString(),
                        FechaHora = TimeService.ObtenerHoraLocalComoString(),
                        Total = TotalVenta,
                        MetodoPago = "Efectivo", // Por simplicidad, fijo. Se podría hacer dinámico después.
                        IdUsuario = AuthService.SesionActual?.IdUsuario
                    };

                    // Se asigna el IdVenta a todos los detalles antes de mandarlos al Repositorio
                    foreach (var detalle in Carrito)
                    {
                        detalle.IdVenta = nuevaVenta.IdVenta;
                    }

                    // Ejecutamos la transacción
                    _ventaRepo.RegistrarVenta(nuevaVenta, Carrito);

                    MessageBox.Show($"Venta registrada exitosamente. Folio: {nuevaVenta.Folio}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    CancelarVenta(); // Limpia el carrito
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error al procesar la venta: {ex.Message}", "Error de Transacción", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void CancelarVenta()
        {
            if (Carrito.Any())
            {
                Carrito.Clear();
                CalcularTotal();
                CodigoEscaneado = string.Empty;
            }
        }

        private void CalcularTotal()
        {
            TotalVenta = Carrito.Sum(d => d.Subtotal);
        }
    }
}

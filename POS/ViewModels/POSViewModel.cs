using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Models;
using POS.Data.Repositories;
using POS.Services;
using POS.Views;

namespace POS.ViewModels
{
    /// <summary>
    /// ViewModel del modulo de Punto de Venta. Gestiona el carrito de compras,
    /// el escaneo de productos, el cálculo de totales con IVA desglosado,
    /// la modificación de cantidades y la ejecución del cobro transaccional.
    /// </summary>
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

        /// <summary>
        /// Obtiene o establece el subtotal de la venta sin incluir el IVA.
        /// Se calcula automáticamente como TotalVenta / 1.16.
        /// </summary>
        [ObservableProperty]
        private decimal _subtotalSinIVA;

        /// <summary>
        /// Obtiene o establece el monto del IVA (16 %) de la venta.
        /// Se calcula automáticamente como TotalVenta - SubtotalSinIVA.
        /// </summary>
        [ObservableProperty]
        private decimal _iva;

        /// <summary>
        /// Obtiene o establece el renglón del carrito actualmente seleccionado.
        /// Se utiliza para las operaciones de incrementar, decrementar y eliminar.
        /// </summary>
        [ObservableProperty]
        private DetalleVenta? _seleccionado;

        public POSViewModel()
        {
            _productoRepo = new ProductoRepository();
            _ventaRepo = new VentaRepository();
            Carrito = new ObservableCollection<DetalleVenta>();
            CodigoEscaneado = string.Empty;
            Carrito.CollectionChanged += Carrito_CollectionChanged;
            CalcularTotal();
        }

        partial void OnTotalVentaChanged(decimal value)
        {
            SubtotalSinIVA = value / 1.16m;
            Iva = value - SubtotalSinIVA;
        }

        private void Carrito_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (DetalleVenta item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (DetalleVenta item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DetalleVenta.Subtotal) ||
                e.PropertyName == nameof(DetalleVenta.Cantidad))
            {
                CalcularTotal();
            }
        }

        [RelayCommand]
        private void AgregarProducto()
        {
            if (string.IsNullOrWhiteSpace(CodigoEscaneado)) return;

            var producto = _productoRepo.ObtenerProductosActivos()
                            .FirstOrDefault(p => p.CodigoBarras == CodigoEscaneado);

            if (producto == null)
            {
                MessageBox.Show("Producto no encontrado.", "Búsqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                CodigoEscaneado = string.Empty;
                return;
            }

            var detalleExistente = Carrito.FirstOrDefault(d => d.IdProducto == producto.IdProducto);

            if (detalleExistente != null)
            {
                detalleExistente.Cantidad += 1;

                if (detalleExistente.Cantidad > producto.Stock)
                {
                    MessageBox.Show(
                        $"Advertencia: el stock actual es de {producto.Stock}. La venta se permitirá dejando el inventario en negativo.",
                        "Stock Insuficiente",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            else
            {
                if (producto.Stock < 1)
                {
                    MessageBox.Show(
                        $"Advertencia: el stock actual es de {producto.Stock}. La venta se permitirá dejando el inventario en negativo.",
                        "Stock Insuficiente",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
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

            CalcularTotal();
            CodigoEscaneado = string.Empty;
        }

        /// <summary>
        /// Comando que incrementa en uno la cantidad del producto seleccionado en el carrito.
        /// Muestra una advertencia si el stock en base de datos es insuficiente.
        /// Atajo de teclado: tecla +.
        /// </summary>
        [RelayCommand]
        private void IncrementarCantidad()
        {
            if (Seleccionado == null) return;

            Seleccionado.Cantidad += 1;

            var producto = _productoRepo.ObtenerProductosActivos()
                .FirstOrDefault(p => p.IdProducto == Seleccionado.IdProducto);

            if (producto != null && Seleccionado.Cantidad > producto.Stock)
            {
                MessageBox.Show(
                    $"Advertencia: el stock actual es de {producto.Stock}. La venta se permitirá dejando el inventario en negativo.",
                    "Stock Insuficiente",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Comando que decrementa en uno la cantidad del producto seleccionado.
        /// Si la cantidad llega a cero, elimina el renglón del carrito.
        /// Atajo de teclado: tecla -.
        /// </summary>
        [RelayCommand]
        private void DecrementarCantidad()
        {
            if (Seleccionado == null) return;

            if (Seleccionado.Cantidad <= 1)
            {
                Carrito.Remove(Seleccionado);
                Seleccionado = Carrito.LastOrDefault();
                CalcularTotal();
            }
            else
            {
                Seleccionado.Cantidad -= 1;
            }
        }

        /// <summary>
        /// Comando que desplaza la selección al producto anterior en el carrito.
        /// Atajo de teclado: flecha Arriba.
        /// </summary>
        [RelayCommand]
        private void SeleccionarAnterior()
        {
            if (Carrito.Count == 0) return;

            int indiceActual = Seleccionado != null
                ? Carrito.IndexOf(Seleccionado)
                : Carrito.Count;

            Seleccionado = Carrito[Math.Max(0, indiceActual - 1)];
        }

        /// <summary>
        /// Comando que desplaza la selección al producto siguiente en el carrito.
        /// Atajo de teclado: flecha Abajo.
        /// </summary>
        [RelayCommand]
        private void SeleccionarSiguiente()
        {
            if (Carrito.Count == 0) return;

            int indiceActual = Seleccionado != null
                ? Carrito.IndexOf(Seleccionado)
                : -1;

            Seleccionado = Carrito[Math.Min(Carrito.Count - 1, indiceActual + 1)];
        }

        /// <summary>
        /// Comando que elimina por completo el renglón seleccionado del carrito.
        /// Selecciona automáticamente el renglón siguiente o anterior.
        /// Atajo de teclado: tecla Suprimir (Delete).
        /// </summary>
        [RelayCommand]
        private void EliminarRenglon()
        {
            if (Seleccionado == null) return;

            var indice = Carrito.IndexOf(Seleccionado);
            Carrito.Remove(Seleccionado);
            Seleccionado = Carrito.Count > 0
                ? Carrito[Math.Min(indice, Carrito.Count - 1)]
                : null;
            CalcularTotal();
        }

        [RelayCommand]
        private void CobrarVenta()
        {
            if (!Carrito.Any())
            {
                MessageBox.Show("No se puede cobrar un carrito vacío.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var cobroVM = new CobroViewModel(TotalVenta);
            var cobroView = new CobroView(cobroVM)
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
            };

            cobroView.ShowDialog();

            if (!cobroVM.Confirmado) return;

            try
            {
                var nuevaVenta = new Venta
                {
                    IdVenta = Guid.NewGuid().ToString(),
                    FechaHora = TimeService.ObtenerHoraLocalComoString(),
                    Total = TotalVenta,
                    MetodoPago = "Efectivo",
                    IdUsuario = AuthService.SesionActual?.IdUsuario
                };

                foreach (var detalle in Carrito)
                {
                    detalle.IdVenta = nuevaVenta.IdVenta;
                }

                _ventaRepo.RegistrarVenta(nuevaVenta, Carrito);

                MessageBox.Show($"Venta registrada exitosamente. Folio: {nuevaVenta.Folio}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CancelarVenta();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al procesar la venta: {ex.Message}", "Error de Transacción", MessageBoxButton.OK, MessageBoxImage.Error);
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
                Seleccionado = null;
            }
        }

        private void CalcularTotal()
        {
            TotalVenta = Carrito.Sum(d => d.Subtotal);
        }
    }
}

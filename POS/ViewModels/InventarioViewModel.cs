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

namespace POS.ViewModels
{
    /// 
    /// ViewModel para la gestión del inventario. Controla el CRUD de productos 
    /// y la vinculación de datos con la vista, sin acoplamiento a la UI.
    /// 
    public partial class InventarioViewModel : ObservableObject
    {
        private readonly ProductoRepository _productoRepo;
        private readonly CategoriaRepository _categoriaRepo;

        [ObservableProperty]
        private ObservableCollection<Producto> _productos;

        [ObservableProperty]
        private ObservableCollection<Categoria> _categorias;

        // Propiedades del formulario
        [ObservableProperty] private string _idProducto;
        [ObservableProperty] private string _codigoBarras;
        [ObservableProperty] private string _descripcion;
        [ObservableProperty] private decimal _precioCompra;
        [ObservableProperty] private decimal _precioVenta;
        [ObservableProperty] private decimal _stock;
        [ObservableProperty] private Categoria _categoriaSeleccionada;

        [ObservableProperty]
        private string _textoBusqueda;

        public InventarioViewModel()
        {
            _productoRepo = new ProductoRepository();
            _categoriaRepo = new CategoriaRepository();
            Productos = new ObservableCollection<Producto>();
            Categorias = new ObservableCollection<Categoria>();
            CargarDatos();
        }

        [RelayCommand]
        private void CargarDatos()
        {
            var cats = _categoriaRepo.ObtenerCategorias();
            Categorias = new ObservableCollection<Categoria>(cats);
            BuscarProductos();
        }

        /// 
        /// Método interceptor de CommunityToolkit que se dispara automáticamente
        /// cada vez que el usuario teclea en el buscador.
        /// 
        partial void OnTextoBusquedaChanged(string value)
        {
            BuscarProductos();
        }

        private void BuscarProductos()
        {
            // Usamos tu método exacto que ya filtra los inactivos
            var prods = _productoRepo.ObtenerProductosActivos();

            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                prods = prods.Where(p =>
                    p.Descripcion.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                    p.CodigoBarras.Contains(TextoBusqueda));
            }

            Productos = new ObservableCollection<Producto>(prods);
        }

        [RelayCommand]
        private void GuardarProducto()
        {
            if (string.IsNullOrWhiteSpace(CodigoBarras) || string.IsNullOrWhiteSpace(Descripcion) || CategoriaSeleccionada == null)
            {
                MessageBox.Show("Por favor, llene los campos obligatorios (Código, Descripción y Categoría).", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PrecioVenta <= PrecioCompra)
            {
                MessageBox.Show("El precio de venta debe ser estrictamente mayor al precio de compra.", "Validación de Margen", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var producto = new Producto
            {
                IdProducto = string.IsNullOrEmpty(IdProducto) ? Guid.NewGuid().ToString() : IdProducto,
                CodigoBarras = CodigoBarras,
                Descripcion = Descripcion,
                PrecioCompra = PrecioCompra,
                PrecioVenta = PrecioVenta,
                Stock = Stock,
                Disponible = 1,
                IdCategoria = CategoriaSeleccionada.IdCategoria
            };

            try
            {
                if (string.IsNullOrEmpty(IdProducto))
                {
                    _productoRepo.AgregarProducto(producto);
                }
                else
                {
                    _productoRepo.EditarProducto(producto);
                }

                LimpiarFormulario();
                CargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar en base de datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void SeleccionarProducto(Producto producto)
        {
            if (producto == null) return;

            IdProducto = producto.IdProducto;
            CodigoBarras = producto.CodigoBarras;
            Descripcion = producto.Descripcion;
            PrecioCompra = producto.PrecioCompra;
            PrecioVenta = producto.PrecioVenta;
            Stock = producto.Stock;
            CategoriaSeleccionada = Categorias.FirstOrDefault(c => c.IdCategoria == producto.IdCategoria);
        }

        [RelayCommand]
        private void EliminarProducto(Producto producto)
        {
            if (producto == null) return;

            var result = MessageBox.Show($"¿Desea eliminar el producto: {producto.Descripcion}?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                // Usamos el nombre exacto de tu repositorio
                _productoRepo.EliminarProductoLogicamente(producto.IdProducto);
                CargarDatos();
            }
        }

        [RelayCommand]
        private void LimpiarFormulario()
        {
            IdProducto = string.Empty;
            CodigoBarras = string.Empty;
            Descripcion = string.Empty;
            PrecioCompra = 0;
            PrecioVenta = 0;
            Stock = 0;
            CategoriaSeleccionada = null;
        }

        // ==========================================
        // PROPIEDADES PARA EL CRUD DE CATEGORÍAS
        // ==========================================
        [ObservableProperty] private string _idCategoriaEdicion;
        [ObservableProperty] private string _nombreCategoria;

        // ==========================================
        // COMANDOS PARA EL CRUD DE CATEGORÍAS
        // ==========================================

        [RelayCommand]
        private void GuardarCategoria()
        {
            if (string.IsNullOrWhiteSpace(NombreCategoria))
            {
                MessageBox.Show("El nombre de la categoría es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var categoria = new Categoria
            {
                // Generamos un GUID como string tal como lo dictan las reglas del proyecto
                IdCategoria = string.IsNullOrEmpty(IdCategoriaEdicion) ? Guid.NewGuid().ToString() : IdCategoriaEdicion,
                Nombre = NombreCategoria
            };

            try
            {
                if (string.IsNullOrEmpty(IdCategoriaEdicion))
                {
                    _categoriaRepo.AgregarCategoria(categoria);
                }
                else
                {
                    _categoriaRepo.EditarCategoria(categoria);
                }

                LimpiarFormularioCategoria();
                CargarDatos(); // Esto refrescará tanto la tabla de categorías como el ComboBox de productos
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar categoría: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void SeleccionarCategoriaParaEdicion(Categoria categoria)
        {
            if (categoria == null) return;

            IdCategoriaEdicion = categoria.IdCategoria;
            NombreCategoria = categoria.Nombre;
        }

        [RelayCommand]
        private void EliminarCategoria(Categoria categoria)
        {
            if (categoria == null) return;

            var result = MessageBox.Show($"¿Desea eliminar la categoría: {categoria.Nombre}?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _categoriaRepo.EliminarCategoria(categoria.IdCategoria);
                    CargarDatos();
                }
                catch (Exception)
                {
                    // Manejo de error en caso de que la categoría esté siendo usada por un producto (Restricción de llave foránea SQLite)
                    MessageBox.Show("No se puede eliminar esta categoría porque tiene productos asociados.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void LimpiarFormularioCategoria()
        {
            IdCategoriaEdicion = string.Empty;
            NombreCategoria = string.Empty;
        }
    }
}

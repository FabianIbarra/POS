using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Dapper;
using POS.Models;

namespace POS.Data.Repositories
{
    /// <summary>
    /// Repositorio para la gestión de acceso a datos de los Productos apoyándose de Dapper.
    /// Utiliza un borrado lógico actualizando la bandera 'disponible'.
    /// </summary>
    public class ProductoRepository : BaseRepository
    {

        /// <summary>
        /// Inserta un nuevo producto en la base de datos.
        /// </summary>
        public void AgregarProducto(Producto producto)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var sql = @"INSERT INTO Productos (id_producto, codigo_barras, descripcion, precio_compra, precio_venta, stock, disponible, id_categoria) 
                            VALUES (@IdProducto, @CodigoBarras, @Descripcion, @PrecioCompra, @PrecioVenta, @Stock, @Disponible, @IdCategoria)";
                connection.Execute(sql, producto);
            }
        }

        /// <summary>
        /// Obtiene todos los productos que están activos (disponible = 1).
        /// </summary>
        public IEnumerable<Producto> ObtenerProductosActivos()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var sql = "SELECT id_producto AS IdProducto, codigo_barras AS CodigoBarras, descripcion, precio_compra AS PrecioCompra, precio_venta AS PrecioVenta, stock, disponible, id_categoria AS IdCategoria FROM Productos WHERE disponible = 1";
                return connection.Query<Producto>(sql);
            }
        }

        /// <summary>
        /// Obtiene un producto por su Id.
        /// </summary>
        public Producto ObtenerProductoPorId(string idProducto)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var sql = "SELECT id_producto AS IdProducto, codigo_barras AS CodigoBarras, descripcion, precio_compra AS PrecioCompra, precio_venta AS PrecioVenta, stock, disponible, id_categoria AS IdCategoria FROM Productos WHERE id_producto = @IdProducto";
                return connection.QueryFirstOrDefault<Producto>(sql, new { IdProducto = idProducto });
            }
        }

        /// <summary>
        /// Actualiza la información de un producto.
        /// </summary>
        public void EditarProducto(Producto producto)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var sql = @"UPDATE Productos SET 
                            codigo_barras = @CodigoBarras, 
                            descripcion = @Descripcion, 
                            precio_compra = @PrecioCompra, 
                            precio_venta = @PrecioVenta, 
                            stock = @Stock, 
                            disponible = @Disponible, 
                            id_categoria = @IdCategoria 
                            WHERE id_producto = @IdProducto";
                connection.Execute(sql, producto);
            }
        }

        /// <summary>
        /// Borrado lógico estableciendo el campo disponible a 0
        /// </summary>
        public void EliminarProductoLogicamente(string idProducto)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var sql = "UPDATE Productos SET disponible = 0 WHERE id_producto = @IdProducto";
                connection.Execute(sql, new { IdProducto = idProducto });
            }
        }
    }
}

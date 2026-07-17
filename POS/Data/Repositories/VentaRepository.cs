using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Dapper;
using POS.Models;

namespace POS.Data.Repositories
{
    /// <summary>
    /// Repositorio transaccional para el manejo de Ventas.
    /// Realiza los inserts y actualización de stock dentro de una transacción.
    /// </summary>
    public class VentaRepository
    {
        private readonly string _connectionString = "Data Source=POS.db";

        /// <summary>
        /// Registra una nueva venta de manera transaccional y descontando el stock correspondiente.
        /// </summary>
        public void RegistrarVenta(Venta venta, IEnumerable<DetalleVenta> detalles)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var sqlMaxFolio = "SELECT MAX(folio) FROM Ventas";
                        var maxFolio = connection.QueryFirstOrDefault<int?>(sqlMaxFolio, null, transaction) ?? 0;
                        venta.Folio = maxFolio + 1;

                        var sqlVenta = @"
                            INSERT INTO Ventas (id_venta, folio, fecha_hora, total, metodo_pago, id_usuario) 
                            VALUES (@IdVenta, @Folio, @FechaHora, @Total, @MetodoPago, @IdUsuario)";
                        connection.Execute(sqlVenta, venta, transaction);

                        var sqlDetalle = @"
                            INSERT INTO Detalles_Venta (id_detalle, id_venta, id_producto, cantidad, precio_unitario, subtotal) 
                            VALUES (@IdDetalle, @IdVenta, @IdProducto, @Cantidad, @PrecioUnitario, @Subtotal)";
                        
                        var sqlUpdateStock = @"
                            UPDATE Productos 
                            SET stock = stock - @Cantidad 
                            WHERE id_producto = @IdProducto";

                        foreach (var detalle in detalles)
                        {
                            connection.Execute(sqlDetalle, detalle, transaction);
                            connection.Execute(sqlUpdateStock, new { Cantidad = detalle.Cantidad, IdProducto = detalle.IdProducto }, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene todas las ventas.
        /// </summary>
        public IEnumerable<Venta> ObtenerVentas()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sql = "SELECT id_venta AS IdVenta, folio, fecha_hora AS FechaHora, total, metodo_pago AS MetodoPago, id_usuario AS IdUsuario FROM Ventas";
                return connection.Query<Venta>(sql);
            }
        }

        /// <summary>
        /// Obtiene una venta por su Id.
        /// </summary>
        public Venta ObtenerVentaPorId(string idVenta)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sql = "SELECT id_venta AS IdVenta, folio, fecha_hora AS FechaHora, total, metodo_pago AS MetodoPago, id_usuario AS IdUsuario FROM Ventas WHERE id_venta = @IdVenta";
                return connection.QueryFirstOrDefault<Venta>(sql, new { IdVenta = idVenta });
            }
        }

        /// <summary>
        /// Obtiene los detalles de una venta específica mediante el Id de la venta.
        /// </summary>
        public IEnumerable<DetalleVenta> ObtenerDetallesPorVentaId(string idVenta)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sql = "SELECT id_detalle AS IdDetalle, id_venta AS IdVenta, id_producto AS IdProducto, cantidad, precio_unitario AS PrecioUnitario, subtotal FROM Detalles_Venta WHERE id_venta = @IdVenta";
                return connection.Query<DetalleVenta>(sql, new { IdVenta = idVenta });
            }
        }
    }
}

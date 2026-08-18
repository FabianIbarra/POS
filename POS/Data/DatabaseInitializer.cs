using Dapper;
using Microsoft.Data.Sqlite;
using POS.Data.Repositories;

namespace POS.Data
{
    /// <summary>
    /// Inicializa el esquema de la base de datos SQLite de forma idempotente.
    /// Garantiza que las tablas e índices existan antes de ejecutar la aplicación.
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Crea las tablas e índices con la cláusula IF NOT EXISTS, permitiendo
        /// reejecutar la inicialización sin alterar datos existentes.
        /// </summary>
        public static void Inicializar()
        {
            var sql = @"
CREATE TABLE IF NOT EXISTS Categorias (
  id_categoria TEXT PRIMARY KEY,
  nombre TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Productos (
  id_producto TEXT PRIMARY KEY,
  codigo_barras TEXT UNIQUE NOT NULL,
  descripcion TEXT NOT NULL,
  precio_compra NUMERIC NOT NULL,
  precio_venta NUMERIC NOT NULL,
  stock NUMERIC DEFAULT 0,
  disponible INTEGER DEFAULT 1,
  id_categoria TEXT,
  FOREIGN KEY (id_categoria) REFERENCES Categorias(id_categoria)
);

CREATE TABLE IF NOT EXISTS Usuarios (
  id_usuario TEXT PRIMARY KEY,
  username TEXT UNIQUE NOT NULL,
  password_hash TEXT NOT NULL,
  nombre_completo TEXT NOT NULL,
  rol TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Ventas (
  id_venta TEXT PRIMARY KEY,
  folio INTEGER UNIQUE NOT NULL,
  fecha_hora TEXT NOT NULL,
  total NUMERIC NOT NULL,
  metodo_pago TEXT NOT NULL,
  id_usuario TEXT,
  FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario)
);

CREATE TABLE IF NOT EXISTS Detalles_Venta (
  id_detalle TEXT PRIMARY KEY,
  id_venta TEXT NOT NULL,
  id_producto TEXT NOT NULL,
  cantidad NUMERIC NOT NULL,
  precio_unitario NUMERIC NOT NULL,
  subtotal NUMERIC NOT NULL,
  FOREIGN KEY (id_venta) REFERENCES Ventas(id_venta),
  FOREIGN KEY (id_producto) REFERENCES Productos(id_producto)
);

CREATE INDEX IF NOT EXISTS idx_ventas_fecha ON Ventas(fecha_hora);
CREATE INDEX IF NOT EXISTS idx_ventas_folio ON Ventas(folio);
CREATE INDEX IF NOT EXISTS idx_detalles_venta ON Detalles_Venta(id_venta);";

            using (var connection = new SqliteConnection(BaseRepository.ConnectionString))
            {
                connection.Execute(sql);
            }
        }
    }
}
-- 1. Inventario 
CREATE TABLE Categorias ( 
  id_categoria TEXT PRIMARY KEY, -- Aquí se guarda el GUID desde C# 
  nombre TEXT NOT NULL 
);

CREATE TABLE Productos ( 
  id_producto TEXT PRIMARY KEY, -- GUID 
  codigo_barras TEXT UNIQUE NOT NULL, 
  descripcion TEXT NOT NULL, 
  precio_compra NUMERIC NOT NULL, -- NUMERIC es el equivalente a DECIMAL en SQLite 
  precio_venta NUMERIC NOT NULL, 
  stock NUMERIC DEFAULT 0, 
  disponible INTEGER DEFAULT 1, -- En SQLite el Boolean es 1 (True) o 0 (False) 
  id_categoria TEXT, 
  FOREIGN KEY (id_categoria) REFERENCES Categorias(id_categoria) 
);

-- 2. Usuarios 
CREATE TABLE Usuarios ( 
  id_usuario TEXT PRIMARY KEY, -- GUID 
  username TEXT UNIQUE NOT NULL, 
  password_hash TEXT NOT NULL, 
  nombre_completo TEXT NOT NULL, 
  rol TEXT NOT NULL 
);

-- 3. Transacciones 
CREATE TABLE Ventas ( 
  id_venta TEXT PRIMARY KEY, -- GUID (Uso interno de la base de datos) 
  folio INTEGER UNIQUE NOT NULL, -- El número de ticket real visible para el cliente (Ej. 1, 2, 3...) 
  fecha_hora TEXT NOT NULL, -- En SQLite, las fechas se guardan como texto ISO8601 (Ej. '2026-07-15 14:30:00') 
  total NUMERIC NOT NULL, 
  metodo_pago TEXT NOT NULL, 
  id_usuario TEXT, 
  FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario) 
);

CREATE TABLE Detalles_Venta ( 
  id_detalle TEXT PRIMARY KEY, -- GUID 
  id_venta TEXT NOT NULL, 
  id_producto TEXT NOT NULL, 
  cantidad NUMERIC NOT NULL, 
  precio_unitario NUMERIC NOT NULL, 
  subtotal NUMERIC NOT NULL, 
  FOREIGN KEY (id_venta) REFERENCES Ventas(id_venta), 
  FOREIGN KEY (id_producto) REFERENCES Productos(id_producto) 
);

-- 4. Índices de optimización 
CREATE INDEX idx_ventas_fecha ON Ventas(fecha_hora); 
CREATE INDEX idx_ventas_folio ON Ventas(folio); 
CREATE INDEX idx_detalles_venta ON Detalles_Venta(id_venta);
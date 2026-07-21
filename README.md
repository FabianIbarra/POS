# Sistema POS (Punto de Venta)

Sistema de Punto de Venta (POS) de escritorio para tiendas minoristas. Este sistema está diseñado para agilizar el proceso de cobro mediante una interfaz rápida, administrar el inventario, gestionar usuarios y generar reportes básicos de ventas. El sistema mantiene siempre un registro horario preciso basado en la zona horaria geolocalizada de Mazatlán/Sinaloa (MST - Mountain Standard Time).

## 📊 Estado del Proyecto

Actualmente, el proyecto se encuentra a la **mitad de la Fase 5** del plan de implementación.

### ✅ Implementaciones Completadas
*   **Arquitectura Base y Capa de Datos:** Estructura de carpetas establecida, modelos de dominio anémicos creados, repositorios base con Dapper implementados (con soporte para "Soft Delete") y repositorio transaccional configurado.
*   **Servicios Base y Autenticación:** `TimeService` implementado para forzar la hora MST, `AuthService` (junto con su interfaz `IAuthService`) con validación de contraseñas mediante BCrypt, y `LoginViewModel` configurado.
*   **Enrutamiento y Navegación Principal:** Contenedor principal desarrollado con control de accesos por rol (Cajero vs Administrador) y menú lateral dinámico mediante *DataBinding*.
*   **Módulo de Inventario y Catálogos:** CRUD de productos (`InventarioViewModel` e `InventarioView.xaml` creados) implementado con validación de margen de ganancias (`precio_venta` > `precio_compra`) y buscador interactivo por código de barras o nombre.

### 🔄 Implementaciones en Progreso
*   **Módulo de Punto de Venta (Caja) - [En curso - 50%]**
    *   ✅ *Completado:* Vistas base y ViewModels creados (`POSViewModel.cs`, `POSView.xaml`). Carrito de compras (`ObservableCollection`), cálculo en tiempo real de Subtotal (sin IVA), IVA y Total, edición dinámica de cantidades y eliminación de renglones.
    *   ✅ *Completado:* UX/UI base del DataGrid (solo columna "Cantidad" editable) y mantenimiento del foco en el buscador.
    *   ⏳ *Pendiente:* Implementación de atajos de teclado extendidos (`F1`, `F5`, `+`, `-`, `Enter`).
    *   ⏳ *Pendiente:* Modal de cobro, cálculo de cambio, manejo de stock en negativo y procesamiento de la transacción final.

### ⏳ Implementaciones Pendientes
*   **Reportes y Cierre de Caja:** Consultas de datos históricos por fechas/folio, y vista de reportes (`ReportesView.xaml`) con ingresos totales del día.

---

## 🏗️ Arquitectura y Stack Tecnológico

El proyecto es un monolito modular que sigue una arquitectura **MVVM (Model-View-ViewModel) Estricta**.
*   **Regla de Oro:** Existe cero lógica de negocio o acceso a datos en el *Code-Behind* (`.xaml.cs`).
*   **Lenguaje y UI:** C# (.NET 8) y Windows Presentation Foundation (WPF) usando DataBindings.
*   **Lógica de Presentación:** `CommunityToolkit.Mvvm`.
*   **Base de Datos Local:** SQLite (archivo `pos.db`).
*   **Micro-ORM:** Dapper para consultas a base de datos eficientes mediante SQL directo, prescindiendo de Entity Framework.
*   **Seguridad:** Encriptación de contraseñas usando `BCrypt.Net-Next`.

---

## 🗄️ Esquema de Base de Datos y Reglas de Negocio

*   **Tipos de Datos:** Las llaves primarias (`Id`) se almacenan como `string` (GUIDs).
*   **Valores Monetarios:** Se gestionan como tipo `decimal` en C# y se almacenan como `NUMERIC` en SQLite.
*   **Fechas y Zonas Horarias:** Todas las fechas se guardan como `string` en formato ISO8601. Las transacciones fuerzan su registro en la zona horaria "America/Mazatlan" (MST).
*   **Folios:** Se genera un `folio` secuencial (entero) independiente del GUID para visibilidad del cliente.
*   **Soft Delete:** No se utiliza la instrucción `DELETE` en productos; se actualiza el campo `disponible` a `0` para mantener el historial.

---

## 📁 Estructura del Proyecto

A continuación, se detalla la estructura actual del proyecto principal (`POS`):

```plaintext
POS/
 ├── Data/
 │   └── Repositories/       # Capa de persistencia (Dapper + SQLite)
 │       ├── CategoriaRepository.cs
 │       ├── ProductoRepository.cs
 │       ├── UsuarioRepository.cs
 │       └── VentaRepository.cs
 ├── Helpers/                # Convertidores XAML y utilidades
 ├── Models/                 # Entidades anémicas (BD)
 │       ├── Categoria.cs
 │       ├── DetalleVenta.cs
 │       ├── Producto.cs
 │       ├── Usuario.cs
 │       └── Venta.cs
 ├── Services/               # Lógica de negocio pura
 │       ├── AuthService.cs
 │       ├── IAuthService.cs
 │       └── TimeService.cs
 ├── ViewModels/             # Clases de estado de vista (CommunityToolkit.Mvvm)
 │       ├── InventarioViewModel.cs
 │       ├── LoginViewModel.cs
 │       └── POSViewModel.cs
 ├── Views/                  # Interfaces de usuario (XAML)
 │       ├── InventarioView.xaml
 │       ├── LoginView.xaml
 │       └── POSView.xaml
 ├── App.xaml                # Configuración global e inyección de dependencias
 └── POS.db                  # Archivo local de base de datos SQLite

```

---

## 🚀 Guía de Uso Rápido e Instalación

### Pre-requisitos

* Tener instalado **.NET 8 SDK**.

### Compilar y Ejecutar

1. Clona el repositorio.
2. Asegúrate de compilar y restaurar las dependencias del proyecto ejecutando:
```bash
dotnet build

```


3. Ejecuta el sistema:
```bash
dotnet run --project POS/POS.csproj

```



### Uso General

* **Inicio de Sesión:** El sistema bloqueará accesos (como reportes) dependiendo de si el rol es *Cajero* o *Administrador*.
* **Punto de Venta:** Optimizado para uso al 100% con teclado y lector de códigos.
* `Enter`: Agrega el producto al carrito.
* `F1`: Cobra la venta.
* `F5`: Vacía el carrito cancelando la venta.


* **Inventario:** Permite CRUD estricto de productos y valida los márgenes de precio.

> **Nota para Desarrolladores:** Siempre asegúrate de utilizar el servicio de inyección para autenticación (`IAuthService`) y de obtener la hora a través de `TimeService` (America/Mazatlan). No uses `DateTime.Now` directamente en los Repositorios de datos.

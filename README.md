# Sistema POS (Punto de Venta)

Sistema de Punto de Venta (POS) de escritorio para tiendas minoristas. Este sistema está diseñado para agilizar el proceso de cobro mediante una interfaz rápida, administrar el inventario, gestionar usuarios y generar reportes básicos de ventas. El sistema mantiene siempre un registro horario preciso basado en la zona horaria geolocalizada de Mazatlán/Sinaloa (MST - Mountain Standard Time).

### ✅ Características Principales

*   **Arquitectura Base y Capa de Datos:** Estructura de carpetas establecida, modelos de dominio creados, repositorios base con Dapper implementados (con soporte para "Soft Delete") y repositorio transaccional configurado.
*   **Servicios Base y Autenticación:** `TimeService` implementado para forzar la hora MST, `AuthService` (junto con su interfaz `IAuthService`) con validación de contraseñas mediante BCrypt, y `LoginViewModel` configurado.
*   **Enrutamiento y Navegación Principal:** Contenedor principal (`MainViewModel` + `MainView`) con control de accesos por rol (Cajero vs Administrador) y menú lateral dinámico mediante *DataBinding*.
*   **Módulo de Inventario y Catálogos:** CRUD de productos y categorías (`InventarioViewModel` e `InventarioControl`) implementado con validación de margen de ganancias (`precio_venta` > `precio_compra`) y buscador interactivo.
*   **Módulo de Punto de Venta:** Carrito de compras con cálculo en tiempo real de Subtotal (sin IVA), IVA y Total. Edición dinámica de cantidades, eliminación de renglones, modal de cobro con cálculo de cambio, y atajos de teclado extendidos.
*   **Reportes:** Consultas de datos históricos por rango de fechas y número de folio. Vista de reportes con detalle de ventas, panel lateral de productos vendidos y resumen de ingresos totales.

---

## 🏗️ Arquitectura y Stack Tecnológico

El proyecto es un monolito modular que sigue una arquitectura **MVVM (Model-View-ViewModel) Estricta**.

*   **Regla de Oro:** Existe cero lógica de negocio o acceso a datos en el *Code-Behind* (`.xaml.cs`).
*   **Lenguaje y UI:** C# (.NET 8) y Windows Presentation Foundation (WPF) usando DataBindings.
*   **Lógica de Presentación:** `CommunityToolkit.Mvvm`.
*   **Base de Datos Local:** SQLite (archivo `pos.db`).
*   **Micro-ORM:** Dapper para consultas a base de datos eficientes mediante SQL directo, prescindiendo de Entity Framework.
*   **Seguridad:** Encriptación de contraseñas usando `BCrypt.Net-Next`.
*   **Navegación:** Sistema de `DataTemplates` en `MainView` que asocia cada `ViewModel` con su `UserControl` correspondiente.
*   **Idioma:** Forzado a `es-MX` en `App.xaml.cs:OnStartup`. Zona horaria forzada a `America/Mazatlan` vía `TimeService`.

---

## 🗄️ Esquema de Base de Datos y Reglas de Negocio

*   **Tipos de Datos:** Las llaves primarias (`Id`) se almacenan como `string` (GUIDs).
*   **Valores Monetarios:** Se gestionan como tipo `decimal` en C# y se almacenan como `NUMERIC` en SQLite.
*   **Fechas y Zonas Horarias:** Todas las fechas se guardan como `string` en formato ISO8601. Las transacciones fuerzan su registro en la zona horaria "America/Mazatlan" (MST).
*   **Folios:** Se genera un `folio` secuencial (entero) independiente del GUID para visibilidad del cliente.
*   **Soft Delete:** No se utiliza la instrucción `DELETE` en productos; se actualiza el campo `disponible` a `0` para mantener el historial.

---

## 📁 Estructura del Proyecto

A continuación, se detalla la estructura del proyecto principal (`POS`):

```plaintext
POS/
 ├── Data/
 │   └── Repositories/       # Capa de persistencia (Dapper + SQLite)
 │       ├── CategoriaRepository.cs
 │       ├── ProductoRepository.cs
 │       ├── UsuarioRepository.cs
 │       └── VentaRepository.cs
 ├── Helpers/                # Convertidores XAML y utilidades
 │   └── NotBooleanConverter.cs
 ├── Models/                 # Entidades de dominio
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
 │       ├── CobroViewModel.cs
 │       ├── InventarioViewModel.cs
 │       ├── LoginViewModel.cs
 │       ├── MainViewModel.cs
 │       ├── POSViewModel.cs
 │       └── ReportesViewModel.cs
 ├── Views/                  # Interfaces de usuario (XAML)
 │       ├── CobroView.xaml / .cs
 │       ├── InventarioView.xaml / .cs
 │       ├── InventarioControl.xaml / .cs
 │       ├── LoginView.xaml / .cs
 │       ├── MainView.xaml / .cs
 │       ├── POSView.xaml / .cs
 │       ├── POSControl.xaml / .cs
 │       └── ReportesControl.xaml / .cs
 ├── App.xaml                # Configuración global y recursos
 ├── App.xaml.cs             # Forzado de cultura y arranque
 ├── TablasPOS.sql           # Esquema de base de datos
 └── POS.db                  # Archivo local de base de datos SQLite
```

---

## 🚀 Guía de Uso Rápido e Instalación

### Pre-requisitos

* Tener instalado **.NET 8 SDK**.

### Compilar y Ejecutar

1. Clona el repositorio.
2. Compila y restaura las dependencias del proyecto ejecutando:
```bash
dotnet build
```

3. Ejecuta el sistema:
```bash
dotnet run --project POS/POS.csproj
```

### Uso General

* **Inicio de Sesión:** Al abrir la aplicación, el foco se coloca automáticamente en el campo de usuario. Al presionar Enter se salta al campo de contraseña, y al presionar Enter de nuevo se ejecuta el inicio de sesión. El sistema restringe accesos según el rol:
    * **Cajero:** Solo puede acceder al módulo de Punto de Venta.
    * **Administrador:** Accede a Punto de Venta, Inventario y Reportes.

* **Punto de Venta:** Optimizado para uso con teclado y lector de códigos de barras.

| Atajo | Función |
|---|---|
| `Enter` | Agrega el producto al carrito |
| `F1` | Abre el modal de cobro |
| `F5` | Vacía el carrito (cancela la venta) |
| `+` / `+` (num.) | Incrementa en 1 la cantidad del producto seleccionado |
| `-` / `-` (num.) | Decrementa en 1 la cantidad del producto seleccionado |
| `Suprimir` | Elimina por completo el renglón seleccionado |
| `Flechas ↑↓` | Navega entre los productos del carrito |

* **Modal de Cobro:** Al presionar F1 se abre una ventana modal que muestra el total de la venta. El cajero ingresa el efectivo recibido y el sistema calcula el cambio de manera instantánea. Al confirmar, la venta se guarda de forma transaccional y el carrito se limpia.

* **Inventario:** CRUD de productos y categorías. Validación estricta de que `precio_venta` sea mayor a `precio_compra`. Los productos se eliminan de manera lógica (soft delete).

* **Reportes:** Consultas por rango de fechas o número de folio. Muestra un listado de ventas con su detalle, e incluye un panel de resumen con el total de ingresos del periodo seleccionado.

---

## 🛠️ Comandos Disponibles

```powershell
# Compilar el proyecto
dotnet build

# Ejecutar el proyecto
dotnet run --project POS/POS.csproj
```

> **Nota para Desarrolladores:** Siempre asegúrate de utilizar `AuthService` para autenticación y de obtener la hora a través de `TimeService` (America/Mazatlan). No uses `DateTime.Now` directamente en los Repositorios de datos.

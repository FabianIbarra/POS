# Sistema POS (Punto de Venta)

Sistema de Punto de Venta (POS) de escritorio para tiendas minoristas, desarrollado con C# (WPF, .NET 8) y SQLite.
Este sistema está diseñado para agilizar el proceso de cobro, administrar el inventario, gestionar usuarios y generar reportes básicos de ventas, manteniendo siempre un registro horario preciso basado en la zona horaria geolocalizada (MST - Mazatlán/Sinaloa).

## Estado del Proyecto

### Fases Completadas
- ✅ **Fase 1: Arquitectura Base y Capa de Datos** - Completada
  - Estructura de carpetas establecida
  - Modelos anémicos creados (Categoría, Producto, Usuario, Venta, DetalleVenta)
  - Repositorios base implementados con Dapper
  - Repositorio transaccional de ventas con soporte para rollback
  
- ✅ **Fase 2: Servicios Base y Autenticación** - Completada
  - TimeService implementado para zona MST
  - AuthService con validación BCrypt
  - LoginViewModel con CommunityToolkit.Mvvm
  - LoginView.xaml con UX responsiva
  - Pruebas unitarias validadas

### Fases Pendientes
- 🔄 **Fase 3: Módulo de Inventario y Catálogos**
- 🔄 **Fase 4: Módulo de Punto de Venta (Caja)**
- 🔄 **Fase 5: Reportes, Historial y Cierre de Caja**

## Arquitectura

El proyecto sigue una arquitectura **MVVM (Model-View-ViewModel) Estricta**, lo que significa que el _code-behind_ de las vistas (`.xaml.cs`) está libre de lógica de negocio y acceso a datos.

*   **UI:** Windows Presentation Foundation (WPF) usando DataBindings.
*   **Lógica de Presentación:** Librería `CommunityToolkit.Mvvm`.
*   **Base de Datos Local:** SQLite (archivo `pos.db`).
*   **Micro-ORM:** Dapper para consultas a base de datos eficientes mediante SQL crudo.
*   **Seguridad:** Encriptación de contraseñas usando `BCrypt.Net-Next`.

## Estructura del Proyecto

```plaintext
POS/
 ├── Models/           # Entidades que mapean la base de datos (Producto, Venta, Usuario)
 ├── Data/
 │   └── Repositories/ # Capa de persistencia usando Dapper y Sqlite
 ├── Services/         # Lógica de negocio (AuthService, TimeService, etc.)
 ├── ViewModels/       # Clases de estado de la vista y comandos
 ├── Views/            # Interfaces de usuario en XAML
 ├── Helpers/          # Convertidores o herramientas utilitarias 
 ├── App.xaml          # Punto de entrada de la aplicación
 ├── pos.db            # Archivo de base de datos
 └── PRUEBAS_TAREA_2_3.md # Documentación de pruebas
```

## Guía de Uso Rápido e Instalación

### Pre-requisitos
*   Tener instalado **.NET 8 SDK**.

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

### Uso General (Una vez implementadas las vistas)
1. **Inicio de Sesión:** Ingresa tus credenciales en la pantalla de *Login*. Dependiendo de tu rol asignado, el sistema bloqueará ciertos accesos (ej. reportes bloqueados para Cajero).
2. **Caja / Punto de Venta:** Pantalla responsiva orientada al uso del teclado o escáner de códigos de barras (Ej. `F1` para cobrar, `F5` para limpiar carrito).
3. **Inventario:** Permite el CRUD de los productos y sus respectivas categorías, y valida el margen de ganancias de manera estricta.

> **Nota para Desarrolladores:** Siempre asegúrate de utilizar el servicio de inyección para autenticación (`AuthService`) y de obtener la hora a través de `TimeService` (America/Mazatlan). No uses `DateTime.Now` directamente en los Repositorios de datos.

## Documentación de Pruebas

Para ver el detalle de las pruebas unitarias de la **Tarea 2.3** (LoginViewModel y LoginView), consulta el archivo [`PRUEBAS_TAREA_2_3.md`](POS/PRUEBAS_TAREA_2_3.md).

**Resumen de Pruebas:** 7/7 pruebas aprobadas ✅

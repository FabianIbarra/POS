# Guide for Agents y Collaborators

### Language & Communication
* ALL responses, explanations, and documentation MUST be strictly in Latin American Spanish.
* Act as an expert Technical Writer. Document this in Mexican Spanish, ensuring impeccable spelling and grammar.
* Professional, direct tone. Avoid Spain-specific slang ("ustedes" not "vosotros", "archivos" not "ficheros", "computadora" not "ordenador").
* STRICTLY no emojis in code, docs, comments, or terminal output.

### Naming & Code Conventions
* Classes, properties, and methods in Spanish (e.g., `VentaService`, `CalcularTotal`), except standard English framework suffixes (`ViewModel`, `View`, `Repository`).
* Repository methods: descriptive name combining action + entity in Spanish (e.g., `AgregarCategoria`, `ObtenerCategorias`, `EditarCategoria`, `EliminarCategoria`).
* XML doc comments (`/// <summary>`) only for main classes, interfaces, and complex public methods. Write in Spanish. No redundant/obvious comments.
* Inline comments only for complex logic (SQL transactions, algorithms).

### Architecture (WPF .NET 8, MVVM Strict)
* Single project at `POS/POS.csproj`. Startup: `Views/LoginView.xaml`.
* **Zero business logic or data access in `.xaml.cs` code-behind.**
* Data access: Dapper + SQLite (`POS.db` runtime). **DO NOT use Entity Framework.**
* No DI container -- Services/Repositories are instantiated directly (`new()` in ViewModels).
* `MessageBox` is used directly in ViewModels (known anti-pattern, preserve existing pattern).
* Culture forced to `es-MX` in `App.xaml.cs:OnStartup`. Timezone forced to `America/Mazatlan` via static `TimeService`.
* IDs: `string` (GUIDs). Monetary values: `decimal`. Product soft-delete via `disponible` flag.

### Commands
```powershell
dotnet build
dotnet run --project POS/POS.csproj
```

### Repository Completeness
When creating/updating a Repository, ALWAYS implement full CRUD (Agregar, Obtener, ObtenerPorId, Editar, Eliminar) unless explicitly told otherwise.

### Layers (in dependency order)
`Models` -> `Data/Repositories` (Dapper/SQLite) -> `Services` -> `ViewModels` (CommunityToolkit.Mvvm) -> `Views` (XAML)

# Language & Communication
* ALL your responses, explanations, and generated documentation MUST be strictly in Latin American Spanish.
* Use a professional, clear, and direct tone. Avoid Spain-specific slang (e.g., use "ustedes" instead of "vosotros", "archivos" instead of "ficheros", "computadora" instead of "ordenador").

# Documentation & Comments Rules
* Use XML documentation tags (`/// <summary>`) for main classes, interfaces, and complex public methods. Write these summaries in Spanish.
* Comments MUST explain the *why* and the *purpose* in the system. Do NOT generate redundant or obvious documentation (e.g., do not write `// Obtiene el total` above a `GetTotal()` method).
* STRICTLY AVOID the use of emojis in all code documentation, comments, summaries, and terminal outputs.
* Keep code clean. Use inline comments (`//`) only to explain complex logic blocks (like SQL transactions or algorithms).

# Project Maintenance (README)
* Always keep the `README.md` updated, clear, and consistent with the current state of the project. Whenever a new feature, architecture layer, or setup requirement is introduced, proactively suggest the exact markdown to update the `README.md`.

# Code Generation & Naming Conventions
* Classes, properties, and methods must be named in Spanish (e.g., `VentaService`, `CalcularTotal`), except when dealing with standard framework conventions where English suffixes are required (e.g., `ViewModel`, `View`, `Repository`).
* **Repository Completeness:** When instructed to create or update a Repository, ALWAYS implement the full CRUD operations by default (Create, Read, Update, Delete) unless explicitly told otherwise. Do not take shortcuts.
* **Descriptive Method Names:** Repository methods MUST use descriptive names combining the action and the exact entity name in Spanish (e.g., `AgregarCategoria`, `ObtenerCategorias`, `ObtenerCategoriaPorId`, `EditarCategoria`, `EliminarCategoria`).

# Architectural Constraints (POS System)
* This is a WPF project using a STRICT MVVM pattern. There must be ZERO business logic or data access in the Code-Behind (`.xaml.cs`).
* The data access layer uses Dapper and SQLite (DO NOT use Entity Framework).
* Strict data types: Always use `decimal` for monetary variables/calculations and `string` for database IDs (GUIDs).

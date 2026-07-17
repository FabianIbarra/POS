# Language & Communication
* ALL your responses, explanations, and generated documentation MUST be strictly in Latin American Spanish.
* Use a professional, clear, and direct tone. Avoid Spain-specific slang (e.g., use "ustedes" instead of "vosotros", "archivos" instead of "ficheros", "computadora" instead of "ordenador").

# C# Documentation Rules
* Use XML documentation tags (`/// <summary>`) for all generated classes, interfaces, and public methods. Write these summaries in Spanish.
* Comments MUST explain the *why* and the *purpose* in the system, not just translate what the code does. 
* Keep code clean. Use inline comments (`//`) only to explain complex logic blocks (like SQL transactions or calculations).

# Naming Conventions
* Classes, properties, and methods must be named in Spanish (e.g., `VentaService`, `CalcularTotal`), except when dealing with standard framework conventions where English suffixes are required (e.g., `ViewModel`, `View`, `Repository`).

# Architectural Constraints (POS System)
* This is a WPF project using a STRICT MVVM pattern. There must be ZERO business logic or data access in the Code-Behind (`.xaml.cs`).
* The data access layer uses Dapper and SQLite (DO NOT use Entity Framework).
* Strict data types: Always use `decimal` for monetary variables/calculations and `string` for database IDs (GUIDs).
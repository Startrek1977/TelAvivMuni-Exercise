# TelAvivMuni-Exercise

A WPF application demonstrating a reusable data browser control for selecting items from collections with search and filtering capabilities.

## Project Overview

This project is a home exercise created as part of an interview for the Software Developer position at Tel-Aviv Municipality. It showcases a custom WPF control (`DataBrowserBox`) that provides a modern, user-friendly interface for browsing and selecting items from data collections.

## Key Features

- **DataBrowserBox** — Reusable WPF custom control with single/multi-select, custom columns, two-way MVVM binding
- **Browse Dialog** — Real-time search/filter, keyboard navigation, selection persistence, item counter
- **Pluggable Storage** — Switch between File (JSON/XML/CSV) and Database (SQL Server/SQLite/PostgreSQL/MySQL) via `appsettings.json`
- **Themes** — Blue, Emerald, GruvboxDark, AyuDark theme assemblies
- **MVVM** — Pure MVVM with attached behaviors, no UI logic in code-behind
- **176 unit tests** covering all business logic

## Documentation

| Document | Description |
|----------|-------------|
| [ARCHITECTURE.md](ARCHITECTURE.md) | Solution structure, project dependency graph, design patterns, and technical decisions |
| [SETUP_GUIDE.md](SETUP_GUIDE.md) | Prerequisites, build/run instructions, configuration, and database setup |
| [README_USER_SHORT.md](README_USER_SHORT.md) | End-user guide: features, DataBrowserBox usage, keyboard shortcuts |

## Quick Start

```bash
# Build
dotnet build TelAvivMuni-Exercise.sln

# Run
dotnet run --project TelAvivMuni-Exercise/TelAvivMuni-Exercise.csproj

# Test
dotnet test TelAvivMuni-Exercise.Tests/TelAvivMuni-Exercise.Tests.csproj
```

See [SETUP_GUIDE.md](SETUP_GUIDE.md) for full setup instructions including database configuration.

## Technology Stack

- **Framework:** .NET 8.0 (WPF)
- **Language:** C# 12
- **UI Pattern:** MVVM with CommunityToolkit.Mvvm 8.4.0
- **ORM:** Entity Framework Core 8.0 (SQL Server, SQLite, PostgreSQL, MySQL)
- **DI:** Microsoft.Extensions.Hosting 10.0.2
- **Testing:** xUnit 2.7.0, Moq 4.20.70, coverlet

## Solution Structure

The solution has **23 projects** organized in layers:

```
TelAvivMuni-Exercise.sln
│
├── [Persistence Layer]
│   ├── TelAvivMuni-Exercise.Persistence/           # Core abstractions (IDataStore, IEntity, ISerializer)
│   ├── TelAvivMuni-Exercise.Persistence.FileBase/  # File-based store (FileDataStore)
│   ├── TelAvivMuni-Exercise.Persistence.FileBase.Json/   # JSON provider
│   ├── TelAvivMuni-Exercise.Persistence.FileBase.Xml/    # XML provider
│   ├── TelAvivMuni-Exercise.Persistence.FileBase.Csv/    # CSV provider
│   ├── TelAvivMuni-Exercise.Persistence.Database/        # DB store (DbDataStore, EF Core)
│   ├── TelAvivMuni-Exercise.Persistence.Database.SqlServer/  # SQL Server provider (plugin)
│   ├── TelAvivMuni-Exercise.Persistence.Database.Sqlite/     # SQLite provider (plugin)
│   ├── TelAvivMuni-Exercise.Persistence.Database.PostgreSQL/ # PostgreSQL provider (plugin)
│   └── TelAvivMuni-Exercise.Persistence.Database.MySql/      # MySQL provider (plugin)
│
├── [Domain / Business Logic]
│   ├── TelAvivMuni-Exercise.Infrastructure/        # IDeferredInitialization (view-first pattern)
│   ├── TelAvivMuni-Exercise.Domain/                # Domain models (Product, BrowserColumn, OperationResult)
│   ├── TelAvivMuni-Exercise.Core.Contracts/        # Shared interfaces (IRepository, IUnitOfWork, IDialogService)
│   └── TelAvivMuni-Exercise.Core/                  # Business logic (AppDbContext, ProductRepository, UnitOfWork)
│
├── [Presentation Layer]
│   ├── TelAvivMuni-Exercise.Controls/              # DataBrowserBox, DataBrowserDialog, Behaviors
│   └── TelAvivMuni-Exercise.Presentation/          # ViewModels, DialogService, ViewModelLocator, MainWindow
│
├── [Themes]
│   ├── TelAvivMuni-Exercise.Themes/                # Shared base resources (Shared.xaml)
│   ├── TelAvivMuni-Exercise.Themes.Blue/           # Blue theme
│   ├── TelAvivMuni-Exercise.Themes.Emerald/        # Emerald theme
│   ├── TelAvivMuni-Exercise.Themes.Zed.GruvboxDark/ # Gruvbox Dark theme
│   └── TelAvivMuni-Exercise.Themes.Zed.AyuDark/   # Ayu Dark theme
│
├── TelAvivMuni-Exercise/                           # Composition root (DI wiring, entry point)
└── TelAvivMuni-Exercise.Tests/                     # Unit tests (176 tests)
```

> Provider plugins (SqlServer, SQLite, PostgreSQL, MySQL, Json, XML, CSV) are discovered at runtime via assembly scanning — drop a new provider DLL into the bin directory and update `appsettings.json`; no recompilation needed.

## License

This project is created for educational and interview purposes.

## Author

Created as part of the Tel-Aviv Municipality interview process for Software Developer position.

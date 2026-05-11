# Architecture

## Overview

The solution follows a clean layered architecture with 23 projects. Dependencies flow strictly downward: Persistence → Domain → Core → Presentation → Composition Root. Provider-specific assemblies (database and file-format plugins) are discovered at runtime via assembly scanning, so the composition root has no compile-time type coupling to individual providers. In the default build, the composition root may still reference provider projects so their DLLs are deployed and available for runtime discovery.

## Layer Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│  Composition Root (TelAvivMuni-Exercise WPF)                         │
│  DI wiring, App.xaml, StorageRegistrationExtensions                 │
└──────────────────────────┬──────────────────────────────────────────┘
                           │
         ┌─────────────────┴──────────────────┐
         ▼                                    ▼
┌─────────────────┐                ┌───────────────────────┐
│  Presentation   │                │       Themes          │
│  Controls       │                │  Blue, Emerald,       │
│  Presentation   │                │  GruvboxDark, AyuDark │
└────────┬────────┘                └───────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────┐
│  Business Logic                                              │
│  Core / Core.Contracts / Domain / Infrastructure            │
└──────────────────────────┬──────────────────────────────────┘
                           │
         ┌─────────────────┴──────────────────┐
         ▼                                    ▼
┌─────────────────┐               ┌──────────────────────────────┐
│  Persistence    │               │  Provider Plugins            │
│  (abstractions) │               │  (runtime-discovered DLLs)   │
│  FileBase       │               │  SqlServer, Sqlite,          │
│  Database       │               │  PostgreSQL, MySql,          │
└─────────────────┘               │  Xml, Csv                    │
                                  └──────────────────────────────┘
```

## Project Dependency Graph

```
TelAvivMuni-Exercise.Persistence  (no project dependencies)
    └── Contains: IEntity, IDataStore<T>, ISerializer<T>, ILocatableDataStore<T>

TelAvivMuni-Exercise.Persistence.FileBase
    └── → TelAvivMuni-Exercise.Persistence
    └── Contains: FileDataStore<T>, IFileProviderRegistrar

TelAvivMuni-Exercise.Persistence.FileBase.Json
    └── → TelAvivMuni-Exercise.Persistence.FileBase
    └── Contains: JsonSerializer<T>, JsonFileProviderRegistrar

TelAvivMuni-Exercise.Persistence.FileBase.Xml
    └── → TelAvivMuni-Exercise.Persistence.FileBase
    └── Contains: XmlSerializer<T>, XmlFileProviderRegistrar

TelAvivMuni-Exercise.Persistence.FileBase.Csv
    └── → TelAvivMuni-Exercise.Persistence.FileBase
    └── Contains: CsvSerializer<T>, CsvFileProviderRegistrar

TelAvivMuni-Exercise.Persistence.Database
    └── → TelAvivMuni-Exercise.Persistence
    └── Contains: DbDataStore<TEntity, TContext>, IDbContextRegistrar, IDbProviderRegistrar, IAppDbContextFactory

TelAvivMuni-Exercise.Persistence.Database.SqlServer
    └── → TelAvivMuni-Exercise.Persistence.Database
    └── Contains: SqlServerProviderRegistrar  [runtime plugin]

TelAvivMuni-Exercise.Persistence.Database.Sqlite
    └── → TelAvivMuni-Exercise.Persistence.Database
    └── Contains: SqliteProviderRegistrar  [runtime plugin]

TelAvivMuni-Exercise.Persistence.Database.PostgreSQL
    └── → TelAvivMuni-Exercise.Persistence.Database
    └── Contains: PostgreSQLProviderRegistrar  [runtime plugin]

TelAvivMuni-Exercise.Persistence.Database.MySql
    └── → TelAvivMuni-Exercise.Persistence.Database
    └── Contains: MySqlProviderRegistrar  [runtime plugin]

TelAvivMuni-Exercise.Infrastructure  (no project dependencies)
    └── Contains: IDeferredInitialization

TelAvivMuni-Exercise.Domain
    └── → TelAvivMuni-Exercise.Persistence
    └── Contains: Product, BrowserColumn, OperationResult

TelAvivMuni-Exercise.Core.Contracts
    ├── → TelAvivMuni-Exercise.Domain
    └── → TelAvivMuni-Exercise.Persistence
    └── Contains: IRepository<T>, IUnitOfWork, IDialogService, IColumnConfiguration, IMultiSelectViewModel

TelAvivMuni-Exercise.Core
    ├── → TelAvivMuni-Exercise.Core.Contracts
    ├── → TelAvivMuni-Exercise.Persistence
    ├── → TelAvivMuni-Exercise.Persistence.Database
    └── → TelAvivMuni-Exercise.Domain
    └── Contains: AppDbContext, ProductRepository, UnitOfWork, AppDbContextRegistrar, CoreAssembly

TelAvivMuni-Exercise.Themes  (no project dependencies)
    └── Contains: Shared.xaml (neutral brush defaults, shared vector icons)

TelAvivMuni-Exercise.Themes.Blue
    └── → TelAvivMuni-Exercise.Themes
    └── Contains: Blue.xaml, Blue.Colors.xaml, Blue.Styles.xaml

TelAvivMuni-Exercise.Themes.Emerald
    └── → TelAvivMuni-Exercise.Themes
    └── Contains: Emerald.xaml, Emerald.Colors.xaml, Emerald.Styles.xaml

TelAvivMuni-Exercise.Themes.Zed.GruvboxDark
    ├── → TelAvivMuni-Exercise.Themes
    └── → TelAvivMuni-Exercise.Controls
    └── Contains: GruvboxDark.xaml, GruvboxDark.Colors.xaml, GruvboxDark.Styles.xaml

TelAvivMuni-Exercise.Themes.Zed.AyuDark
    ├── → TelAvivMuni-Exercise.Themes
    └── → TelAvivMuni-Exercise.Controls
    └── Contains: AyuDark.xaml, AyuDark.Colors.xaml, AyuDark.Styles.xaml

TelAvivMuni-Exercise.Controls
    ├── → TelAvivMuni-Exercise.Core.Contracts
    ├── → TelAvivMuni-Exercise.Persistence
    ├── → TelAvivMuni-Exercise.Domain
    └── → TelAvivMuni-Exercise.Themes
    └── Contains: DataBrowserBox, DataBrowserDialog, Behaviors

TelAvivMuni-Exercise.Presentation
    ├── → TelAvivMuni-Exercise.Core.Contracts
    ├── → TelAvivMuni-Exercise.Persistence
    ├── → TelAvivMuni-Exercise.Controls
    └── → TelAvivMuni-Exercise.Domain
    └── Contains: ViewModels, DialogService, ViewModelLocator, MainWindow

TelAvivMuni-Exercise (WPF — composition root)
    ├── → TelAvivMuni-Exercise.Presentation
    ├── → TelAvivMuni-Exercise.Controls
    ├── → TelAvivMuni-Exercise.Core
    ├── → TelAvivMuni-Exercise.Core.Contracts
    ├── → TelAvivMuni-Exercise.Infrastructure
    ├── → TelAvivMuni-Exercise.Domain
    ├── → TelAvivMuni-Exercise.Persistence
    ├── → TelAvivMuni-Exercise.Persistence.Database
    ├── → TelAvivMuni-Exercise.Persistence.FileBase
    ├── → TelAvivMuni-Exercise.Persistence.Database.SqlServer  [plugin]
    ├── → TelAvivMuni-Exercise.Persistence.Database.Sqlite     [plugin]
    ├── → TelAvivMuni-Exercise.Persistence.Database.PostgreSQL [plugin]
    ├── → TelAvivMuni-Exercise.Persistence.Database.MySql      [plugin]
    ├── → TelAvivMuni-Exercise.Persistence.FileBase.Json       [plugin]
    ├── → TelAvivMuni-Exercise.Persistence.FileBase.Xml        [plugin]
    ├── → TelAvivMuni-Exercise.Persistence.FileBase.Csv        [plugin]
    ├── → TelAvivMuni-Exercise.Themes.Blue
    ├── → TelAvivMuni-Exercise.Themes.Emerald
    ├── → TelAvivMuni-Exercise.Themes.Zed.GruvboxDark
    └── → TelAvivMuni-Exercise.Themes.Zed.AyuDark

TelAvivMuni-Exercise.Tests
    ├── → TelAvivMuni-Exercise (WPF)
    ├── → TelAvivMuni-Exercise.Presentation
    ├── → TelAvivMuni-Exercise.Controls
    ├── → TelAvivMuni-Exercise.Core
    ├── → TelAvivMuni-Exercise.Core.Contracts
    ├── → TelAvivMuni-Exercise.Persistence
    └── → TelAvivMuni-Exercise.Domain
```

## Persistence Layer

The persistence layer uses the **Strategy pattern** with runtime-discovered provider plugins.

### Layered abstraction

```
ProductRepository (Core)
    └── IDataStore<Product> (Persistence)
            ├── FileDataStore<Product> (Persistence.FileBase)
            │       └── ISerializer<Product>
            │               ├── JsonSerializer<Product>  (Persistence.FileBase.Json)
            │               ├── XmlSerializer<Product>   (Persistence.FileBase.Xml)
            │               └── CsvSerializer<Product>   (Persistence.FileBase.Csv)
            │
            └── DbDataStore<Product, AppDbContext>  (Persistence.Database)
                    └── IAppDbContextFactory<AppDbContext> (Core)
                            └── Provider registered by:
                                ├── SqlServerProviderRegistrar
                                ├── SqliteProviderRegistrar
                                ├── PostgreSQLProviderRegistrar
                                └── MySqlProviderRegistrar
```

### Provider plugin discovery

`StorageRegistrationExtensions` scans loaded assemblies at runtime for `IFileProviderRegistrar` and `IDbProviderRegistrar` implementations. The selected provider (from `appsettings.json`) is instantiated and calls its `Register(IServiceCollection)` method. This means:
- Adding a new provider requires dropping a DLL in the bin directory and updating `appsettings.json`.
- No recompilation of the composition root is needed.

## Repository and Unit of Work Patterns

```csharp
public interface IRepository<TEntity> where TEntity : class
{
    IEnumerable<TEntity> Entities { get; }
    Task<TEntity[]> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
    Task<OperationResult> AddAsync(TEntity entity);
    Task<OperationResult> UpdateAsync(TEntity entity);
    Task<OperationResult> DeleteAsync(TEntity entity);
}

public interface IUnitOfWork : IDisposable
{
    IRepository<Product> Products { get; }
    Task<int> SaveChangesAsync();
}
```

`OperationResult` provides a consistent return type for CRUD operations:
```csharp
var result = await repository.AddAsync(product);
if (!result.Success)
    Console.WriteLine(result.ErrorMessage); // "A product with Id 1 already exists."
```

## MVVM and Attached Behaviors

The application is a **pure MVVM** implementation — no UI logic in code-behind. Keyboard handling and DataGrid interactions are implemented as reusable attached behaviors:

| Behavior | Description |
|----------|-------------|
| `AutoFocusSearchBehavior` | Focuses the search box when the user starts typing anywhere in the dialog |
| `DataGridEnterBehavior` | Fires a command when Enter is pressed in the DataGrid |
| `DataGridScrollIntoViewBehavior` | Scrolls the DataGrid to show the selected item |
| `DataGridMultiSelectBehavior` | Synchronizes DataGrid multi-selection with the ViewModel |
| `DialogCloseBehavior` | Closes the window when `ViewModel.DialogResult` changes |
| `EscapeClearBehavior` | Clears a TextBox when Escape is pressed |

## View-First Initialization (Deferred Loading)

The browse dialog uses a **View-First** pattern to display a loading overlay while data loads asynchronously:

1. The window is shown immediately with a semi-transparent "Loading..." overlay.
2. `ContentRendered` triggers `Initialize()` via XAML `CallMethodAction` (no code-behind).
3. The ViewModel implements `IDeferredInitialization` and stores pending items/selection until called.
4. Once initialized, the overlay is hidden and the DataGrid is populated.

`IColumnConfiguration` is a separate interface consumed by `DataBrowserDialog` to receive custom column definitions without a circular project reference.

## Custom Control vs UserControl

`DataBrowserBox` is a **Custom Control** (not a UserControl) for the following reasons:
- Template-based: consumers can completely replace the visual tree via `ControlTemplate`.
- Dependency properties enable WPF-style binding, animations, and style triggers.
- Follows WPF lookup conventions (`Generic.xaml` default template).

## Theme Architecture

Each theme is a standalone WPF class library with a three-file structure:

```
Theme.xaml          ← Entry point; merges Shared.xaml + Theme.Styles.xaml
Theme.Colors.xaml   ← Brush definitions (color palette)
Theme.Styles.xaml   ← Control styles (Button, DataGrid, DataBrowserBox override)
```

`Shared.xaml` (base `Themes` project) defines neutral fallback brushes and shared vector icons used by all themes.

All resource lookups use `StaticResource` (not `DynamicResource`) for faster XAML parsing and no runtime re-resolution overhead.

Dark themes (`GruvboxDark`, `AyuDark`) include an implicit `Style TargetType="DataBrowserBox"` that overrides the default light-mode styles defined in `Generic.xaml`.

## Key Design Decisions

### No circular dependencies
`IMultiSelectViewModel` lives in `Core.Contracts` so `DataBrowserDialog` (Controls) can reference it without depending on `Presentation`.

### `CoreAssembly` helper
`StorageRegistrationExtensions` uses `CoreAssembly.Reference` (a static property returning `Assembly.GetExecutingAssembly()` from the Core project) rather than a hardcoded assembly name string, ensuring correct runtime resolution regardless of rename or deployment path.

### XmlnsDefinition mappings
- `http://telaviv-muni-exe/controls` → `Controls` and `Controls.Behaviors` namespaces
- `http://telaviv-muni-exe/presentation` → all `Presentation` namespaces

These allow XAML consumers to use short namespace aliases instead of verbose `clr-namespace:...;assembly=...` declarations.

### Error handling strategy
- Collections are always initialized to empty, never `null`.
- `OperationResult` carries descriptive error messages for user display.
- File-loading errors (missing file, invalid JSON, IO errors) are caught and surfaced with user-friendly messages.
- Dialog opening is guarded: a null/empty collection shows a message box instead of an empty dialog.

## Version History Summary

| Version | Change |
|---------|--------|
| v8.3 | Multi-select mode (`AllowMultipleSelection`) + 13 new tests (176 total) |
| v8.2 | Ayu Dark theme (Zed editor palette) |
| v8.1 | GruvboxDark theme + full dark UI theming |
| v8.0 | Theme assemblies extracted; `DynamicResource` → `StaticResource` |
| v7.3 | `XmlnsDefinition` for Controls assembly |
| v7.2 | `MainWindow` moved to Presentation; pure composition root |
| v7.1 | `CoreAssembly` helper; removed hardcoded assembly name |
| v7.0 | Project decomposition (Domain, Controls, Presentation extracted) |
| v6.0 | Core decoupled from EF Core; clean layering |
| v5.0 | EF Core database support; pluggable persistence |
| v4.0 | 163 unit tests + coverlet coverage |
| v3.0 | Repository and Unit of Work patterns |
| v2.0 | MVVM refactor; attached behaviors replacing code-behind |

# TelAviv Municipality Exercise

A WPF application demonstrating a reusable data browser control for selecting items from collections with search and filtering capabilities.

## Project Overview

This project is a home exercise created as part of an interview for the Software Developer position at Tel-Aviv Municipality. It showcases a custom WPF control (`DataBrowserBox`) that provides a modern, user-friendly interface for browsing and selecting items from data collections.

## Features

### Custom DataBrowserBox Control
- **Reusable WPF Custom Control** - Can be used anywhere in the application with any data type
- **Display Member Path** - Configure which property to display in the control
- **Two-Way Data Binding** - Full MVVM support: `SelectedItem` (single) / `SelectedItems` (multi) bindings
- **Multi-Select Mode** - `AllowMultipleSelection` enables multi-item selection with a ×-clear button
- **Custom Column Configuration** - Define columns with custom headers, widths, formats, and alignment
- **Auto-Generated Columns** - Automatically generates columns from data type (default behavior)
- **Visual Feedback** - Watermark text in italic, selected items in bold
- **Modern UI** - Styled browse button with hover effects
- **Selection Persistence** - Currently selected item is highlighted when dialog reopens

### Browse Dialog
- **Modern Design** - Clean, professional interface with Material Design-inspired colors
- **MVVM Architecture** - Pure MVVM implementation using WPF Attached Behaviors
- **Vector Search Icon** - Clean magnifying glass icon using SVG path for crisp rendering at any size
- **Search & Filter** - Real-time search across all item properties
- **Clear Filter Button** - Appears when search text is entered, clears with one click
- **Flexible Column Display** - Auto-generated or custom column definitions
- **Column Formatting** - Support for currency, date, and custom string formats
- **Column Alignment** - Configure horizontal alignment (Left, Right, Center)
- **Selection Highlighting** - Bold text and blue background for selected items
- **Selection Persistence** - Previously selected item is automatically highlighted and scrolled into view
- **Row Hover Effects** - Visual feedback on mouse over
- **Item Counter** - Displays total filtered items count
- **Responsive Design** - Resizable dialog with proper scrolling
- **Keyboard Navigation** - Type to search, Enter to select, Escape to clear/cancel

### Error Handling
- **File Not Found** - Gracefully handles missing Products.json file
- **Empty File** - Handles empty or whitespace-only JSON files
- **Invalid JSON** - Catches and handles JSON deserialization errors
- **IO Errors** - Handles file access and permission issues
- **Null Collections** - Validates data before opening dialogs with user-friendly messages

## Technology Stack

- **Framework:** .NET 8.0 (WPF)
- **Language:** C# 12
- **UI Pattern:** MVVM (Model-View-ViewModel)
- **Libraries:**
  - CommunityToolkit.Mvvm 8.4.0 - For MVVM infrastructure (Core, Presentation)
  - Microsoft.EntityFrameworkCore 8.0.12 - ORM for database access (Infrastructure, Core)
  - Microsoft.EntityFrameworkCore.SqlServer 8.0.12 - SQL Server provider
  - Microsoft.EntityFrameworkCore.Sqlite 8.0.12 - SQLite provider
  - Npgsql.EntityFrameworkCore.PostgreSQL 8.0.11 - PostgreSQL provider
  - Pomelo.EntityFrameworkCore.MySql 8.0.2 - MySQL provider
  - Microsoft.Extensions.DependencyInjection 10.0.2 - IoC container (WPF)
  - Microsoft.Extensions.DependencyInjection.Abstractions 10.0.2 - DI abstractions (WPF, Presentation)
  - Microsoft.Extensions.Hosting 10.0.2 - Host builder for DI setup (WPF)
  - Microsoft.Xaml.Behaviors.Wpf 1.1.135 - Attached behaviors (Controls)
  - System.Text.Json - For JSON serialization
  - xUnit 2.7.0 - For unit testing
  - xunit.runner.visualstudio 2.5.7 - Test runner for Visual Studio
  - Microsoft.NET.Test.Sdk 17.9.0 - Test platform
  - Microsoft.EntityFrameworkCore.InMemory 8.0.12 - For database testing
  - Moq 4.20.70 - For mocking in tests
  - coverlet.collector 6.0.1 - For code coverage

## Solution Structure

The solution is organized into 23 projects to separate concerns and promote reusability:

### TelAvivMuni-Exercise.Infrastructure
Low-level data persistence abstractions and implementations:
- **Data:**
  - `IDataStore<T>` - Data persistence abstraction
  - `FileDataStore<T>` - File-based data store with thread-safe async operations
  - `DbDataStore<TEntity, TContext>` - SQL Server database data store using Entity Framework Core
- **Models:**
  - `IEntity` - Base entity interface with Id property
- **Patterns:**
  - `IDeferredInitialization` - View-First initialization interface
- **Serializers:**
  - `ISerializer<T>` - Serialization abstraction
  - `JsonSerializer<T>` - JSON serialization using System.Text.Json

### TelAvivMuni-Exercise.Domain
Domain models shared across all projects:
- **Models:**
  - `Product` - Product data model implementing IEntity
  - `BrowserColumn` - Column configuration for data browser
  - `OperationResult` - Operation result with success/failure states

### TelAvivMuni-Exercise.Core.Contracts
Shared contracts and interfaces:
- **Config:**
  - `IColumnConfiguration` - Column configuration interface
- **Patterns:**
  - `IRepository<T>` - Generic repository interface
  - `IUnitOfWork` - Unit of Work interface
- **Services:**
  - `IDialogService` - Dialog service interface
- **ViewModels:**
  - `IMultiSelectViewModel` - Multi-select state contract (`AllowMultipleSelection` + `SelectedItems`) consumed by `DataBrowserDialog` code-behind without creating a circular project reference

### TelAvivMuni-Exercise.Core
Application-specific business logic:
- **Data:**
  - `AppDbContext` - Entity Framework Core DbContext for SQL Server
- **Patterns:**
  - `ProductRepository` - Product-specific repository implementation
  - `UnitOfWork` - Unit of Work coordination pattern
- `AppDbContextRegistrar` - Registers `AppDbContext` into DI; discovered at runtime via assembly scanning
- `CoreAssembly` - Provides a reference to the Core assembly for runtime discovery without hardcoding its name

### TelAvivMuni-Exercise.Controls
Reusable WPF controls and attached behaviors:
- **Controls:**
  - `DataBrowserBox` - Custom reusable control for item selection
  - `DataBrowserDialog` - Browse dialog UI
- **Behaviors:**
  - `AutoFocusSearchBehavior` - Auto-focus on typing
  - `DataGridEnterBehavior` - Handle Enter key in DataGrid
  - `DataGridScrollIntoViewBehavior` - Scroll to selected item
  - `DialogCloseBehavior` - MVVM-friendly dialog closing
  - `EscapeClearBehavior` - Clear text on Escape key
- **Infrastructure:**
  - `AssemblyInfo.cs` - `[XmlnsDefinition]` mapping `http://telaviv-muni-exe/controls` to Controls and Controls.Behaviors namespaces

### TelAvivMuni-Exercise.Presentation
ViewModels, services, and presentation infrastructure:
- **Views:**
  - `MainWindow.xaml(.cs)` - Main application window
- **ViewModels:**
  - `MainWindowViewModel` - Main window view model
  - `DataBrowserDialogViewModel` - Dialog view model
- **Services:**
  - `DialogService` - Dialog service implementation (WPF-dependent)
- **Infrastructure:**
  - `ViewModelLocator` - DI-based ViewModel resolution for XAML
  - `ICommand.Extension` - Command extension methods (WPF/MVVM-dependent)
  - `AssemblyInfo.cs` - `[XmlnsDefinition]` mapping `http://telaviv-muni-exe/presentation` to all Presentation namespaces

### TelAvivMuni-Exercise.Themes
Base theme resources shared across all theme variants:
- **Themes:**
  - `Shared.xaml` - Neutral brush defaults, `LoadingOverlayBrush` fallback (used by `Generic.xaml`) and shared vector icons

### TelAvivMuni-Exercise.Themes.Blue
Blue theme variant:
- **Themes:**
  - `Blue.xaml` - Theme entry point (aggregator merging Shared + Blue.Styles)
  - `Blue.Colors.xaml` - Blue color palette (brush definitions)
  - `Blue.Styles.xaml` - Blue-specific control styles (buttons, DataGrid, ClearButton)

### TelAvivMuni-Exercise.Themes.Emerald
Emerald theme variant:
- **Themes:**
  - `Emerald.xaml` - Theme entry point (aggregator merging Shared + Emerald.Styles)
  - `Emerald.Colors.xaml` - Emerald color palette (brush definitions)
  - `Emerald.Styles.xaml` - Emerald-specific control styles (buttons, DataGrid, ClearButton)

### TelAvivMuni-Exercise.Themes.Zed.GruvboxDark
Gruvbox Dark theme variant:
- **Themes:**
  - `GruvboxDark.xaml` - Theme entry point (aggregator merging Shared + GruvboxDark.Styles)
  - `GruvboxDark.Colors.xaml` - Gruvbox Dark color palette (brush definitions)
  - `GruvboxDark.Styles.xaml` - GruvboxDark-specific control styles (buttons, DataGrid, DataBrowserBox override)

### TelAvivMuni-Exercise.Themes.Zed.AyuDark
Ayu Dark theme variant (Zed editor built-in palette):
- **Themes:**
  - `AyuDark.xaml` - Theme entry point (aggregator merging Shared + AyuDark.Styles)
  - `AyuDark.Colors.xaml` - Ayu Dark color palette mapped from Zed's `ayu.json` (brush definitions)
  - `AyuDark.Styles.xaml` - AyuDark-specific control styles (buttons, DataGrid, DataBrowserBox override)

### TelAvivMuni-Exercise (Main WPF Application)
Pure composition root — DI wiring and application entry point:
- **Data:** - Sample product data (Products.json, Product.xml, Product.csv), SQL scripts
- **Configuration:** - appsettings.json, appsettings.Development.json
- `App.xaml(.cs)` - Application entry point with DI registration

### TelAvivMuni-Exercise.Tests
Unit tests for all projects

### Project Dependencies
```
TelAvivMuni-Exercise.Infrastructure (no project dependencies)
    └── Contains: IEntity, IDataStore, FileDataStore, DbDataStore, Serializers

TelAvivMuni-Exercise.Domain
    └── → TelAvivMuni-Exercise.Infrastructure
    └── Contains: Product, BrowserColumn, OperationResult

TelAvivMuni-Exercise.Core.Contracts
    ├── → TelAvivMuni-Exercise.Domain
    └── → TelAvivMuni-Exercise.Infrastructure
    └── Contains: IRepository<T>, IUnitOfWork, IDialogService, IColumnConfiguration

TelAvivMuni-Exercise.Core
    ├── → TelAvivMuni-Exercise.Core.Contracts
    ├── → TelAvivMuni-Exercise.Infrastructure
    └── → TelAvivMuni-Exercise.Domain
    └── Contains: AppDbContext, ProductRepository, UnitOfWork

TelAvivMuni-Exercise.Themes (no project dependencies)
    └── Contains: Shared.xaml (neutral brush defaults, shared icons)

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
    ├── → TelAvivMuni-Exercise.Infrastructure
    ├── → TelAvivMuni-Exercise.Domain
    └── → TelAvivMuni-Exercise.Themes
    └── Contains: DataBrowserBox, DataBrowserDialog, Behaviors

TelAvivMuni-Exercise.Presentation
    ├── → TelAvivMuni-Exercise.Core.Contracts
    ├── → TelAvivMuni-Exercise.Infrastructure
    ├── → TelAvivMuni-Exercise.Controls
    └── → TelAvivMuni-Exercise.Domain
    └── Contains: ViewModels, DialogService, ViewModelLocator

TelAvivMuni-Exercise (WPF)
    ├── → TelAvivMuni-Exercise.Presentation
    ├── → TelAvivMuni-Exercise.Controls
    ├── → TelAvivMuni-Exercise.Core
    ├── → TelAvivMuni-Exercise.Core.Contracts
    ├── → TelAvivMuni-Exercise.Infrastructure
    ├── → TelAvivMuni-Exercise.Domain
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
    ├── → TelAvivMuni-Exercise.Infrastructure
    └── → TelAvivMuni-Exercise.Domain
```

## Project Structure

```
TelAvivMuni-Exercise.sln
│
├── TelAvivMuni-Exercise.Infrastructure/     # Data persistence layer (no project dependencies)
│   ├── Data/
│   │   ├── DbDataStore.cs                   # Database data store (EF Core)
│   │   ├── FileDataStore.cs                 # File-based data store
│   │   └── IDataStore.cs                    # Data persistence abstraction
│   ├── Models/
│   │   └── IEntity.cs                       # Base entity interface
│   ├── Patterns/
│   │   └── IDeferredInitialization.cs       # View-First initialization interface
│   └── Serializers/
│       ├── ISerializer.cs                   # Serialization abstraction
│       └── JsonSerializer.cs                # JSON serialization implementation
│
├── TelAvivMuni-Exercise.Domain/             # Domain models
│   └── Models/
│       ├── BrowserColumn.cs                 # Column configuration model
│       ├── OperationResult.cs               # Operation result with error messages
│       └── Product.cs                       # Product data model
│
├── TelAvivMuni-Exercise.Core.Contracts/     # Shared contracts and interfaces
│   ├── Config/
│   │   └── IColumnConfiguration.cs          # Column configuration interface
│   ├── Patterns/
│   │   ├── IRepositoryT.cs                  # Generic repository interface
│   │   └── IUnitOfWork.cs                   # Unit of Work interface
│   ├── Services/
│   │   └── IDialogService.cs                # Dialog service interface
│   └── ViewModels/
│       └── IMultiSelectViewModel.cs         # Multi-select state contract for dialog ViewModels
│
├── TelAvivMuni-Exercise.Core/               # Business logic
│   ├── Data/
│   │   └── AppDbContext.cs                  # EF Core DbContext for SQL Server
│   ├── Patterns/
│   │   ├── ProductRepository.cs             # Product-specific repository
│   │   └── UnitOfWork.cs                    # Unit of Work implementation
│   ├── AppDbContextRegistrar.cs             # Registers AppDbContext into DI (runtime-discovered)
│   └── CoreAssembly.cs                      # Assembly reference helper for runtime discovery
│
├── TelAvivMuni-Exercise.Controls/           # Reusable WPF controls & behaviors
│   ├── Behaviors/
│   │   ├── AutoFocusSearchBehavior.cs       # Auto-focus on typing
│   │   ├── DataGridEnterBehavior.cs         # Handle Enter key in DataGrid
│   │   ├── DataGridScrollIntoViewBehavior.cs # Scroll to selected item
│   │   ├── DialogCloseBehavior.cs           # MVVM-friendly dialog closing
│   │   └── EscapeClearBehavior.cs           # Clear text on Escape key
│   ├── Themes/
│   │   └── Generic.xaml                     # Control templates (merges Shared.xaml, all StaticResource)
│   ├── AssemblyInfo.cs                      # XmlnsDefinition for XAML namespace alias
│   ├── DataBrowserBox.cs                    # Custom reusable control
│   └── DataBrowserDialog.xaml(.cs)          # Browse dialog UI
│
├── TelAvivMuni-Exercise.Themes/             # Base theme resources (shared across variants)
│   ├── AssemblyInfo.cs
│   └── Themes/
│       └── Shared.xaml                      # Neutral brush defaults & shared vector icons
│
├── TelAvivMuni-Exercise.Themes.Blue/        # Blue theme variant
│   ├── AssemblyInfo.cs
│   └── Themes/
│       ├── Blue.xaml                        # Theme entry point (aggregator)
│       ├── Blue.Colors.xaml                 # Blue color palette (brush definitions)
│       └── Blue.Styles.xaml                 # Blue-specific control styles
│
├── TelAvivMuni-Exercise.Themes.Emerald/     # Emerald theme variant
│   ├── AssemblyInfo.cs
│   └── Themes/
│       ├── Emerald.xaml                     # Theme entry point (aggregator)
│       ├── Emerald.Colors.xaml              # Emerald color palette (brush definitions)
│       └── Emerald.Styles.xaml              # Emerald-specific control styles
│
├── TelAvivMuni-Exercise.Themes.Zed.GruvboxDark/  # Gruvbox Dark theme variant
│   └── Themes/
│       ├── GruvboxDark.xaml                 # Theme entry point (aggregator)
│       ├── GruvboxDark.Colors.xaml          # Gruvbox Dark color palette (brush definitions)
│       └── GruvboxDark.Styles.xaml          # GruvboxDark-specific control styles + DataBrowserBox override
│
├── TelAvivMuni-Exercise.Themes.Zed.AyuDark/      # Ayu Dark theme variant (Zed editor palette)
│   └── Themes/
│       ├── AyuDark.xaml                     # Theme entry point (aggregator)
│       ├── AyuDark.Colors.xaml              # Ayu Dark color palette from Zed ayu.json (brush definitions)
│       └── AyuDark.Styles.xaml              # AyuDark-specific control styles + DataBrowserBox override
│
├── TelAvivMuni-Exercise.Presentation/       # ViewModels, views & presentation services
│   ├── Services/
│   │   └── DialogService.cs                 # Dialog service implementation
│   ├── ViewModels/
│   │   ├── DataBrowserDialogViewModel.cs    # Dialog view model
│   │   └── MainWindowViewModel.cs           # Main window view model
│   ├── Views/
│   │   └── MainWindow.xaml(.cs)             # Main application window
│   ├── AssemblyInfo.cs                      # XmlnsDefinition for XAML namespace alias
│   ├── ICommand.Extension.cs                # ICommand extension methods
│   └── ViewModelLocator.cs                  # DI-based ViewModel resolution
│
├── TelAvivMuni-Exercise/                    # Composition root (entry point & DI wiring)
│   ├── Data/
│   │   ├── CreateProductsDatabase.sql       # SQL Server database setup script
│   │   ├── GenerateSqlScript.ps1            # PowerShell script to generate SQL
│   │   ├── Products.json                    # Sample product data (JSON)
│   │   ├── Product.xml                      # Sample product data (XML)
│   │   └── Product.csv                      # Sample product data (CSV)
│   ├── App.xaml(.cs)                        # Application entry point with DI
│   ├── AssemblyInfo.cs
│   ├── StorageOptions.cs                    # Pluggable storage configuration
│   ├── StorageRegistrationExtensions.cs     # Runtime provider discovery & DI registration
│   ├── appsettings.json                     # Default configuration
│   └── appsettings.Development.json         # Development configuration
│
├── TelAvivMuni-Exercise.Tests/              # Unit test project
│   ├── Core/
│   │   ├── AppDbContextTests.cs
│   │   └── CoreAssemblyTests.cs
│   ├── Domain/
│   │   ├── BrowserColumnTests.cs
│   │   ├── IEntityTests.cs
│   │   └── OperationResultTests.cs
│   ├── Infrastructure/
│   │   ├── DbDataStoreTests.cs
│   │   ├── FileDataStoreTests.cs
│   │   ├── JsonSerializerTests.cs
│   │   ├── ProductRepositoryTests.cs
│   │   └── UnitOfWorkTests.cs
│   └── Presentation/
│       ├── DataBrowserDialogViewModelTests.cs
│       └── MainWindowViewModelTests.cs
│
├── coverlet.runsettings                     # Code coverage configuration
└── Settings.XamlStyler                      # XAML Styler formatting rules (xstyler CLI config)
```

## Configuration

### Storage Provider

The application supports pluggable storage providers configured via `appsettings.json`:

```json
{
  "Storage": {
    "Kind": "File",
    "Provider": "Json"
  }
}
```

**Storage Options:**

| Property | Description | Default |
|----------|-------------|---------|
| `Kind` | `"File"` or `"Database"` | `"File"` |
| `Provider` | File: `"Json"`, `"Xml"`, `"Csv"` — Database: `"SqlServer"`, `"Sqlite"`, `"PostgreSQL"`, `"MySql"` | `"Json"` |
| `ConnectionString` | Direct connection string (database only) | — |
| `ConnectionStringName` | Named entry from `ConnectionStrings` section | — |
| `FilePath` | Custom file path (file-based only) | `Data/Products.{ext}` |

**Example — File-based (JSON, default):**
```json
{
  "Storage": { "Kind": "File", "Provider": "Json" }
}
```

**Example — SQL Server:**
```json
{
  "Storage": {
    "Kind": "Database",
    "Provider": "SqlServer",
    "ConnectionStringName": "DefaultConnection"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TelAvivMuni;Integrated Security=true;TrustServerCertificate=True"
  }
}
```

**Configuration Files:**
- `appsettings.json` - Default configuration for all environments
- `appsettings.Development.json` - Development-specific overrides (optional)

The application uses `Host.CreateDefaultBuilder()` which automatically loads configuration files based on the environment.

**Note:** Never commit sensitive credentials (usernames/passwords) to source control. Use environment-specific configuration files or secure configuration providers for production deployments.

## How to Use

### 1. Using the DataBrowserBox Control

#### Basic Usage (Auto-Generated Columns)

```xaml
<controls:DataBrowserBox
    Height="30"
    ItemsSource="{Binding Products}"
    SelectedItem="{Binding SelectedProduct, Mode=TwoWay}"
    DisplayMemberPath="Name"
    DialogTitle="Select Product"
    DialogService="{StaticResource DialogService}" />
```

#### Advanced Usage (Custom Columns)

```xaml
<controls:DataBrowserBox
    Height="30"
    ItemsSource="{Binding Products}"
    SelectedItem="{Binding SelectedProduct, Mode=TwoWay}"
    DisplayMemberPath="Name"
    DialogTitle="Select Product"
    DialogService="{StaticResource DialogService}">
    <controls:DataBrowserBox.Columns>
        <local:BrowserColumn DataField="Name" Header="שם מוצר" Width="200"/>
        <local:BrowserColumn DataField="Price" Header="מחיר" Width="100" Format="C2" HorizontalAlignment="Right"/>
        <local:BrowserColumn DataField="Category" Header="קטגוריה" Width="150"/>
        <local:BrowserColumn DataField="Code" Header="קוד" Width="120"/>
    </controls:DataBrowserBox.Columns>
</controls:DataBrowserBox>
```

**Properties:**
- `ItemsSource` - The collection of items to browse
- `SelectedItem` - The currently selected item (two-way binding, single-select mode)
- `SelectedItems` - The currently selected items collection (two-way binding, multi-select mode)
- `AllowMultipleSelection` - Enables multi-item selection mode; bind `SelectedItems` instead of `SelectedItem` (default: `false`)
- `DisplayMemberPath` - Property name to display in the control
- `DialogTitle` - Title for the browse dialog
- `DialogService` - Service for showing the dialog
- `Columns` - Optional custom column definitions (if not specified, columns are auto-generated)

**BrowserColumn Properties:**
- `DataField` - The property name to bind to
- `Header` - Column header text (supports Hebrew and other languages)
- `Width` - Column width in pixels (optional)
- `Format` - String format (e.g., "C2" for currency, "N0" for numbers)
- `HorizontalAlignment` - Text alignment: "Left", "Right", or "Center"

### 2. Setting Up Dialog Service

In `App.xaml`:
```xaml
<Application xmlns:pres="http://telaviv-muni-exe/presentation">
  <Application.Resources>
    <pres:DialogService x:Key="DialogService" />
    <pres:ViewModelLocator x:Key="Locator" />
  </Application.Resources>
</Application>
```

### 3. Data Format

Products.json example:
```json
[
  {
    "Id": 1,
    "Code": "LAP-001",
    "Name": "Laptop Pro 15",
    "Category": "Computers",
    "Price": 1299.99
  }
]
```

## Architecture

### Repository and Unit of Work Patterns

The application uses the Repository and Unit of Work patterns for data management, providing:

- **Abstraction** - Decouples business logic from data persistence
- **Testability** - Easy to mock for unit testing
- **Flexibility** - Can swap persistence strategies (file, database, etc.)

#### Key Components

**IRepository<TEntity>** - Generic repository interface:
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
```

**IUnitOfWork** - Coordinates repositories and manages transactions:
```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<Product> Products { get; }
    Task<int> SaveChangesAsync();
}
```

**OperationResult** - Consistent result type for CRUD operations:
```csharp
var result = await repository.AddAsync(product);
if (!result.Success)
{
    Console.WriteLine(result.ErrorMessage); // "A product with Id 1 already exists."
}
```

#### Layered Abstraction

The persistence layer uses Strategy pattern for flexibility:

```
ProductRepository (Core)
    └── IDataStore<Product> (Infrastructure)
            ├── FileDataStore<Product>           # File-based (JSON)
            │       └── ISerializer<Product>
            │               └── JsonSerializer<Product>
            │
            └── DbDataStore<Product, AppDbContext>  # Database (SQL Server)
                    └── IDbContextFactory<AppDbContext> (Core)
```

This allows:
- Swapping file-based storage for database storage (just change DI registration)
- Changing serialization format (JSON → XML) without changing repository
- Testing with in-memory implementations

## Key Features Implementation

### Column Configuration
The control supports two modes for column display:

1. **Auto-Generated Columns** (Default)
   - Automatically creates columns for all public properties
   - Simple to use, no configuration needed
   - Good for quick prototypes and simple scenarios

2. **Custom Columns**
   - Define exactly which columns to display and in what order
   - Support for Hebrew headers and RTL languages
   - Custom formatting (currency, numbers, dates)
   - Custom column widths and alignment
   - Only displays specified columns

### Search Functionality
- Searches across all object properties
- Case-insensitive matching
- Real-time filtering as you type
- Clear button appears automatically when text is entered
- Maintains selection after filtering (if visible)

### Selection Behavior
- **Initial Selection** - Previously selected item is highlighted when dialog opens
- **Auto-Scroll** - Selected item is automatically scrolled into view
- **Filter Persistence** - Selection remains when typing filter text (if item is visible)
- **Clear Filter Persistence** - Selection is restored when clearing the filter
- **Dialog Reopening** - Selection is maintained and highlighted when dialog reopens
- **Visual Feedback** - Bold blue text on light blue background (#E3F2FD, #1976D2)

### Visual States
- **Watermark** - Gray italic text: "Click to select..." when no item is selected
- **Selected Item** - Black bold text with item name displayed in control
- **No Selection Label** - Gray "No selection" text (hidden but preserves space when item is selected)
- **Grid Selection** - Blue background (#E3F2FD) with bold blue text (#1976D2)
- **Row Hover** - Light gray background (#F5F5F5)

### Button Styles
- **OK Button** - Blue (#2196F3) with white text, darker on hover
- **Cancel Button** - Light gray (#F5F5F5) with dark text, darker on hover
- **Browse Button** - Matches Cancel button style
- **Clear Button** - Circular, transparent, appears on hover when search text exists

## Building and Running

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022 (or any IDE with .NET 8 support)

### Build
```bash
dotnet build TelAvivMuni-Exercise.sln
```

### Run
```bash
dotnet run --project TelAvivMuni-Exercise/TelAvivMuni-Exercise.csproj
```

Or simply press F5 in Visual Studio.

### Run Tests
```bash
dotnet test TelAvivMuni-Exercise.Tests/TelAvivMuni-Exercise.Tests.csproj
```

### Run Tests with Code Coverage
```bash
dotnet test --settings coverlet.runsettings --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

Generate coverage report:
```bash
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:TextSummary
```

The test suite includes **176 unit tests** with **45.6% line coverage** and **39.2% branch coverage** across all assemblies (core testable code maintains near-100% coverage; lower overall figures reflect untested persistence provider registrars — CSV, XML, MySQL, PostgreSQL, SQLite, SqlServer — and the composition-root `StorageRegistrationExtensions` class):
- Repository operations (CRUD, error handling)
- Unit of Work coordination
- ViewModel commands and state management
- Data store and serialization (File and Database)
- AppDbContext entity operations
- DbDataStore CRUD operations
- OperationResult success/failure patterns
- BrowserColumn model properties
- DataBrowserDialogViewModel search, filter, and selection logic
- MainWindowViewModel CRUD commands and error handling

## Design Decisions

### MVVM Pattern with Attached Behaviors
- **Pure MVVM Implementation** - No code-behind event handlers for UI logic
- **Attached Behaviors** - Reusable, declarative UI behaviors defined in XAML
- **Clean Separation** - View logic separated from business logic
- **Testable** - Behaviors and ViewModels can be unit tested independently
- **Reusable** - Behaviors can be applied to any WPF control across the application

#### Implemented Behaviors:
1. **AutoFocusSearchBehavior** - Automatically focuses search box when typing starts anywhere in the dialog
2. **DataGridEnterBehavior** - Executes a command when Enter key is pressed in DataGrid
3. **DataGridScrollIntoViewBehavior** - Scrolls DataGrid to show the selected item when selection changes
4. **DialogCloseBehavior** - Closes window when ViewModel's DialogResult changes (MVVM-friendly)
5. **EscapeClearBehavior** - Clears TextBox content when Escape key is pressed

### Custom Control vs UserControl
- Chose Custom Control for better reusability
- Template-based approach for flexible styling
- Dependency properties for WPF-style usage
- Follows WPF best practices

### Dialog Service
- Abstraction layer for showing dialogs
- Easier to test and mock
- Centralized dialog logic
- Supports dependency injection
- Passes column configuration to dialog

### Error Handling Strategy
- Defensive programming approach
- Always initialize collections to empty (never null)
- User-friendly error messages
- Silent fallback for missing data files

### Selection Synchronization (View-First Pattern)
- **View-First Initialization** - Dialog window is created and rendered first, then data is loaded
- **Deferred Loading** - ViewModel stores pending items/selection until View is ready
- **Loading Overlay** - Semi-transparent "Loading..." overlay displayed while data loads
- **ContentRendered Event** - Triggers `Initialize()` method via XAML `CallMethodAction` (no code-behind)
- **IDeferredInitialization Interface** - Contract for ViewModels supporting deferred initialization
- **IColumnConfiguration Interface** - Contract for ViewModels providing custom column definitions
- Two-way XAML binding between DataGrid.SelectedItem and ViewModel.SelectedItem
- `DataGridScrollIntoViewBehavior` scrolls to selected item (MVVM-friendly)
- Clean MVVM separation: View depends only on interfaces, not concrete ViewModel types

### Keyboard Interaction (via Attached Behaviors)
- **Type-to-Search** - Start typing anywhere to focus search box automatically
- **Enter to Select** - Press Enter in DataGrid to select item and close dialog
- **Escape to Clear** - Press Escape in search box to clear filter text
- **Escape to Cancel** - Press Escape elsewhere to cancel and close dialog
- **No Code-Behind** - All keyboard handling implemented via reusable attached behaviors

## Recent Improvements

### Multi-Select Support + Clear Button Fix (v8.3)
- **`AllowMultipleSelection` property added** � `DataBrowserBox` now supports multi-item selection mode; set `AllowMultipleSelection=True` and bind `SelectedItems` (an `IList`) to receive the selected collection
- **`SelectedItems` dependency property** � Two-way bindable `IList` DP with `FrameworkPropertyMetadataOptions.BindsTwoWayByDefault`; subscribes to `INotifyCollectionChanged` when the collection reference changes
- **Clear button (�) fixed in multi-select mode** � `OnBrowseButtonClick` now explicitly calls `UpdateHasSelection()` and `UpdateDisplayText()` after in-place `SelectedItems.Clear()` + `Add()` mutations, mirroring the pattern already used by `OnClearButtonClick`; `UpdateHasSelection()` also sets `_clearButton.Visibility` directly as a belt-and-suspenders bypass for the WPF `ControlTemplate.Trigger` pipeline
- **`DataBrowserDialog` multi-select DataGrid** � `SelectionMode=Extended` now correctly applied via a Style `<Setter>` (not a local attribute) so `AllowMultipleSelection` can override it through the `DataTrigger` without being shadowed by local-value precedence
- **13 new unit tests** in `DataBrowserDialogViewModelTests` covering multi-select constructor, preselection, `OkCommand.CanExecute` with selection states, and filter interactions � **176 unit tests** total
### Ayu Dark Theme (v8.2)
- **`TelAvivMuni-Exercise.Themes.Zed.AyuDark` added** — New standalone theme assembly implementing the Zed editor built-in [Ayu Dark](https://github.com/zed-industries/zed/blob/main/assets/themes/ayu/ayu.json) palette, following the exact same three-file structure as `TelAvivMuni-Exercise.Themes.Zed.GruvboxDark`
- **Exact Zed palette** — All 21 brush keys (`PrimaryBrush #5ac1fe`, `NeutralDarkBrush #313337`, `TextPrimaryBrush #bfbdb6`, `WhiteBrush #0d1016`, etc.) sourced directly from Zed's `assets/themes/ayu/ayu.json`
- **DataBrowserBox dark theme override** — `AyuDark.Styles.xaml` provides an implicit `Style TargetType="controls:DataBrowserBox"` using `AyuDarkBrowseButtonStyle` and `AyuDarkDataBrowserTextBoxStyle` (internal keyed styles), consistent with the GruvboxDark approach
- **Project reference to Controls** — `Themes.Zed.AyuDark` references `TelAvivMuni-Exercise.Controls` and `TelAvivMuni-Exercise.Themes` (no circular dependencies)
- **App.xaml updated** — Comment block lists all four available theme URIs (Blue, Emerald, GruvboxDark, AyuDark) for easy theme switching
- **All 163 unit tests** pass unchanged (pure XAML assembly, no C# logic to test)

### GruvboxDark Theme + Full Dark UI Theming (v8.1)
- **`TelAvivMuni-Exercise.Themes.Zed.GruvboxDark` added** — New standalone theme assembly implementing the [morhetz/gruvbox](https://github.com/morhetz/gruvbox) dark palette (`bg0 #282828`, `fg1 #ebdbb2`, green accent `#689d6a`)
- **DataBrowserBox dark theme override** — `GruvboxDark.Styles.xaml` provides an implicit `Style TargetType="controls:DataBrowserBox"` that overrides `Generic.xaml`'s light defaults at app level, using only `StaticResource` (no `DynamicResource`)
- **Project reference to Controls** — `Themes.Zed.GruvboxDark` references `TelAvivMuni-Exercise.Controls` (no circular dependency) to allow the `xmlns:controls` namespace in `GruvboxDark.Styles.xaml`
- **`MainWindow.xaml` fully themed** — Background, all `TextBlock` foregrounds, and card `Border` brushes replaced with theme-aware `StaticResource` keys (`NeutralDarkBrush`, `TextPrimaryBrush`, `TextTertiaryBrush`, `WhiteBrush`, `BorderLightBrush`)
- **`DataBrowserDialog.xaml` fully themed** — Background, search-box border, `TextBox` foreground/caret, and loading overlay replaced with `StaticResource` keys; hard-coded `"White"` and `"#80FFFFFF"` removed
- **`LoadingOverlayBrush` added to all themes** — `Shared.xaml` defines the fallback (`#80FFFFFF`); `Blue.Colors.xaml` and `Emerald.Colors.xaml` override with the same light value; `GruvboxDark.Colors.xaml` overrides with semi-transparent dark (`#C0282828`)
- **All 163 unit tests** pass unchanged

### Theme Assembly Extraction (v8.0)
- **Themes extracted to separate assemblies** — `TelAvivMuni-Exercise.Themes` (base), `.Themes.Blue`, and `.Themes.Emerald` are now standalone WPF class libraries, decoupling visual theming from the composition root
- **`DynamicResource` eliminated** — All resource lookups replaced with `StaticResource` for faster XAML parsing and reduced runtime overhead (no runtime re-resolution)
- **`Generic.xaml` self-contained** — The control template dictionary merges `Shared.xaml` via pack URI, providing its own brush scope; `BrowseButtonStyle` and `DataBrowserTextBoxStyle` moved from theme files into `Generic.xaml` where they architecturally belong (internal parts of `DataBrowserBox`)
- **Pack URIs for cross-assembly references** — `App.xaml` loads the active theme via `pack://application:,,,/TelAvivMuni-Exercise.Themes.Blue;component/Themes/Blue.xaml`
- **Theme entry-point aggregators** — Each theme's root XAML (`Blue.xaml`, `Emerald.xaml`) merges `Shared.xaml` + its own styles, providing a single include point for consumers
- **163 unit tests** — all pass unchanged

### Controls XmlnsDefinition (v7.3)
- **`[assembly: XmlnsDefinition]`** added to `TelAvivMuni-Exercise.Controls/AssemblyInfo.cs`, mapping `http://telaviv-muni-exe/controls` to both `TelAvivMuni_Exercise.Controls` and `TelAvivMuni_Exercise.Controls.Behaviors` — mirrors the same convention already established in the Presentation assembly
- **Consumer XAML simplified** — `MainWindow.xaml` replaces the verbose `clr-namespace:TelAvivMuni_Exercise.Controls;assembly=TelAvivMuni-Exercise.Controls` with `xmlns:controls="http://telaviv-muni-exe/controls"`
- **Intra-assembly references unchanged** — `DataBrowserDialog.xaml` and `Generic.xaml` continue to use bare `clr-namespace:` (no `assembly=`), which is correct for references within the same assembly
- **MC3074 fix** — `MainWindow.xaml` `xmlns:local` restored to `clr-namespace:TelAvivMuni_Exercise.Domain;assembly=TelAvivMuni-Exercise.Domain`; Domain stays `net8.0` (platform-neutral) so `XmlnsDefinitionAttribute` (WPF-only) cannot be applied to it — the full `clr-namespace:...;assembly=...` form is required here
- **163 unit tests** — all pass unchanged

### MainWindow Moved to Presentation (v7.2)
- **`MainWindow.xaml(.cs)` relocated** to `TelAvivMuni-Exercise.Presentation/Views/` — the Presentation project now owns all view-layer files
- **WinExe project is a pure composition root** — `TelAvivMuni-Exercise` contains only `App`, DI wiring, themes, and configuration; no Window types remain there
- **`[assembly: XmlnsDefinition]`** added to `TelAvivMuni-Exercise.Presentation/AssemblyInfo.cs`, mapping the URI `http://telaviv-muni-exe/presentation` to all Presentation namespaces; `App.xaml` now uses a single `xmlns:pres` alias instead of separate `xmlns:services`/`xmlns:localInfra` declarations
- **Programmatic startup** — `StartupUri` replaced by `new MainWindow().Show()` in `OnStartup`, the idiomatic WPF approach when the startup window lives in a referenced assembly
- **163 unit tests** — all pass unchanged; `MainWindow` was already `[ExcludeFromCodeCoverage]`

### Assembly Discovery Refactor (v7.1)
- **Removed hardcoded assembly name** - `StorageRegistrationExtensions` no longer references the literal string `"TelAvivMuni-Exercise.Core.dll"` for `IDbContextRegistrar` discovery
- **Added `CoreAssembly`** - A lightweight static helper in the Core project that exposes `Assembly.GetExecutingAssembly()`, ensuring the correct assembly is always resolved at runtime regardless of what calls it
- **Shared `ScanAssembly` helper** - The type-scanning loop is now extracted and reused by both the glob-based and assembly-based overloads of `DiscoverRegistrars`
- **163 unit tests** - Added `CoreAssemblyTests` to verify the assembly name and identity

### Project Decomposition Refactor (v7.0)
- **Extracted `TelAvivMuni-Exercise.Domain`** - Domain models (Product, BrowserColumn, OperationResult) now in a dedicated project
- **Extracted `TelAvivMuni-Exercise.Controls`** - WPF controls and attached behaviors separated from the main WPF project
- **Extracted `TelAvivMuni-Exercise.Presentation`** - ViewModels, DialogService, and ViewModelLocator in their own project
- **Moved `IEntity` to Infrastructure** - Base entity interface lives with persistence abstractions
- **Moved `AppDbContext` to Core** - DbContext consolidated with business logic patterns
- **7 clean projects** - Infrastructure → Domain → Core.Contracts → Core → Controls/Presentation → WPF
- **163 unit tests** - Test project references all projects for comprehensive coverage

### Architectural Layering Refactor (v6.0)
- **Core project no longer depends on EF Core** - Clean separation of business logic from infrastructure
- **Eliminated circular dependencies** - Clear dependency direction
- **Better layering** - Infrastructure owns persistence, Core owns business logic patterns

### Database Support (v5.0)
- **Entity Framework Core 8.0** - Added SQL Server database support
- **DbDataStore<TEntity, TContext>** - Generic database data store implementing `IDataStore<T>`
- **AppDbContext** - EF Core DbContext with Product entity configuration
- **Pluggable persistence** - Switch between file and database storage via DI configuration
- **163 unit tests** - Expanded test coverage including database operations
- **InMemory testing** - Database tests use EF Core InMemory provider

### Local Database Setup

Follow these steps to run the SQL Server–backed version of the application locally.

1. **Install SQL Server (or use a local container)**
   - Install **SQL Server Developer** or **SQL Server Express** on your machine  
     – or –  
     run a local SQL Server container (for example, using the official `mcr.microsoft.com/mssql/server` image).
   - Ensure you have a SQL client tool installed, such as **SQL Server Management Studio (SSMS)**, **Azure Data Studio**, or the `sqlcmd` CLI.

2. **Generate and/or run the database script**
   - Option A – **PowerShell script**  
     Run `GenerateSqlScript.ps1` from a PowerShell prompt in the repository root (or the folder where the script resides):
     ```powershell
     pwsh ./GenerateSqlScript.ps1
     ```
     This will generate the SQL script needed to create the database and tables (if applicable).
   - Option B – **SQL script**  
     Open `CreateProductsDatabase.sql` in your SQL client and execute it against your local SQL Server instance.

3. **Create the database and tables**
   - Ensure that the script is executed against the intended server/instance (for example, `localhost` or `localhost,1433`).
   - After running `CreateProductsDatabase.sql` (directly or via the PowerShell script), verify that:
     - A database (for example, `ProductsDb`) has been created.
     - The `Products` table (and any other required tables) exists and was created without errors.

4. **Configure the connection string**
   - Locate the application’s connection string configuration (for example, in `appsettings.json`, `App.config`, or `appsettings.Development.json`).
   - Update the SQL Server connection string used by the application (for example, a connection named `ProductsConnection`) so that:
     - `Server` (or `Data Source`) points to your local SQL Server instance, such as `localhost` or `localhost\\SQLEXPRESS`.
     - `Database` (or `Initial Catalog`) matches the database name created by `CreateProductsDatabase.sql`.
     - Authentication (Integrated Security or User ID/Password) matches how you connect to your local SQL Server.

5. **Verify the setup**
   - Start your local SQL Server instance and confirm the database is reachable using your SQL client.
   - Run the WPF application.
   - Navigate to the part of the UI that loads products. If the database is configured correctly, product data should load from SQL Server without errors.
   - Optionally, add or edit a product in the application and confirm that the changes appear in the `Products` table when queried from your SQL client.
### Test Coverage (v4.0)
- **163 unit tests** - Comprehensive test coverage for all business logic
- **41.4% line coverage** - Measured via coverlet with Cobertura reporting (core testable code at near-100%; lower overall figure reflects untested persistence provider registrars — CSV, XML, MySQL, PostgreSQL, SQLite, SqlServer — and composition-root classes)
- **34.0% branch coverage** - Covers conditional logic paths
- **Coverage exclusions** - WPF UI components (behaviors, controls, dialogs) are excluded using `[ExcludeFromCodeCoverage]` attribute
- **Coverlet configuration** - `coverlet.runsettings` file for consistent coverage measurement

### Repository and Unit of Work Patterns (v3.0)
- **Added Repository pattern** - Abstracts data access with `IRepository<T>`
- **Added Unit of Work pattern** - Coordinates repositories via `IUnitOfWork`
- **Layered persistence** - `IDataStore<T>` and `ISerializer<T>` for flexibility
- **OperationResult type** - Consistent error handling with descriptive messages
- **Improved testability** - All dependencies are injectable and mockable

### MVVM Refactoring (v2.0)
- **Removed ~80 lines** of keyboard event handling code from code-behind
- **Added 4 reusable attached behaviors** for declarative UI logic
- **Improved maintainability** - Behaviors are independently testable and reusable
- **Better separation of concerns** - View logic moved from code-behind to XAML
- **Cleaner architecture** - True MVVM pattern with no UI logic in code-behind

## Future Enhancements

Potential improvements for production use:
- Add virtualization for large datasets (currently loads all items)
- Implement column sorting (click column headers to sort)
- Add column resizing (drag column borders)
- Add export functionality (CSV, Excel)
- Custom column templates (images, buttons, checkboxes)
- Additional behaviors (double-click to select, arrow key navigation)
- Accessibility features (screen reader support, high contrast themes)
- Full localization support (resources for all strings)
- Column reordering (drag-and-drop columns)
- Save/restore column configurations
- Advanced filtering (dropdown filters per column)

## License

This project is created for educational and interview purposes.

## Author

Created as part of the Tel-Aviv Municipality interview process for Software Developer position.

## Contact

For questions or feedback about this project, please contact through the interview process.
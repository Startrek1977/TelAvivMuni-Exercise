# TelAviv Municipality Exercise

A WPF application demonstrating a reusable data browser control for selecting items from collections with search and filtering capabilities.

## Project Overview

This project is a home exercise created as part of an interview for the Software Developer position at Tel-Aviv Municipality. It showcases a custom WPF control (`DataBrowserBox`) that provides a modern, user-friendly interface for browsing and selecting items from data collections.

## Features

### Custom DataBrowserBox Control
- **Reusable WPF Custom Control** - Can be used anywhere in the application with any data type
- **Display Member Path** - Configure which property to display in the control
- **Two-Way Data Binding** - Full support for MVVM pattern with `SelectedItem` binding
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
  - CommunityToolkit.Mvvm 8.4.0 - For MVVM infrastructure
  - Microsoft.EntityFrameworkCore 8.0.12 - ORM for database access
  - Microsoft.EntityFrameworkCore.SqlServer 8.0.12 - SQL Server provider
  - Microsoft.Extensions.DependencyInjection 10.0.2 - IoC container
  - Microsoft.Extensions.DependencyInjection.Abstractions 10.0.2 - DI abstractions
  - Microsoft.Extensions.Hosting 10.0.2 - Host builder for DI setup
  - Microsoft.Xaml.Behaviors.Wpf 1.1.135 - Attached behaviors
  - System.Text.Json - For JSON serialization
  - xUnit 2.7.0 - For unit testing
  - xunit.runner.visualstudio 2.5.7 - Test runner for Visual Studio
  - Microsoft.NET.Test.Sdk 17.9.0 - Test platform
  - Microsoft.EntityFrameworkCore.InMemory 8.0.12 - For database testing
  - Moq 4.20.70 - For mocking in tests
  - coverlet.collector 6.0.1 - For code coverage

## Solution Structure

The solution is organized into multiple projects to separate concerns and promote reusability:

### TelAvivMuni-Exercise.Infrastructure
Generic, reusable infrastructure code that can be used in any project:
- **Interfaces:**
  - `IDataStore<T>` - Data persistence abstraction
  - `IRepository<T>` - Generic repository interface
  - `IEntity` - Base entity interface
  - `ISerializer<T>` - Serialization abstraction
  - `IDeferredInitialization` - View-First initialization interface
- **Data Stores:**
  - `FileDataStore<T>` - File-based data store with thread-safe async operations
  - `DbDataStore<TEntity, TContext>` - SQL Server database data store using Entity Framework Core
- **Implementations:**
  - `JsonSerializer<T>` - JSON serialization using System.Text.Json
  - `OperationResult` - Operation result with success/failure states

### TelAvivMuni-Exercise.Core
Application-specific but reusable business logic and models:
- **Models:**
  - `Product` - Product data model implementing IEntity
  - `BrowserColumn` - Column configuration for data browser
- **Data:**
  - `AppDbContext` - Entity Framework Core DbContext for SQL Server
- **Repositories:**
  - `ProductRepository` - Product-specific repository implementation
  - `UnitOfWork` - Unit of Work coordination pattern
- **Contracts:**
  - `IUnitOfWork` - Unit of Work interface
- **Services:**
  - `IDialogService` - Dialog service interface
- **Config:**
  - `IColumnConfiguration` - Column configuration interface

### TelAvivMuni-Exercise (Main WPF Application)
WPF-specific UI code:
- **Controls:**
  - `DataBrowserBox` - Custom reusable control for item selection
  - `DataBrowserDialog` - Browse dialog UI
- **ViewModels:**
  - `MainWindowViewModel` - Main window view model
  - `DataBrowserDialogViewModel` - Dialog view model
- **Services:**
  - `DialogService` - Dialog service implementation (WPF-dependent)
- **Infrastructure/Behaviors:** - WPF Attached Behaviors (MVVM pattern)
  - `AutoFocusSearchBehavior` - Auto-focus on typing
  - `DataGridEnterBehavior` - Handle Enter key in DataGrid
  - `DataGridScrollIntoViewBehavior` - Scroll to selected item
  - `DialogCloseBehavior` - MVVM-friendly dialog closing
  - `EscapeClearBehavior` - Clear text on Escape key
- **Infrastructure:**
  - `ViewModelLocator` - DI-based ViewModel resolution for XAML
  - `ICommand.Extension` - Command extension methods (WPF/MVVM-dependent)
- **Themes:** - Control templates and styles
- **Data:** - Sample product data (Products.json)

### TelAvivMuni-Exercise.Tests
Unit tests for all projects

### Project Dependencies
```
TelAvivMuni-Exercise (WPF)
    ├── → TelAvivMuni-Exercise.Core
    └── → TelAvivMuni-Exercise.Infrastructure

TelAvivMuni-Exercise.Core
    └── → TelAvivMuni-Exercise.Infrastructure

TelAvivMuni-Exercise.Tests
    ├── → TelAvivMuni-Exercise (WPF)
    ├── → TelAvivMuni-Exercise.Core
    └── → TelAvivMuni-Exercise.Infrastructure
```

## Project Structure

```
TelAvivMuni-Exercise.sln
│
├── TelAvivMuni-Exercise.Infrastructure/     # Generic, reusable infrastructure
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
├── TelAvivMuni-Exercise.Core.Contracts/     # Shared contracts and models
│   ├── Config/
│   │   └── IColumnConfiguration.cs          # Column configuration interface
│   ├── Models/
│   │   ├── BrowserColumn.cs                 # Column configuration model
│   │   ├── OperationResult.cs               # Operation result with error messages
│   │   └── Product.cs                       # Product data model
│   ├── Patterns/
│   │   ├── IRepositoryT.cs                  # Generic repository interface
│   │   └── IUnitOfWork.cs                   # Unit of Work interface
│   └── Services/
│       └── IDialogService.cs                # Dialog service interface
│
├── TelAvivMuni-Exercise.Core/               # Application-specific business logic
│   ├── Data/
│   │   └── AppDbContext.cs                  # EF Core DbContext
│   └── Patterns/
│       ├── ProductRepository.cs             # Product-specific repository
│       └── UnitOfWork.cs                    # Unit of Work implementation
│
├── TelAvivMuni-Exercise/                    # WPF Application
│   ├── Controls/
│   │   ├── DataBrowserBox.cs                # Custom reusable control
│   │   └── DataBrowserDialog.xaml(.cs)      # Browse dialog UI
│   ├── Infrastructure/
│   │   ├── Behaviors/                       # WPF Attached Behaviors (MVVM)
│   │   │   ├── AutoFocusSearchBehavior.cs
│   │   │   ├── DataGridEnterBehavior.cs
│   │   │   ├── DataGridScrollIntoViewBehavior.cs
│   │   │   ├── DialogCloseBehavior.cs
│   │   │   └── EscapeClearBehavior.cs
│   │   ├── ICommand.Extension.cs            # ICommand extension methods
│   │   └── ViewModelLocator.cs              # DI-based ViewModel resolution
│   ├── Services/
│   │   └── DialogService.cs                 # Dialog service implementation
│   ├── Themes/
│   │   ├── Generic.xaml                     # Control templates and styles
│   │   ├── Theme1.xaml
│   │   └── Theme2.xaml
│   ├── ViewModels/
│   │   ├── DataBrowserDialogViewModel.cs    # Dialog view model
│   │   └── MainWindowViewModel.cs           # Main window view model
│   ├── Data/
│   │   └── Products.json                    # Sample product data
│   ├── App.xaml(.cs)                        # Application entry point with DI
│   └── MainWindow.xaml(.cs)                 # Main application window
│
├── TelAvivMuni-Exercise.Tests/              # Unit test project
│   ├── Infrastructure/
│   │   ├── AppDbContextTests.cs
│   │   ├── DbDataStoreTests.cs
│   │   ├── FileDataStoreTests.cs
│   │   ├── IEntityTests.cs
│   │   ├── JsonSerializerTests.cs
│   │   ├── OperationResultTests.cs
│   │   ├── ProductRepositoryTests.cs
│   │   └── UnitOfWorkTests.cs
│   ├── Models/
│   │   └── BrowserColumnTests.cs
│   └── ViewModels/
│       ├── DataBrowserDialogViewModelTests.cs
│       └── MainWindowViewModelTests.cs
│
└── coverlet.runsettings                     # Code coverage configuration
```

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
- `SelectedItem` - The currently selected item (two-way binding)
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
<Application.Resources>
    <services:DialogService x:Key="DialogService" />
</Application.Resources>
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
ProductRepository
    └── IDataStore<Product>
            ├── FileDataStore<Product>           # File-based (JSON)
            │       └── ISerializer<Product>
            │               └── JsonSerializer<Product>
            │
            └── DbDataStore<Product, AppDbContext>  # Database (SQL Server)
                    └── IDbContextFactory<AppDbContext>
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
dotnet build TelAvivMuni-Exercise.csproj
```

### Run
```bash
dotnet run --project TelAvivMuni-Exercise.csproj
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

The test suite includes **158 unit tests** with comprehensive coverage on all testable code:
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

### Database Support (v5.0)
- **Entity Framework Core 8.0** - Added SQL Server database support
- **DbDataStore<TEntity, TContext>** - Generic database data store implementing `IDataStore<T>`
- **AppDbContext** - EF Core DbContext with Product entity configuration
- **Pluggable persistence** - Switch between file and database storage via DI configuration
- **158 unit tests** - Expanded test coverage including database operations
- **InMemory testing** - Database tests use EF Core InMemory provider

### Test Coverage (v4.0)
- **158 unit tests** - Comprehensive test coverage for all business logic
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
- Multi-selection support (Ctrl+Click, Shift+Click)
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
# Setup Guide

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Visual Studio 2022 (or any IDE with .NET 8 WPF support, e.g. Rider)
- *(Database mode only)* An accessible instance of SQL Server, SQLite, PostgreSQL, or MySQL

## Build

```bash
dotnet build TelAvivMuni-Exercise.sln
```

## Run

```bash
dotnet run --project TelAvivMuni-Exercise/TelAvivMuni-Exercise.csproj
```

Or press **F5** in Visual Studio.

## Run Tests

```bash
dotnet test TelAvivMuni-Exercise.Tests/TelAvivMuni-Exercise.Tests.csproj
```

### Run Tests with Code Coverage

```bash
dotnet test --settings coverlet.runsettings --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

Generate a human-readable coverage report (requires `reportgenerator` tool):

```bash
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" \
                -targetdir:"coveragereport" \
                -reporttypes:TextSummary
```

Current coverage: **176 unit tests**, **45.6% line coverage**, **39.2% branch coverage** across all assemblies. Core testable code (repositories, ViewModels, domain models, data stores) maintains near-100% coverage; lower overall figures reflect untested provider registrar classes (CSV, XML, MySQL, PostgreSQL, SQLite, SqlServer) and the composition-root `StorageRegistrationExtensions`.

## Storage Configuration

The application reads `TelAvivMuni-Exercise/appsettings.json` (and optionally `appsettings.Development.json`) on startup.

### Storage options

| Property | Description | Default |
|----------|-------------|---------|
| `Kind` | `"File"` or `"Database"` | `"File"` |
| `Provider` | File: `"Json"`, `"Xml"`, `"Csv"` — Database: `"SqlServer"`, `"Sqlite"`, `"PostgreSQL"`, `"MySql"` | `"Json"` |
| `ConnectionString` | Inline connection string (database only) | — |
| `ConnectionStringName` | Key from the `ConnectionStrings` section | — |
| `FilePath` | Custom file path (file-based only) | `Data/Product.{ext}` |

### Example — File-based JSON (simplest)

```json
{
  "Storage": { "Kind": "File", "Provider": "Json" }
}
```

### Example — File-based XML

```json
{
  "Storage": { "Kind": "File", "Provider": "Xml" }
}
```

### Example — SQL Server

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

### Example — SQLite

```json
{
  "Storage": {
    "Kind": "Database",
    "Provider": "Sqlite",
    "ConnectionString": "Data Source=TelAvivMuni.db"
  }
}
```

### Example — PostgreSQL

```json
{
  "Storage": {
    "Kind": "Database",
    "Provider": "PostgreSQL",
    "ConnectionString": "Host=localhost;Database=TelAvivMuni;Username=postgres;Password=secret"
  }
}
```

> **Security note:** Never commit credentials to source control. Use `appsettings.Development.json` (git-ignored) or environment variables for secrets.

## Local SQL Server Setup

Follow these steps to run the SQL Server-backed version locally.

### 1. Install SQL Server

Install **SQL Server Developer** or **SQL Server Express**, or run a local container:

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourPassword123!" \
  -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

Have a SQL client ready: **SSMS**, **Azure Data Studio**, or `sqlcmd`.

### 2. Create the database and tables

**Option A — PowerShell script:**

```powershell
pwsh ./Data/GenerateSqlScript.ps1
```

**Option B — Run the SQL script directly:**

Open `Data/CreateProductsDatabase.sql` in your SQL client and execute it against your SQL Server instance.

After running the script, verify that:
- The database (e.g. `TelAvivMuni`) was created.
- The `Products` table exists.

### 3. Update the connection string

Edit `TelAvivMuni-Exercise/appsettings.json`:

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

Adjust `Server`, `Database`, and authentication to match your instance.

### 4. Verify

Start the application and navigate to the product browser. Products should load from SQL Server. Add a product in the UI and confirm the row appears in the `Products` table when queried from your SQL client.

## Switching the Active Theme

Open `TelAvivMuni-Exercise/App.xaml` and replace the merged dictionary URI with the desired theme:

```xml
<!-- Blue (default light) -->
<ResourceDictionary Source="pack://application:,,,/TelAvivMuni-Exercise.Themes.Blue;component/Themes/Blue.xaml" />

<!-- Emerald (green light) -->
<ResourceDictionary Source="pack://application:,,,/TelAvivMuni-Exercise.Themes.Emerald;component/Themes/Emerald.xaml" />

<!-- Gruvbox Dark -->
<ResourceDictionary Source="pack://application:,,,/TelAvivMuni-Exercise.Themes.Zed.GruvboxDark;component/Themes/GruvboxDark.xaml" />

<!-- Ayu Dark -->
<ResourceDictionary Source="pack://application:,,,/TelAvivMuni-Exercise.Themes.Zed.AyuDark;component/Themes/AyuDark.xaml" />
```

## Data Files

Sample product data lives in the `Data/` folder at solution root:

| File | Format |
|------|--------|
| `Data/Product.json` | JSON |
| `Data/Product.xml` | XML |
| `Data/Product.csv` | CSV |
| `Data/CreateProductsDatabase.sql` | SQL Server DDL + seed data |
| `Data/GenerateSqlScript.ps1` | PowerShell helper to generate the SQL script |

The WPF project copies all three file formats to its output `Data/` folder at build time. The active provider reads from `Data/Product.{ext}` by default; override with the `FilePath` setting.

## Adding a New Provider (Plugin Architecture)

Provider assemblies are discovered at runtime — no changes to the composition root are needed:

1. Create a class library implementing `IFileProviderRegistrar` (file) or `IDbProviderRegistrar` (database).
2. Build and copy the DLL to the application's bin directory.
3. Update `appsettings.json` to reference the new `Provider` name.
4. Restart the application.

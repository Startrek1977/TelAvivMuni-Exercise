namespace TelAvivMuni_Exercise;

/// <summary>
/// Configuration options for the storage provider.
/// Bound from the "Storage" section in appsettings.json.
/// </summary>
public class StorageOptions
{
	public const string SectionName = "Storage";

	/// <summary>
	/// The storage kind: "Database" or "File".
	/// </summary>
	public string Kind { get; set; } = "File";

	/// <summary>
	/// The specific provider within the selected kind.
	/// Database: "SqlServer", "Sqlite", "PostgreSQL".
	/// File: "Json", "Xml", "Csv".
	/// </summary>
	public string Provider { get; set; } = "Json";

	/// <summary>
	/// Direct connection string for database providers.
	/// Takes precedence over ConnectionStringName when both are set.
	/// </summary>
	public string? ConnectionString { get; set; }

	/// <summary>
	/// References a named entry in the standard ConnectionStrings section.
	/// Used when ConnectionString is not set directly.
	/// </summary>
	public string? ConnectionStringName { get; set; }

	/// <summary>
	/// File path for file-based providers.
	/// Defaults to Data/Products.{ext} relative to the application base directory.
	/// </summary>
	public string? FilePath { get; set; }
}

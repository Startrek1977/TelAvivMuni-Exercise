namespace TelAvivMuni_Exercise.Persistence.FileBase.Csv;

/// <summary>
/// Registers CSV as the file storage provider.
/// </summary>
public class CsvFileProviderRegistrar : IFileProviderRegistrar
{
	/// <inheritdoc />
	public string ProviderName => "Csv";

	/// <inheritdoc />
	public string FileExtension => ".csv";

	/// <inheritdoc />
	public ISerializer<T> CreateSerializer<T>() where T : class
		=> new CsvSerializer<T>();
}

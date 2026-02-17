namespace TelAvivMuni_Exercise.Persistence.FileBase.Json;

/// <summary>
/// Registers JSON as the file storage provider.
/// </summary>
public class JsonFileProviderRegistrar : IFileProviderRegistrar
{
	/// <inheritdoc />
	public string ProviderName => "Json";

	/// <inheritdoc />
	public string FileExtension => ".json";

	/// <inheritdoc />
	public ISerializer<T> CreateSerializer<T>() where T : class
		=> new JsonSerializer<T>();
}

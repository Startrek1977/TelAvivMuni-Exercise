using TelAvivMuni_Exercise.Persistence;

namespace TelAvivMuni_Exercise.Persistence.FileBase;

/// <summary>
/// Registers a specific file-based storage provider.
/// Implement this interface to add support for a new file format (JSON, XML, CSV, etc.).
/// </summary>
public interface IFileProviderRegistrar
{
	/// <summary>
	/// The provider name this registrar handles (case-insensitive match against StorageOptions.Provider).
	/// </summary>
	string ProviderName { get; }

	/// <summary>
	/// The file extension used by this provider (e.g., ".json", ".xml").
	/// </summary>
	string FileExtension { get; }

	/// <summary>
	/// Creates a serializer for the given entity type.
	/// </summary>
	ISerializer<T> CreateSerializer<T>() where T : class;
}

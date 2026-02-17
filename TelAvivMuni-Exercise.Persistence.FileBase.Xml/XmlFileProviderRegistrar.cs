namespace TelAvivMuni_Exercise.Persistence.FileBase.Xml;

/// <summary>
/// Registers XML as the file storage provider.
/// </summary>
public class XmlFileProviderRegistrar : IFileProviderRegistrar
{
	/// <inheritdoc />
	public string ProviderName => "Xml";

	/// <inheritdoc />
	public string FileExtension => ".xml";

	/// <inheritdoc />
	public ISerializer<T> CreateSerializer<T>() where T : class
		=> new XmlSerializer<T>();
}

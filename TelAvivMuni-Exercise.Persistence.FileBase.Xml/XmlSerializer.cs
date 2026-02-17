using System.IO;
using System.Xml;
using SystemXmlSerializer = System.Xml.Serialization.XmlSerializer;

namespace TelAvivMuni_Exercise.Persistence.FileBase.Xml;

/// <summary>
/// XML implementation of the serializer using BCL System.Xml.Serialization.
/// </summary>
/// <typeparam name="T">The type of entity to serialize.</typeparam>
public class XmlSerializer<T> : ISerializer<T> where T : class
{
	private readonly SystemXmlSerializer _xmlSerializer = new(typeof(T[]));

	/// <inheritdoc />
	public string FileExtension => ".xml";

	/// <inheritdoc />
	public Task<string> SerializeAsync(IEnumerable<T> entities)
	{
		ArgumentNullException.ThrowIfNull(entities);

		var settings = new XmlWriterSettings { Indent = true, Async = true };
		using var sw = new StringWriter();
		using var writer = XmlWriter.Create(sw, settings);
		_xmlSerializer.Serialize(writer, entities.ToArray());
		return Task.FromResult(sw.ToString());
	}

	/// <inheritdoc />
	public Task<T[]> DeserializeAsync(string content)
	{
		if (string.IsNullOrWhiteSpace(content))
			return Task.FromResult(Array.Empty<T>());

		try
		{
			using var reader = new StringReader(content);
			var result = _xmlSerializer.Deserialize(reader) as T[];
			return Task.FromResult(result ?? []);
		}
		catch (InvalidOperationException)
		{
			return Task.FromResult(Array.Empty<T>());
		}
	}
}

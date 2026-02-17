using System.IO;
using System.Text.Json;

namespace TelAvivMuni_Exercise.Persistence.FileBase.Json;

/// <summary>
/// JSON implementation of the serializer using System.Text.Json.
/// Uses stream-based async operations for true non-blocking I/O.
/// </summary>
/// <typeparam name="T">The type of entity to serialize.</typeparam>
public class JsonSerializer<T>(JsonSerializerOptions? options = null) : ISerializer<T> where T : class
{
	private readonly JsonSerializerOptions? _options = options ?? new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true,
		WriteIndented = true
	};

	/// <inheritdoc />
	public string FileExtension => ".json";

	/// <inheritdoc />
	public async Task<string> SerializeAsync(IEnumerable<T> entities)
	{
		ArgumentNullException.ThrowIfNull(entities);

		using (var stream = new MemoryStream())
		{
			await System.Text.Json.JsonSerializer.SerializeAsync(stream, entities, _options);
			stream.Position = 0;
			using (var reader = new StreamReader(stream))
			{
				return await reader.ReadToEndAsync();
			}
		}
	}

	/// <inheritdoc />
	public async Task<T[]> DeserializeAsync(string content)
	{
		if (string.IsNullOrWhiteSpace(content))
		{
			return [];
		}

		try
		{
			using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)))
			{
				var entities = await System.Text.Json.JsonSerializer.DeserializeAsync<T[]>(stream, _options);
				return entities ?? [];
			}
		}
		catch (JsonException)
		{
			return [];
		}
	}
}

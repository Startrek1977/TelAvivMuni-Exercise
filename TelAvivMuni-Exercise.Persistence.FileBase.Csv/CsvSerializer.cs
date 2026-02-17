using System.IO;
using System.Reflection;
using System.Text;

namespace TelAvivMuni_Exercise.Persistence.FileBase.Csv;

/// <summary>
/// CSV implementation of the serializer using RFC-4180 format.
/// Serializes public readable/writable properties as columns.
/// </summary>
/// <typeparam name="T">The type of entity to serialize.</typeparam>
public class CsvSerializer<T> : ISerializer<T> where T : class
{
	private static readonly PropertyInfo[] _properties =
		typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(p => p.CanRead && p.CanWrite)
			.ToArray();

	/// <inheritdoc />
	public string FileExtension => ".csv";

	/// <inheritdoc />
	public Task<string> SerializeAsync(IEnumerable<T> entities)
	{
		ArgumentNullException.ThrowIfNull(entities);

		var sb = new StringBuilder();
		sb.AppendLine(string.Join(",", _properties.Select(p => Escape(p.Name))));

		foreach (var entity in entities)
		{
			sb.AppendLine(string.Join(",", _properties.Select(p => Escape(p.GetValue(entity)?.ToString()))));
		}

		return Task.FromResult(sb.ToString());
	}

	/// <inheritdoc />
	public Task<T[]> DeserializeAsync(string content)
	{
		if (string.IsNullOrWhiteSpace(content))
			return Task.FromResult(Array.Empty<T>());

		try
		{
			using var reader = new StringReader(content);
			var headerLine = reader.ReadLine();
			if (headerLine is null)
				return Task.FromResult(Array.Empty<T>());

			var headers = ParseLine(headerLine);
			var propMap = headers
				.Select((h, i) => (Index: i, Prop: _properties.FirstOrDefault(p =>
					string.Equals(p.Name, h, StringComparison.OrdinalIgnoreCase))))
				.Where(x => x.Prop is not null)
				.ToArray();

			var results = new List<T>();
			string? line;
			while ((line = reader.ReadLine()) is not null)
			{
				if (string.IsNullOrWhiteSpace(line)) continue;

				var values = ParseLine(line);
				var instance = Activator.CreateInstance<T>();
				foreach (var (index, prop) in propMap)
				{
					if (index >= values.Length || prop is null) continue;
					var converted = Convert.ChangeType(values[index], prop.PropertyType);
					prop.SetValue(instance, converted);
				}
				results.Add(instance);
			}

			return Task.FromResult(results.ToArray());
		}
		catch (Exception)
		{
			return Task.FromResult(Array.Empty<T>());
		}
	}

	private static string Escape(string? value)
	{
		if (value is null) return string.Empty;
		if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
			return $"\"{value.Replace("\"", "\"\"")}\"";
		return value;
	}

	private static string[] ParseLine(string line)
	{
		var fields = new List<string>();
		var sb = new StringBuilder();
		bool inQuotes = false;

		for (int i = 0; i < line.Length; i++)
		{
			char c = line[i];
			if (inQuotes)
			{
				if (c == '"')
				{
					if (i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; }
					else inQuotes = false;
				}
				else sb.Append(c);
			}
			else
			{
				if (c == '"') inQuotes = true;
				else if (c == ',') { fields.Add(sb.ToString()); sb.Clear(); }
				else sb.Append(c);
			}
		}
		fields.Add(sb.ToString());
		return fields.ToArray();
	}
}

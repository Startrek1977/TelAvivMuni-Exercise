namespace TelAvivMuni_Exercise.Core.Contracts;

/// <summary>
/// Defines custom column configurations for data browser dialogs.
/// </summary>
public interface IColumnConfiguration
{
	/// <summary>
	/// Gets a value indicating whether custom columns are defined.
	/// </summary>
	bool HasCustomColumns { get; }

	/// <summary>
	/// Gets the custom column definitions.
	/// </summary>
	IEnumerable<BrowserColumn>? Columns { get; }
}
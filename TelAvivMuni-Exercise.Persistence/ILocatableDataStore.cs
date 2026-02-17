namespace TelAvivMuni_Exercise.Persistence;

/// <summary>
/// Opt-in interface for data store implementations that expose their storage location.
/// File-based stores implement this to surface the file path; database stores do not.
/// </summary>
public interface ILocatableDataStore
{
	/// <summary>
	/// Gets the storage location (e.g., file path for file-based stores).
	/// </summary>
	string Location { get; }
}

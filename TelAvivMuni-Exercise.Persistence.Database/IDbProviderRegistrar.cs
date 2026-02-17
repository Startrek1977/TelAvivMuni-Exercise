using Microsoft.EntityFrameworkCore;

namespace TelAvivMuni_Exercise.Persistence.Database;

/// <summary>
/// Registers a specific EF Core database provider into the DbContextOptionsBuilder.
/// Implement this interface to add support for a new database provider.
/// </summary>
public interface IDbProviderRegistrar
{
	/// <summary>
	/// The provider name this registrar handles (case-insensitive match against StorageOptions.Provider).
	/// </summary>
	string ProviderName { get; }

	/// <summary>
	/// Configures the DbContextOptionsBuilder for this provider using the given connection string.
	/// </summary>
	void Configure(DbContextOptionsBuilder options, string connectionString);
}

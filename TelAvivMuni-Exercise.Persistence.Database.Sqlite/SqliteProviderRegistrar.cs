using Microsoft.EntityFrameworkCore;
using TelAvivMuni_Exercise.Persistence.Database;

namespace TelAvivMuni_Exercise.Persistence.Database.Sqlite;

/// <summary>
/// Registers SQLite as the EF Core database provider.
/// </summary>
public class SqliteProviderRegistrar : IDbProviderRegistrar
{
	/// <inheritdoc />
	public string ProviderName => "Sqlite";

	/// <inheritdoc />
	public void Configure(DbContextOptionsBuilder options, string connectionString)
		=> options.UseSqlite(connectionString);
}

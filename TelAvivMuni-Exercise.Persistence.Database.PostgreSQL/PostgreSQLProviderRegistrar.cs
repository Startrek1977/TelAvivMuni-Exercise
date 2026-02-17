using Microsoft.EntityFrameworkCore;
using TelAvivMuni_Exercise.Persistence.Database;

namespace TelAvivMuni_Exercise.Persistence.Database.PostgreSQL;

/// <summary>
/// Registers PostgreSQL (via Npgsql) as the EF Core database provider.
/// </summary>
public class PostgreSQLProviderRegistrar : IDbProviderRegistrar
{
	/// <inheritdoc />
	public string ProviderName => "PostgreSQL";

	/// <inheritdoc />
	public void Configure(DbContextOptionsBuilder options, string connectionString)
		=> options.UseNpgsql(connectionString);
}

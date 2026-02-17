using Microsoft.EntityFrameworkCore;
using TelAvivMuni_Exercise.Persistence.Database;

namespace TelAvivMuni_Exercise.Persistence.Database.SqlServer;

/// <summary>
/// Registers Microsoft SQL Server as the EF Core database provider.
/// </summary>
public class SqlServerProviderRegistrar : IDbProviderRegistrar
{
	/// <inheritdoc />
	public string ProviderName => "SqlServer";

	/// <inheritdoc />
	public void Configure(DbContextOptionsBuilder options, string connectionString)
		=> options.UseSqlServer(connectionString);
}

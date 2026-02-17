using Microsoft.EntityFrameworkCore;
using TelAvivMuni_Exercise.Persistence.Database;

namespace TelAvivMuni_Exercise.Persistence.Database.MySql;

/// <summary>
/// Registers MySQL (via Pomelo) as the EF Core database provider.
/// </summary>
public class MySqlProviderRegistrar : IDbProviderRegistrar
{
	/// <inheritdoc />
	public string ProviderName => "MySql";

	/// <inheritdoc />
	public void Configure(DbContextOptionsBuilder options, string connectionString)
	{
		var serverVersion = ServerVersion.AutoDetect(connectionString);
		options.UseMySql(connectionString, serverVersion);
	}
}

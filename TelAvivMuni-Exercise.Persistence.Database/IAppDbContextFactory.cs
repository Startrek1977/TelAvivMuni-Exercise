using Microsoft.EntityFrameworkCore;

namespace TelAvivMuni_Exercise.Persistence.Database;

/// <summary>
/// Non-generic factory abstraction for creating DbContext instances.
/// Decouples <see cref="DbDataStore{TEntity}"/> from the concrete DbContext type,
/// removing the need for the consumer to know about EF Core or any specific context class.
/// </summary>
public interface IAppDbContextFactory
{
	/// <summary>Creates and returns a new DbContext instance.</summary>
	Task<DbContext> CreateDbContextAsync(CancellationToken cancellationToken = default);
}

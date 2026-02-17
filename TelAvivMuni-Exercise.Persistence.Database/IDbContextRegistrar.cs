using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TelAvivMuni_Exercise.Persistence.Database;

/// <summary>
/// Registers a concrete DbContext and its <see cref="IAppDbContextFactory"/> adapter into DI.
/// Implement this interface in the project that owns the DbContext (e.g. Core).
/// Implementations are discovered at runtime via assembly scanning —
/// no compile-time reference to any concrete DbContext is required by the App.
/// </summary>
public interface IDbContextRegistrar
{
	/// <summary>
	/// Registers the DbContext factory into the service collection, applying the
	/// <paramref name="configure"/> callback to set up the chosen database provider.
	/// </summary>
	void RegisterDbContext(IServiceCollection services, Action<DbContextOptionsBuilder> configure);
}

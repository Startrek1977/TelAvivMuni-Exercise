using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TelAvivMuni_Exercise.Persistence.Database;

namespace TelAvivMuni_Exercise.Core;

/// <summary>
/// Registers <see cref="AppDbContext"/> and its <see cref="IAppDbContextFactory"/> adapter
/// into the DI container. Discovered at runtime by the App via assembly scanning —
/// the App has zero compile-time knowledge of <see cref="AppDbContext"/> or EF Core.
/// </summary>
public class AppDbContextRegistrar : IDbContextRegistrar
{
	/// <inheritdoc />
	public void RegisterDbContext(IServiceCollection services, Action<DbContextOptionsBuilder> configure)
	{
		services.AddDbContextFactory<AppDbContext>(configure);
		services.AddSingleton<IAppDbContextFactory>(sp =>
			new AppDbContextFactoryAdapter(sp.GetRequiredService<IDbContextFactory<AppDbContext>>()));
	}
}

/// <summary>
/// Adapts <see cref="IDbContextFactory{AppDbContext}"/> to the non-generic
/// <see cref="IAppDbContextFactory"/> interface consumed by <see cref="DbDataStore{TEntity}"/>.
/// </summary>
internal sealed class AppDbContextFactoryAdapter(IDbContextFactory<AppDbContext> inner) : IAppDbContextFactory
{
	public Task<DbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
		=> inner.CreateDbContextAsync(cancellationToken)
		        .ContinueWith(t => (DbContext)t.Result, cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
}

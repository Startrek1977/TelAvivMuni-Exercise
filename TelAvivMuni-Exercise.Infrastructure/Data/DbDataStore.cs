using Microsoft.EntityFrameworkCore;
using TelAvivMuni_Exercise.Core.Contracts;

namespace TelAvivMuni_Exercise.Infrastructure;

/// <summary>
/// SQL Server database implementation of IDataStore using Entity Framework Core.
/// Thread-safe for async operations.
/// </summary>
/// <typeparam name="TEntity">The type of entity to store.</typeparam>
/// <typeparam name="TContext">The type of DbContext to use.</typeparam>
public class DbDataStore<TEntity, TContext> : IDataStore<TEntity>
	where TEntity : class, IEntity
	where TContext : DbContext
{
	private readonly IDbContextFactory<TContext> _contextFactory;
	private readonly SemaphoreSlim _semaphore = new(1, 1);

	/// <summary>
	/// Initializes a new instance of the DbDataStore.
	/// </summary>
	/// <param name="contextFactory">The DbContext factory for creating database contexts.</param>
	/// <exception cref="ArgumentNullException">Thrown when contextFactory is null.</exception>
	public DbDataStore(IDbContextFactory<TContext> contextFactory)
	{
		_contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
	}

	/// <inheritdoc />
	public async Task<TEntity[]> LoadAsync()
	{
		await _semaphore.WaitAsync();
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();
			return await context.Set<TEntity>().ToArrayAsync();
		}
		finally
		{
			_semaphore.Release();
		}
	}

	/// <inheritdoc />
	public async Task<int> SaveAsync(IEnumerable<TEntity> entities)
	{
        ArgumentNullException.ThrowIfNull(entities);

        await _semaphore.WaitAsync();
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();
			var entityArray = entities as TEntity[] ?? entities.ToArray();

			// Clear existing entities (compatible with both SQL Server and InMemory providers)
			var existingEntities = await context.Set<TEntity>().ToListAsync();
			context.Set<TEntity>().RemoveRange(existingEntities);

			// Add new entities (matches FileDataStore overwrite behavior)
			context.Set<TEntity>().AddRange(entityArray);
			await context.SaveChangesAsync();

			return entityArray.Length;
		}
		finally
		{
			_semaphore.Release();
		}
	}
}

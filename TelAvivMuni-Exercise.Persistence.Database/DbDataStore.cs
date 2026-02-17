using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using TelAvivMuni_Exercise.Persistence;

namespace TelAvivMuni_Exercise.Persistence.Database;

/// <summary>
/// Database implementation of IDataStore using Entity Framework Core.
/// Thread-safe for async operations.
/// </summary>
/// <typeparam name="TEntity">The type of entity to store.</typeparam>
public class DbDataStore<TEntity>(IAppDbContextFactory contextFactory) : IDataStore<TEntity>
	where TEntity : class, IEntity
{
	private readonly IAppDbContextFactory _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
	private readonly SemaphoreSlim _semaphore = new(1, 1);

	private static string GetSqlErrorMessage(int errorNumber, string defaultMessage)
	{
		return errorNumber switch
		{
			-2 => "Database operation timed out. The server might be busy or unreachable.",
			1205 => "Database deadlock detected. Please retry the operation.",
			2601 or 2627 => "Cannot insert duplicate data. A unique constraint was violated.",
			547 => "Cannot complete operation due to foreign key constraint violation.",
			_ => defaultMessage
		};
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
		catch (DbException ex)
		{
			throw new InvalidOperationException(
				$"Database connection failed: {ex.Message}. Please check your connection string and ensure the database server is running.",
				ex);
		}
		catch (TimeoutException ex)
		{
			throw new InvalidOperationException(
				$"Database operation timed out: {ex.Message}. The server may be overloaded or unreachable.",
				ex);
		}
		catch (Exception ex) when (ex is not InvalidOperationException)
		{
			throw new InvalidOperationException(
				$"An unexpected error occurred while loading data: {ex.Message}",
				ex);
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
			var entityArray = entities as TEntity[] ?? [.. entities];

			// Clear existing entities
			var existing = await context.Set<TEntity>().ToListAsync();
			context.Set<TEntity>().RemoveRange(existing);

			// Add new entities (matches FileDataStore overwrite behavior)
			context.Set<TEntity>().AddRange(entityArray);
			await context.SaveChangesAsync();

			return entityArray.Length;
		}
		catch (DbException ex)
		{
			throw new InvalidOperationException(
				$"Database connection failed: {ex.Message}. Please check your connection string and ensure the database server is running.",
				ex);
		}
		catch (TimeoutException ex)
		{
			throw new InvalidOperationException(
				$"Database operation timed out: {ex.Message}. The server may be overloaded or unreachable.",
				ex);
		}
		catch (DbUpdateException ex)
		{
			throw new InvalidOperationException(
				$"Database update failed: {ex.Message}. This may be due to constraint violations or data integrity issues.",
				ex);
		}
		catch (Exception ex) when (ex is not InvalidOperationException and not ArgumentNullException)
		{
			throw new InvalidOperationException(
				$"An unexpected error occurred while saving data: {ex.Message}",
				ex);
		}
		finally
		{
			_semaphore.Release();
		}
	}
}

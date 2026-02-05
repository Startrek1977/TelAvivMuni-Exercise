using System.Data;
using Microsoft.Data.SqlClient;
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

	/// <summary>
	/// Maps SQL Server error codes to user-friendly error messages.
	/// </summary>
	/// <param name="errorNumber">The SQL Server error number.</param>
	/// <param name="defaultMessage">The default message to use if no specific mapping exists.</param>
	/// <returns>A user-friendly error message.</returns>
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
		catch (SqlException ex)
		{
			// Handle SQL Server specific errors (connection failures, timeouts, etc.)
			throw new InvalidOperationException(
				$"Database error occurred while loading data: {ex.Message}", ex);
		}
		catch (Exception ex) when (ex is not InvalidOperationException and not OperationCanceledException)
		{
			// Handle any other unexpected exceptions (network issues, etc.)
			throw new InvalidOperationException(
				$"Unexpected error occurred while loading data: {ex.Message}", ex);
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

			// Clear existing entities using a set-based delete for better performance
			await context.Set<TEntity>().ExecuteDeleteAsync();

			// Add new entities (matches FileDataStore overwrite behavior)
			context.Set<TEntity>().AddRange(entityArray);
			await context.SaveChangesAsync();

			return entityArray.Length;
		}
		catch (SqlException ex)
		{
			// Handle SQL Server specific errors including deadlocks and timeouts
			var errorMessage = GetSqlErrorMessage(ex.Number, 
				$"Database error occurred while saving data: {ex.Message}");
			
			throw new InvalidOperationException(errorMessage, ex);
		}
		catch (DbUpdateException ex)
		{
			// Handle constraint violations, unique key conflicts, foreign key violations
			// Check if the inner exception is a SqlException for more specific error messages
			if (ex.InnerException is SqlException sqlEx)
			{
				var errorMessage = GetSqlErrorMessage(sqlEx.Number,
					$"Database error occurred while saving data: {sqlEx.Message}");
				
				throw new InvalidOperationException(errorMessage, ex);
			}
			
			throw new InvalidOperationException(
				$"Database constraint violation or update error occurred while saving data: {ex.Message}", ex);
		}
		catch (Exception ex) when (ex is not InvalidOperationException and not ArgumentNullException and not OperationCanceledException)
		{
			// Handle any other unexpected exceptions (network issues, memory issues, etc.)
			throw new InvalidOperationException(
				$"Unexpected error occurred while saving data: {ex.Message}", ex);
		}
		finally
		{
			_semaphore.Release();
		}
	}
}

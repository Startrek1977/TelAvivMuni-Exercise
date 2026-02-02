namespace TelAvivMuni_Exercise.Core.Contracts.Patterns;

/// <summary>
/// Defines the contract for coordinating repositories and managing persistence.
/// </summary>
public interface IUnitOfWork : IDisposable
{
	/// <summary>
	/// Gets the repository for Product entities.
	/// </summary>
	IRepository<Product> Products { get; }

	/// <summary>
	/// Saves all pending changes to the underlying data store.
	/// </summary>
	/// <returns>The number of entities affected.</returns>
	Task<int> SaveChangesAsync();
}
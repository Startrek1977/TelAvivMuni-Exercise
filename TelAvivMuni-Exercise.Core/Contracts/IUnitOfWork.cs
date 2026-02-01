using System;
using System.Threading.Tasks;
using TelAvivMuni_Exercise.Models;
using TelAvivMuni_Exercise.Infrastructure;

namespace TelAvivMuni_Exercise.Core
{
    /// <summary>
    /// Coordinates work across multiple repositories and manages transaction boundaries.
    /// Ensures all repository changes are saved together or rolled back on failure.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the repository for Product entities.
        /// </summary>
        IRepository<Product> Products { get; }

        /// <summary>
        /// Saves all pending changes across all repositories.
        /// </summary>
        /// <returns>The number of entities saved.</returns>
        Task<int> SaveChangesAsync();
    }
}

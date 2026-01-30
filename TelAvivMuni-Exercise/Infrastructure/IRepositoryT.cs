using System.Collections.Generic;
using System.Threading.Tasks;

namespace TelAvivMuni_Exercise.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> Entities { get; }
        Task<TEntity[]> GetAllAsync();
        Task<TEntity?> GetByIdAsync(int id);
        Task<OperationResult> AddAsync(TEntity entity);
        Task<OperationResult> UpdateAsync(TEntity entity);
        Task<OperationResult> DeleteAsync(TEntity entity);

        /// <summary>
        /// Saves all changes to the data store.
        /// </summary>
        /// <returns>The number of entities saved.</returns>
        Task<int> SaveAsync();
    }
}

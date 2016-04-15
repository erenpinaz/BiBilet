using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BiBilet.Domain.Entities.Application;

namespace BiBilet.Domain.Repositories.Application
{
    /// <summary>
    /// Repository interface for <see cref="Category" />
    /// </summary>
    public interface ICategoryRepository : IRepository<Category>
    {
        /// <summary>
        /// Returns a list of categories including 
        /// published events
        /// </summary>
        /// <returns>A list of <see cref="Category" /></returns>
        List<Category> GetCategoriesWithEvents();

        /// <summary>
        /// Asynchronously returns a list of categories 
        /// including published events
        /// </summary>
        /// <returns>A list of <see cref="Category" /></returns>
        Task<List<Category>> GetCategoriesWithEventsAsync();

        /// <summary>
        /// Asynchronously returns a list of categories 
        /// including published events with cancellation 
        /// support
        /// </summary>
        /// <returns>A list of <see cref="Category" /></returns>
        Task<List<Category>> GetCategoriesWithEventsAsync(CancellationToken cancellationToken);
    }
}
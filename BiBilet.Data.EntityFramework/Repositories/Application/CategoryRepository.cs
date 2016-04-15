using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BiBilet.Domain.Entities.Application;
using BiBilet.Domain.Repositories.Application;

namespace BiBilet.Data.EntityFramework.Repositories.Application
{
    /// <summary>
    /// Entity framework implementation of <see cref="ICategoryRepository" />
    /// </summary>
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public CategoryRepository(BiBiletContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Returns a list of categories including 
        /// published events
        /// </summary>
        /// <returns>A list of <see cref="Category" /></returns>
        public List<Category> GetCategoriesWithEvents()
        {
            return Set
                .Include(c => c.Events.Select(e => e.Venue))
                .Include(c => c.Events.Select(e => e.Topic))
                .Include(c => c.Events.Select(e => e.SubTopic))
                .Where(c => c.Events.Count(e => e.Published) > 0)
                .ToList();
        }

        /// <summary>
        /// Asynchronously returns a list of categories 
        /// including published events
        /// </summary>
        /// <returns>A list of <see cref="Category" /></returns>
        public Task<List<Category>> GetCategoriesWithEventsAsync()
        {
            return Set
                .Include(c => c.Events.Select(e => e.Venue))
                .Include(c => c.Events.Select(e => e.Topic))
                .Include(c => c.Events.Select(e => e.SubTopic))
                .Where(c => c.Events.Count(e => e.Published) > 0)
                .ToListAsync();
        }

        /// <summary>
        /// Asynchronously returns a list of categories 
        /// including published events with cancellation 
        /// support
        /// </summary>
        /// <returns>A list of <see cref="Category" /></returns>
        public Task<List<Category>> GetCategoriesWithEventsAsync(CancellationToken cancellationToken)
        {
            return Set
                .Include(c => c.Events.Select(e => e.Venue))
                .Include(c => c.Events.Select(e => e.Topic))
                .Include(c => c.Events.Select(e => e.SubTopic))
                .Where(c => c.Events.Count(e => e.Published) > 0)
                .ToListAsync(cancellationToken);
        }
    }
}
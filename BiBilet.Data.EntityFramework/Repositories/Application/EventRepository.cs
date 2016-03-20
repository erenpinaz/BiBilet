using System;
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
    /// Entity framework implementation of <see cref="IEventRepository" />
    /// </summary>
    public class EventRepository : Repository<Event>, IEventRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public EventRepository(BiBiletContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Returns a list of published events
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of published <see cref="Event"/></returns>
        public virtual List<Event> GetEvents(Guid? userId)
        {
            var events = userId.HasValue ? Set.Where(e => e.Published && e.Organizer.UserId == userId) : Set.Where(e => e.Published);
            return events.ToList();
        }

        /// <summary>
        /// Asynchronously returns a list of published events
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of published <see cref="Event"/></returns>
        public virtual Task<List<Event>> GetEventsAsync(Guid? userId)
        {
            var events = userId.HasValue ? Set.Where(e => e.Published && e.Organizer.UserId == userId) : Set.Where(e => e.Published);
            return events.ToListAsync();
        }

        /// <summary>
        /// Asynchronously returns a list of published events
        /// with cancellation support
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="userId"></param>
        /// <returns>List of published <see cref="Event"/></returns>
        public virtual Task<List<Event>> GetEventsAsync(CancellationToken cancellationToken, Guid? userId)
        {
            var events = userId.HasValue ? Set.Where(e => e.Published && e.Organizer.UserId == userId) : Set.Where(e => e.Published);
            return events.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Returns single published event
        /// </summary>
        /// <param name="slug"></param>
        /// <returns>A published <see cref="Event"/></returns>
        public virtual Event GetEvent(string slug)
        {
            return Set.FirstOrDefault(e => e.Slug.Equals(slug) && e.Published);
        }

        /// <summary>
        /// Asynchronously returns single published event
        /// </summary>
        /// <param name="slug"></param>
        /// <returns>A published <see cref="Event"/></returns>
        public virtual Task<Event> GetEventAsync(string slug)
        {
            return Set.FirstOrDefaultAsync(e => e.Slug.Equals(slug) && e.Published);
        }

        /// <summary>
        /// Asynchronously returns single published event
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A published <see cref="Event"/></returns>
        public virtual Task<Event> GetEventAsync(string slug, CancellationToken cancellationToken)
        {
            return Set.FirstOrDefaultAsync(e => e.Slug.Equals(slug) && e.Published, cancellationToken);
        }
    }
}
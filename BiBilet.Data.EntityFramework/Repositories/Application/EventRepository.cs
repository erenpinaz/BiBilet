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
        /// <returns>List of published <see cref="Event" /></returns>
        public virtual List<Event> GetEvents()
        {
            return Set
                .Include(e => e.Venue)
                .Include(e => e.Topic)
                .Include(e => e.SubTopic)
                .Where(e => e.Published).ToList();
        }

        /// <summary>
        /// Asynchronously returns a list of published events
        /// </summary>
        /// <returns>List of published <see cref="Event" /></returns>
        public virtual Task<List<Event>> GetEventsAsync()
        {
            return Set
                .Include(e => e.Venue)
                .Include(e => e.Topic)
                .Include(e => e.SubTopic)
                .Where(e => e.Published).ToListAsync();
        }

        /// <summary>
        /// Asynchronously returns a list of published events
        /// with cancellation support
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>List of published <see cref="Event" /></returns>
        public virtual Task<List<Event>> GetEventsAsync(CancellationToken cancellationToken)
        {
            return Set
                .Include(e => e.Venue)
                .Include(e => e.Topic)
                .Include(e => e.SubTopic)
                .Where(e => e.Published).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Returns single published event
        /// </summary>
        /// <param name="slug"></param>
        /// <returns>A published <see cref="Event" /></returns>
        public virtual Event GetEvent(string slug)
        {
            return Set
                .Include(e => e.Organizer)
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .Include(e => e.Topic)
                .Include(e => e.SubTopic)
                .Include(e => e.Tickets.Select(t => t.UserTickets))
                .FirstOrDefault(e => e.Slug.Equals(slug) && e.Published);
        }

        /// <summary>
        /// Asynchronously returns single published event
        /// </summary>
        /// <param name="slug"></param>
        /// <returns>A published <see cref="Event" /></returns>
        public virtual Task<Event> GetEventAsync(string slug)
        {
            return Set
                .Include(e => e.Organizer)
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .Include(e => e.Topic)
                .Include(e => e.SubTopic)
                .Include(e => e.Tickets.Select(t => t.UserTickets))
                .FirstOrDefaultAsync(e => e.Slug.Equals(slug) && e.Published);
        }

        /// <summary>
        /// Asynchronously returns single published event
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A published <see cref="Event" /></returns>
        public virtual Task<Event> GetEventAsync(string slug, CancellationToken cancellationToken)
        {
            return Set
                .Include(e => e.Organizer)
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .Include(e => e.Topic)
                .Include(e => e.SubTopic)
                .Include(e => e.Tickets.Select(t => t.UserTickets))
                .FirstOrDefaultAsync(e => e.Slug.Equals(slug) && e.Published, cancellationToken);
        }

        /// <summary>
        /// Returns single user event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns>An <see cref="Event" /></returns>
        public virtual Event GetUserEvent(Guid id, Guid userId)
        {
            return Set
                .Include(e => e.Organizer)
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .Include(e => e.Topic)
                .Include(e => e.SubTopic)
                .Include(e => e.Tickets.Select(t => t.UserTickets))
                .FirstOrDefault(e => e.EventId == id && e.Organizer.UserId == userId);
        }

        /// <summary>
        /// Asynchronously returns single user event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns>An <see cref="Event" /></returns>
        public virtual Task<Event> GetUserEventAsync(Guid id, Guid userId)
        {
            return Set
                .Include(e => e.Organizer)
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .Include(e => e.Topic)
                .Include(e => e.SubTopic)
                .Include(e => e.Tickets.Select(t => t.UserTickets))
                .FirstOrDefaultAsync(e => e.EventId == id && e.Organizer.UserId == userId);
        }

        /// <summary>
        /// Asynchronously returns single user event
        /// with cancellation support
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>An <see cref="Event" /></returns>
        public virtual Task<Event> GetUserEventAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            return Set
                .Include(e => e.Organizer)
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .Include(e => e.Topic)
                .Include(e => e.SubTopic)
                .Include(e => e.Tickets.Select(t => t.UserTickets))
                .FirstOrDefaultAsync(e => e.EventId == id && e.Organizer.UserId == userId, cancellationToken);
        }
    }
}
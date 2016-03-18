using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BiBilet.Domain.Entities.Application;

namespace BiBilet.Domain.Repositories.Application
{
    /// <summary>
    /// Repository interface for <see cref="Event"/>
    /// </summary>
    public interface IEventRepository : IRepository<Event>
    {
        /// <summary>
        /// Returns a list of published events
        /// </summary>
        /// <returns>List of published <see cref="Event"/></returns>
        List<Event> GetEvents();

        /// <summary>
        /// Asynchronously returns a list of published events
        /// </summary>
        /// <returns>List of published <see cref="Event"/></returns>
        Task<List<Event>> GetEventsAsync();

        /// <summary>
        /// Asynchronously returns a list of published events
        /// with cancellation support
        /// </summary>
        /// <returns>List of published <see cref="Event"/></returns>
        Task<List<Event>> GetEventsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns single published event
        /// </summary>
        /// <param name="slug"></param>
        /// <returns>A published <see cref="Event"/></returns>
        Event GetEvent(string slug);

        /// <summary>
        /// Asynchronously returns single published event
        /// </summary>
        /// <param name="slug"></param>
        /// <returns>A published <see cref="Event"/></returns>
        Task<Event> GetEventAsync(string slug);

        /// <summary>
        /// Asynchronously returns single published event
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A published <see cref="Event"/></returns>
        Task<Event> GetEventAsync(string slug, CancellationToken cancellationToken);
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BiBilet.Domain.Entities.Application;

namespace BiBilet.Domain.Repositories.Application
{
    /// <summary>
    /// Repository interface for <see cref="Organizer"/>
    /// </summary>
    public interface IOrganizerRepository : IRepository<Organizer>
    {
        /// <summary>
        /// Returns a list of organizer
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>A <see cref="Organizer" /></returns>
        List<Organizer> GetUserOrganizers(Guid userId);

        /// <summary>
        /// Asynchronously returns a list of organizer
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>A list of <see cref="Organizer" /></returns>
        Task<List<Organizer>> GetUserOrganizersAsync(Guid userId);

        /// <summary>
        /// Asynchronously returns a list of organizer
        /// with cancellation support
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of <see cref="Organizer" /></returns>
        Task<List<Organizer>> GetUserOrganizersAsync(Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Returns an organizer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns>An <see cref="Organizer" /></returns>
        Organizer GetUserOrganizer(Guid id, Guid userId);

        /// <summary>
        /// Asynchronously returns an organizer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns>An <see cref="Organizer" /></returns>
        Task<Organizer> GetUserOrganizerAsync(Guid id, Guid userId);

        /// <summary>
        /// Asynchronously returns an organizer
        /// with cancellation support
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>An <see cref="Organizer" /></returns>
        Task<Organizer> GetUserOrganizerAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    }
}
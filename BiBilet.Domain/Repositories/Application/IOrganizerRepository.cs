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
        List<Organizer> GetUserOrganizers(Guid userId);
        Task<List<Organizer>> GetUserOrganizersAsync(Guid userId);
        Task<List<Organizer>> GetUserOrganizersAsync(Guid userId, CancellationToken cancellationToken);

        Organizer GetUserOrganizer(Guid id, Guid getGuid);
        Task<Organizer> GetUserOrganizerAsync(Guid id, Guid getGuid);
        Task<Organizer> GetUserOrganizerAsync(Guid id, Guid getGuid, CancellationToken cancellationToken);
    }
}
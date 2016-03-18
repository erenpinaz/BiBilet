using System;
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
        Organizer GetOrganizerByUserId(Guid id);
        Task<Organizer> GetOrganizerByUserIdAsync(Guid id);
        Task<Organizer> GetOrganizerByUserIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
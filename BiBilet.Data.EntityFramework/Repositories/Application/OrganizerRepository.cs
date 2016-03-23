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
    /// Entity framework implementation of <see cref="IOrganizerRepository" />
    /// </summary>
    public class OrganizerRepository : Repository<Organizer>, IOrganizerRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public OrganizerRepository(BiBiletContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Returns a list of organizer
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>A list of <see cref="Organizer" /></returns>
        public List<Organizer> GetUserOrganizers(Guid userId)
        {
            return Set.Where(o => o.UserId.Equals(userId)).ToList();
        }

        /// <summary>
        /// Asynchronously returns a list of organizer
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>A list of <see cref="Organizer" /></returns>
        public Task<List<Organizer>> GetUserOrganizersAsync(Guid userId)
        {
            return Set.Where(o => o.UserId.Equals(userId)).ToListAsync();
        }

        /// <summary>
        /// Asynchronously returns a list of organizer
        /// with cancellation support
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of <see cref="Organizer" /></returns>
        public Task<List<Organizer>> GetUserOrganizersAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Set.Where(o => o.UserId.Equals(userId)).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Returns an organizer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns>An <see cref="Organizer" /></returns>
        public Organizer GetUserOrganizer(Guid id, Guid userId)
        {
            return Set.FirstOrDefault(o => o.OrganizerId.Equals(id) && o.UserId.Equals(userId));
        }

        /// <summary>
        /// Asynchronously returns an organizer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns>An <see cref="Organizer" /></returns>
        public Task<Organizer> GetUserOrganizerAsync(Guid id, Guid userId)
        {
            return Set.FirstOrDefaultAsync(o => o.OrganizerId.Equals(id) && o.UserId.Equals(userId));
        }

        /// <summary>
        /// Asynchronously returns an organizer
        /// with cancellation support
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>An <see cref="Organizer" /></returns>
        public Task<Organizer> GetUserOrganizerAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            return Set.FirstOrDefaultAsync(o => o.OrganizerId.Equals(id) && o.UserId.Equals(userId), cancellationToken);
        }
    }
}
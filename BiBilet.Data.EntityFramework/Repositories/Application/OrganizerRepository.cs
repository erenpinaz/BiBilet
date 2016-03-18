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

        public List<Organizer> GetUserOrganizers(Guid userId)
        {
            return Set.Where(o => o.UserId.Equals(userId)).ToList();
        }

        public Task<List<Organizer>> GetUserOrganizersAsync(Guid userId)
        {
            return Set.Where(o => o.UserId.Equals(userId)).ToListAsync();
        }

        public Task<List<Organizer>> GetUserOrganizersAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Set.Where(o => o.UserId.Equals(userId)).ToListAsync(cancellationToken);
        }

        public Organizer GetUserOrganizer(Guid id, Guid userId)
        {
            return Set.FirstOrDefault(o => o.OrganizerId.Equals(id) && o.UserId.Equals(userId));
        }

        public Task<Organizer> GetUserOrganizerAsync(Guid id, Guid userId)
        {
            return Set.FirstOrDefaultAsync(o => o.OrganizerId.Equals(id) && o.UserId.Equals(userId));

        }

        public Task<Organizer> GetUserOrganizerAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            return Set.FirstOrDefaultAsync(o => o.OrganizerId.Equals(id) && o.UserId.Equals(userId), cancellationToken);
        }
    }
}
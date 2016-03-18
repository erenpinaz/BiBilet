using System;
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

        //TODO: Add organizer list methods & get organizer by id method

        public Organizer GetOrganizerByUserId(Guid id)
        {
            return Set.FirstOrDefault(o => o.UserId.Equals(id));
        }

        public Task<Organizer> GetOrganizerByUserIdAsync(Guid id)
        {
            return Set.FirstOrDefaultAsync(o => o.UserId.Equals(id));
        }

        public Task<Organizer> GetOrganizerByUserIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Set.FirstOrDefaultAsync(o => o.UserId.Equals(id), cancellationToken);
        }
    }
}
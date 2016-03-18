using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BiBilet.Domain.Entities.Identity;
using BiBilet.Domain.Repositories.Identity;

namespace BiBilet.Data.EntityFramework.Repositories.Identity
{
    /// <summary>
    /// Entity framework implementation of <see cref="IRoleRepository" />
    /// </summary>
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public RoleRepository(BiBiletContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Returns role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>A <see cref="Role" /></returns>
        public Role FindByName(string roleName)
        {
            return Set.FirstOrDefault(x => x.Name == roleName);
        }

        /// <summary>
        /// Asynchronously returns role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>A <see cref="Role" /></returns>
        public Task<Role> FindByNameAsync(string roleName)
        {
            return Set.FirstOrDefaultAsync(x => x.Name == roleName);
        }

        /// <summary>
        /// Asynchronously returns role with cancellation support
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="roleName"></param>
        /// <returns>A <see cref="Role" /></returns>
        public Task<Role> FindByNameAsync(CancellationToken cancellationToken, string roleName)
        {
            return Set.FirstOrDefaultAsync(x => x.Name == roleName, cancellationToken);
        }
    }
}
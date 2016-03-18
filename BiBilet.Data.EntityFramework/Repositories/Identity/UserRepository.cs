using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BiBilet.Domain.Entities.Application;
using BiBilet.Domain.Entities.Identity;
using BiBilet.Domain.Repositories.Identity;

namespace BiBilet.Data.EntityFramework.Repositories.Identity
{
    /// <summary>
    /// Entity framework implementation of <see cref="IUserRepository" />
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public UserRepository(BiBiletContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Returns user
        /// </summary>
        /// <param name="username"></param>
        /// <returns>A <see cref="User" /></returns>
        public User FindByUserName(string username)
        {
            return Set.FirstOrDefault(x => x.UserName == username);
        }

        /// <summary>
        /// Asynchronously returns user
        /// </summary>
        /// <param name="username"></param>
        /// <returns>A <see cref="User" /></returns>
        public Task<User> FindByUserNameAsync(string username)
        {
            return Set.FirstOrDefaultAsync(x => x.UserName == username);
        }

        /// <summary>
        /// Asynchronously returns user with cancellation support
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="username"></param>
        /// <returns>A <see cref="User" /></returns>
        public Task<User> FindByUserNameAsync(CancellationToken cancellationToken, string username)
        {
            return Set.FirstOrDefaultAsync(x => x.UserName == username, cancellationToken);
        }

        /// <summary>
        /// Returns user
        /// </summary>
        /// <param name="email"></param>
        /// <returns>A <see cref="User" /></returns>
        public User FindByEmail(string email)
        {
            return Set.FirstOrDefault(x => x.Email == email);
        }

        /// <summary>
        /// Asynchronously returns user
        /// </summary>
        /// <param name="email"></param>
        /// <returns>A <see cref="User" /></returns>
        public Task<User> FindByEmailAsync(string email)
        {
            return Set.FirstOrDefaultAsync(x => x.Email == email);
        }

        /// <summary>
        /// Asynchronously returns user 
        /// with cancellation support
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A <see cref="User" /></returns>
        public Task<User> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return Set.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }
    }
}
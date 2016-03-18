using System.Threading;
using System.Threading.Tasks;
using BiBilet.Domain.Entities.Identity;

namespace BiBilet.Domain.Repositories.Identity
{
    /// <summary>
    /// Repository interface for <see cref="User"/>
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Returns user
        /// </summary>
        /// <param name="username"></param>
        /// <returns>A <see cref="User" /></returns>
        User FindByUserName(string username);

        /// <summary>
        /// Asynchronously returns user
        /// </summary>
        /// <param name="username"></param>
        /// <returns>A <see cref="User" /></returns>
        Task<User> FindByUserNameAsync(string username);

        /// <summary>
        /// Asynchronously returns user 
        /// with cancellation support
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="username"></param>
        /// <returns>A <see cref="User" /></returns>
        Task<User> FindByUserNameAsync(CancellationToken cancellationToken, string username);

        /// <summary>
        /// Returns user
        /// </summary>
        /// <param name="email"></param>
        /// <returns>A <see cref="User" /></returns>
        User FindByEmail(string email);

        /// <summary>
        /// Asynchronously returns user
        /// </summary>
        /// <param name="email"></param>
        /// <returns>A <see cref="User" /></returns>
        Task<User> FindByEmailAsync(string email);

        /// <summary>
        /// Asynchronously returns user 
        /// with cancellation support
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A <see cref="User" /></returns>
        Task<User> FindByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
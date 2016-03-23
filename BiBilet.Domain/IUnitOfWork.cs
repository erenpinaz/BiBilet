using System;
using System.Threading;
using System.Threading.Tasks;
using BiBilet.Domain.Entities.Application;
using BiBilet.Domain.Repositories;
using BiBilet.Domain.Repositories.Application;
using BiBilet.Domain.Repositories.Identity;

namespace BiBilet.Domain
{
    /// <summary>
    /// Unit of work interface
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        #region Properties

        // Identity
        IExternalLoginRepository ExternalLoginRepository { get; }
        IRoleRepository RoleRepository { get; }
        IUserRepository UserRepository { get; }

        // Application
        IOrganizerRepository OrganizerRepository { get; }
        IEventRepository EventRepository { get; }
        IRepository<Category> CategoryRepository { get; }
        IRepository<Topic> TopicRepository { get; }
        ISubTopicRepository SubTopicRepository { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Saves changes that are made in the current context
        /// </summary>
        /// <returns>Number of rows affected as an <see cref="int" /></returns>
        int SaveChanges();

        /// <summary>
        /// Asynchronously saves changes that are made in the current context
        /// </summary>
        /// <returns>Number of rows affected as an <see cref="int" /></returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Asynchronously saves changes that are made in the current context
        /// with cancellation support
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Number of rows affected as an <see cref="int" /></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        #endregion
    }
}
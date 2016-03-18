using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BiBilet.Data.EntityFramework.Repositories.Application;
using BiBilet.Data.EntityFramework.Repositories.Identity;
using BiBilet.Domain;
using BiBilet.Domain.Repositories.Application;
using BiBilet.Domain.Repositories.Identity;

namespace BiBilet.Data.EntityFramework
{
    /// <summary>
    /// Entity framework implementation of <see cref="IUnitOfWork" />
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nameOrConnectionString"></param>
        public UnitOfWork(string nameOrConnectionString)
        {
            _context = new BiBiletContext(nameOrConnectionString);
        }

        #endregion

        #region IDisposable Members

        private bool _disposed = false;

        /// <summary>
        /// Disposes all the resources used by the <see cref="UnitOfWork" />
        /// This will be called by DI containers lifetime manager
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Identity
                    _userRepository = null;
                    _roleRepository = null;
                    _externalLoginRepository = null;

                    // Application
                    _eventRepository = null;
                    _categoryRepository = null;
                    _organizerRepository = null;

                    // Database Context
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Fields

        // Database Context
        private readonly BiBiletContext _context;

        // Identity
        private IExternalLoginRepository _externalLoginRepository;
        private IRoleRepository _roleRepository;
        private IUserRepository _userRepository;

        // Application
        private IEventRepository _eventRepository;
        private ICategoryRepository _categoryRepository;
        private IOrganizerRepository _organizerRepository;

        #endregion

        #region IUnitOfWork Members

        // Identity
        public IExternalLoginRepository ExternalLoginRepository
            => _externalLoginRepository ?? (_externalLoginRepository = new ExternalLoginRepository(_context));

        public IRoleRepository RoleRepository => _roleRepository ?? (_roleRepository = new RoleRepository(_context));

        public IUserRepository UserRepository => _userRepository ?? (_userRepository = new UserRepository(_context));

        // Application
        public IEventRepository EventRepository
            => _eventRepository ?? (_eventRepository = new EventRepository(_context));

        public ICategoryRepository CategoryRepository
            => _categoryRepository ?? (_categoryRepository = new CategoryRepository(_context));

        public IOrganizerRepository OrganizerRepository
            => _organizerRepository ?? (_organizerRepository = new OrganizerRepository(_context));

        /// <summary>
        /// Saves changes that are made in the current context
        /// </summary>
        /// <returns>Number of rows affected as an <see cref="int" /></returns>
        public int SaveChanges()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw FormatException(dbEx);
            }
        }

        /// <summary>
        /// Asynchronously saves changes that are made in the current context
        /// </summary>
        /// <returns>Number of rows affected as an <see cref="int" /></returns>
        public Task<int> SaveChangesAsync()
        {
            try
            {
                return _context.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw FormatException(dbEx);
            }
        }

        /// <summary>
        /// Asynchronously saves changes that are made in the current context
        /// with cancellation support
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Number of rows affected as an <see cref="int" /></returns>
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                return _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbEntityValidationException dbEx)
            {
                throw FormatException(dbEx);
            }
        }

        /// <summary>
        /// Formats DbEntityValidationException
        /// </summary>
        /// <param name="dbEx"></param>
        /// <returns></returns>
        private static DbEntityValidationException FormatException(DbEntityValidationException dbEx)
        {
            // Retrieve the error messages as a list of strings
            var errorMessages = dbEx.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

            // Join the list to a single string
            var fullErrorMessage = string.Join("; ", errorMessages);

            // Combine the original exception message with the new one
            var exceptionMessage = string.Concat(dbEx.Message, " The validation errors are: ", fullErrorMessage);

            // Throw a new DbEntityValidationException with the improved exception message
            return new DbEntityValidationException(exceptionMessage, dbEx.EntityValidationErrors);
        }

        #endregion
    }
}
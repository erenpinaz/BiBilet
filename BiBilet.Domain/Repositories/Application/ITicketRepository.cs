using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BiBilet.Domain.Entities.Application;

namespace BiBilet.Domain.Repositories.Application
{
    /// <summary>
    /// Repository interface for <see cref="Ticket"/>
    /// </summary>
    public interface ITicketRepository : IRepository<Ticket>
    {
        /// <summary>
        /// Returns a single ticket
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A single <see cref="Ticket"/></returns>
        Ticket GetTicket(Guid id);

        /// <summary>
        /// Asynchronously returns a single ticket
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A single <see cref="Ticket"/></returns>
        Task<Ticket> GetTicketAsync(Guid id);

        /// <summary>
        /// Asynchronously returns a single ticket
        /// with cancellation support
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A single <see cref="Ticket"/></returns>
        Task<Ticket> GetTicketAsync(Guid id, CancellationToken cancellationToken);
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BiBilet.Domain.Entities.Application;

namespace BiBilet.Domain.Repositories.Application
{
    /// <summary>
    /// Repository interface for <see cref="UserTicket" />
    /// </summary>
    public interface IUserTicketRepository : IRepository<UserTicket>
    {
        /// <summary>
        /// Returns a list of user tickets
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>A list of <see cref="UserTicket" /></returns>
        List<UserTicket> GetUserTickets(Guid userId);

        /// <summary>
        /// Asynchronously returns a list of user tickets
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>A list of <see cref="UserTicket" /></returns>
        Task<List<UserTicket>> GetUserTicketsAsync(Guid userId);

        /// <summary>
        /// Asynchronously returns a list of user tickets
        /// with cancellation support
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of <see cref="UserTicket" /></returns>
        Task<List<UserTicket>> GetUserTicketsAsync(Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Returns a single user ticket
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns>A single <see cref="UserTicket" /></returns>
        UserTicket GetUserTicke(string orderNumber);

        /// <summary>
        /// Asynchronously returns a single user ticket
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns>A single <see cref="UserTicket" /></returns>
        Task<UserTicket> GetUserTicketAsync(string orderNumber);

        /// <summary>
        /// Asynchronously returns a single user ticket
        /// with cancellation support
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A single <see cref="UserTicket" /></returns>
        Task<UserTicket> GetUserTicketAsync(string orderNumber, CancellationToken cancellationToken);

        /// <summary>
        /// Returns a list of ticket owners
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns>A list of <see cref="UserTicket" /></returns>
        List<UserTicket> GetAttendees(Guid eventId);

        /// <summary>
        /// Asynchronously returns a list of ticket owners
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns>A list of <see cref="UserTicket" /></returns>
        Task<List<UserTicket>> GetAttendeesAsync(Guid eventId);

        /// <summary>
        /// Asynchronously returns a list of ticket owners
        /// with cancellation support
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of <see cref="UserTicket" /></returns>
        Task<List<UserTicket>> GetAttendeesAsync(Guid eventId, CancellationToken cancellationToken);
    }
}
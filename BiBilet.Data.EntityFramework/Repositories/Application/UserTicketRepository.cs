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
    /// Entity framework implementation of <see cref="IUserTicketRepository" />
    /// </summary>
    public class UserTicketRepository : Repository<UserTicket>, IUserTicketRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public UserTicketRepository(BiBiletContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Returns a list of user tickets
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>A list of <see cref="UserTicket"/></returns>
        public List<UserTicket> GetUserTickets(Guid userId)
        {
            return Set
                .Include(ut => ut.Ticket.Event)
                .Include(ut => ut.Ticket)
                .Where(ut => ut.UserId == userId).ToList();
        }

        /// <summary>
        /// Asynchronously returns a list of user tickets
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>A list of <see cref="UserTicket"/></returns>
        public Task<List<UserTicket>> GetUserTicketsAsync(Guid userId)
        {
            return Set
                .Include(ut => ut.Ticket.Event)
                .Include(ut => ut.Ticket)
                .Where(ut => ut.UserId == userId).ToListAsync();
        }

        /// <summary>
        /// Asynchronously returns a list of user tickets
        /// with cancellation support
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of <see cref="UserTicket"/></returns>
        public Task<List<UserTicket>> GetUserTicketsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Set
                .Include(ut => ut.Ticket.Event)
                .Include(ut => ut.Ticket)
                .Where(ut => ut.UserId == userId).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Returns a single user ticket
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns>A single <see cref="UserTicket"/></returns>
        public UserTicket GetUserTicke(string orderNumber)
        {
            return Set
                .Include(ut => ut.Ticket.Event)
                .Include(ut => ut.Ticket)
                .FirstOrDefault(ut => ut.OrderNumber == orderNumber);
        }

        /// <summary>
        /// Asynchronously returns a single user ticket
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns>A single <see cref="UserTicket"/></returns>
        public Task<UserTicket> GetUserTicketAsync(string orderNumber)
        {
            return Set
                .Include(ut => ut.Ticket.Event)
                .Include(ut => ut.Ticket)
                .FirstOrDefaultAsync(ut => ut.OrderNumber == orderNumber);
        }

        /// <summary>
        /// Asynchronously returns a single user ticket
        /// with cancellation support
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A single <see cref="UserTicket"/></returns>
        public Task<UserTicket> GetUserTicketAsync(string orderNumber, CancellationToken cancellationToken)
        {
            return Set
                .Include(ut => ut.Ticket.Event)
                .Include(ut => ut.Ticket)
                .FirstOrDefaultAsync(ut => ut.OrderNumber == orderNumber, cancellationToken);
        }

        /// <summary>
        /// Returns a list of ticket owners
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns>A list of <see cref="UserTicket"/></returns>
        public List<UserTicket> GetAttendees(Guid eventId)
        {
            return Set.Where(ut => ut.Ticket.EventId == eventId).ToList();
        }

        /// <summary>
        /// Asynchronously returns a list of ticket owners
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns>A list of <see cref="UserTicket"/></returns>
        public Task<List<UserTicket>> GetAttendeesAsync(Guid eventId)
        {
            return Set.Where(ut => ut.Ticket.EventId == eventId).ToListAsync();
        }

        /// <summary>
        /// Asynchronously returns a list of ticket owners
        /// with cancellation support
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of <see cref="UserTicket"/></returns>
        public Task<List<UserTicket>> GetAttendeesAsync(Guid eventId, CancellationToken cancellationToken)
        {
            return Set.Where(ut => ut.Ticket.EventId == eventId).ToListAsync(cancellationToken);
        }
    }
}
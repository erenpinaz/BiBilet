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
    /// Entity framework implementation of <see cref="ITicketRepository" />
    /// </summary>
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public TicketRepository(BiBiletContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Returns a single ticket
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A single <see cref="Ticket" /></returns>
        public Ticket GetTicket(Guid id)
        {
            return Set.FirstOrDefault(t => t.TicketId == id);
        }

        /// <summary>
        /// Asynchronously returns a single ticket
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A single <see cref="Ticket" /></returns>
        public Task<Ticket> GetTicketAsync(Guid id)
        {
            return Set
                .Include(t => t.UserTickets)
                .FirstOrDefaultAsync(t => t.TicketId == id);
        }

        /// <summary>
        /// Asynchronously returns a single ticket
        /// with cancellation support
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A single <see cref="Ticket" /></returns>
        public Task<Ticket> GetTicketAsync(Guid id, CancellationToken cancellationToken)
        {
            return Set
                .Include(t => t.UserTickets)
                .FirstOrDefaultAsync(t => t.TicketId == id, cancellationToken);
        }
    }
}
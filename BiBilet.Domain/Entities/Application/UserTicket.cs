using System;
using BiBilet.Domain.Entities.Identity;

namespace BiBilet.Domain.Entities.Application
{
    public class UserTicket
    {
        #region Scalar Properties

        public Guid UserId { get; set; }
        public Guid TicketId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerAddress { get; set; }

        #endregion

        #region Navigation Properties

        public virtual User User { get; set; }

        public virtual Ticket Ticket { get; set; }

        #endregion
    }
}
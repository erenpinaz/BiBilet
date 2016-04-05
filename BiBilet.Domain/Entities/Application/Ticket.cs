using System;
using System.Collections.Generic;

namespace BiBilet.Domain.Entities.Application
{
    public class Ticket
    {
        #region Fields

        private ICollection<UserTicket> _userTickets;

        #endregion

        #region Scalar Properties

        public Guid TicketId { get; set; }
        public Guid EventId { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public TicketType Type { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Event Event { get; set; }

        public virtual ICollection<UserTicket> UserTickets
        {
            get { return _userTickets ?? (_userTickets = new List<UserTicket>()); }
            set { _userTickets = value; }
        }

        #endregion
    }
}
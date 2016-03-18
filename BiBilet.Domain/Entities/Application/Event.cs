using System;
using System.Collections.Generic;

namespace BiBilet.Domain.Entities.Application
{
    public class Event
    {
        #region Fields

        private ICollection<Ticket> _tickets;

        #endregion

        #region Scalar Properties

        public Guid EventId { get; set; }
        public Guid OrganizerId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid TopicId { get; set; }
        public Guid SubTopicId { get; set; }
        public Guid VenueId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Slug { get; set; }
        public bool Published { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Organizer Organizer { get; set; }

        public virtual Category Category { get; set; }

        public virtual Topic Topic { get; set; }

        public virtual SubTopic SubTopic { get; set; }

        public virtual Venue Venue { get; set; }

        public virtual ICollection<Ticket> Tickets
        {
            get { return _tickets ?? (_tickets = new List<Ticket>()); }
            set { _tickets = value; }
        }

        #endregion
    }
}
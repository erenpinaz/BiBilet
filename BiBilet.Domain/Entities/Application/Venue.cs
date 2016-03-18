using System;
using System.Collections.Generic;

namespace BiBilet.Domain.Entities.Application
{
    public class Venue
    {
        #region Fields

        private ICollection<Event> _events;

        #endregion

        #region Scalar Properties

        public Guid VenueId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        #endregion

        #region Navigation Properties

        public virtual ICollection<Event> Events
        {
            get { return _events ?? (_events = new List<Event>()); }
            set { _events = value; }
        }

        #endregion
    }
}
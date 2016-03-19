using System;
using System.Collections.Generic;
using BiBilet.Domain.Entities.Identity;

namespace BiBilet.Domain.Entities.Application
{
    public class Organizer
    {
        #region Fields

        private ICollection<Event> _events;

        #endregion

        #region Scalar Properties

        public Guid OrganizerId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public string Image { get; set; }
        public string Slug { get; set; }
        public bool IsDefault { get; set; }

        #endregion

        #region Navigation Properties

        public virtual ICollection<Event> Events
        {
            get { return _events ?? (_events = new List<Event>()); }
            set { _events = value; }
        }

        public virtual User User { get; set; }

        #endregion
    }
}
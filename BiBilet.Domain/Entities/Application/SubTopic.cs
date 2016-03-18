using System;
using System.Collections.Generic;

namespace BiBilet.Domain.Entities.Application
{
    public class SubTopic
    {
        #region Fields

        private ICollection<Event> _events;

        #endregion

        #region Scalar Properties

        public Guid SubTopicId { get; set; }
        public Guid TopicId { get; set; }
        public string Name { get; set; }

        #endregion

        #region Navigation Properties

        public virtual ICollection<Event> Events
        {
            get { return _events ?? (_events = new List<Event>()); }
            set { _events = value; }
        }

        public virtual Topic Topic { get; set; }

        #endregion
    }
}
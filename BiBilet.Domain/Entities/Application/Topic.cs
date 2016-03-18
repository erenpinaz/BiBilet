using System;
using System.Collections.Generic;

namespace BiBilet.Domain.Entities.Application
{
    public class Topic
    {
        #region Fields

        private ICollection<Event> _events;
        private ICollection<SubTopic> _subTopics;

        #endregion

        #region Scalar Properties

        public Guid TopicId { get; set; }
        public string Name { get; set; }

        #endregion

        #region Navigation Properties

        public virtual ICollection<Event> Events
        {
            get { return _events ?? (_events = new List<Event>()); }
            set { _events = value; }
        }

        public virtual ICollection<SubTopic> SubTopics
        {
            get { return _subTopics ?? (_subTopics = new List<SubTopic>()); }
            set { _subTopics = value; }
        }

        #endregion
    }
}
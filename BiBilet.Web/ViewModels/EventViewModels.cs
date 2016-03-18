using System;
using System.Collections.Generic;

namespace BiBilet.Web.ViewModels
{
    public class EventViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Slug { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string OrganizerName { get; set; }
        public string OrganizerSlug { get; set; }

        public string VenueName { get; set; }
        public string VenueAddress { get; set; }

        public string CategoryName { get; set; }
        public string CategorySlug { get; set; }

        public string TopicName { get; set; }
        public Guid TopicId { get; set; }
        public string SubTopicName { get; set; }
        public Guid SubTopicId { get; set; }

        public List<TicketViewModel> Tickets { get; set; }
    }

    public class TicketViewModel
    {
        public Guid TicketId { get; set; }

        public string Title { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
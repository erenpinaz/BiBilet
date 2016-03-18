using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using BiBilet.Domain;
using BiBilet.Web.ViewModels;

namespace BiBilet.Web.Controllers
{
    public class EventController : BaseController
    {
        public EventController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        [HttpGet]
        public async Task<ActionResult> Details(string slug)
        {
            var eventObj = await UnitOfWork.EventRepository.GetEventAsync(slug);
            if (eventObj == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(new EventViewModel()
            {
                Title = eventObj.Title,
                Description = eventObj.Description,
                Image = eventObj.Image,
                Slug = eventObj.Slug,
                StartDate = eventObj.StartDate,
                EndDate = eventObj.EndDate,
                OrganizerName = eventObj.Organizer.Name,
                OrganizerSlug = eventObj.Organizer.Slug,
                VenueName = eventObj.Venue.Name,
                VenueAddress =
                    string.Format("{0}, {1}/{2}", eventObj.Venue.Address, eventObj.Venue.City, eventObj.Venue.Country),
                CategoryName = eventObj.Category.Name,
                CategorySlug = eventObj.Category.Slug,
                TopicName = eventObj.Topic.Name,
                TopicId = eventObj.Topic.TopicId,
                SubTopicName = eventObj.SubTopic.Name,
                SubTopicId = eventObj.SubTopic.SubTopicId,
                Tickets = eventObj.Tickets.Select(t => new TicketViewModel()
                {
                    TicketId = t.TicketId,
                    Title = t.Title,
                    Quantity = t.Quantity,
                    Price = t.Price
                }).ToList()
            });
        }
    }
}
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using BiBilet.Domain;
using BiBilet.Web.ViewModels;
using Microsoft.AspNet.Identity;

namespace BiBilet.Web.Controllers
{
    public class EventController : BaseController
    {
        public EventController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        [Authorize]
        [HttpGet]
        public ActionResult MyEvents()
        {
            return View();
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

        [HttpGet]
        public async Task<ActionResult> PopulateUserEvents()
        {
            if (!Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var events = await UnitOfWork.EventRepository.GetEventsAsync(GetGuid(User.Identity.GetUserId()));
            var json = events.OrderBy(e => e.StartDate).Select(e => new
            {
                eventid = e.EventId,
                title = e.Title,
                organizer = e.Organizer.Name,
                category = e.Category.Name,
                topic = string.Format("{0} / {1}", e.Topic.Name, e.SubTopic.Name),
                status = e.Published,
                startdate = e.StartDate,
                enddate = e.EndDate
            });

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        #region Helpers

        /// <summary>
        /// Converts string to Guid
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Guid GetGuid(string value)
        {
            Guid result;
            Guid.TryParse(value, out result);
            return result;
        }

        #endregion
    }
}
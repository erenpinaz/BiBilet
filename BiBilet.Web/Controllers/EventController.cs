using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using BiBilet.Domain;
using BiBilet.Web.ViewModels;
using Microsoft.AspNet.Identity;
using BiBilet.Domain.Entities.Application;
using BiBilet.Web.Utils;

namespace BiBilet.Web.Controllers
{
    [Authorize]
    public class EventController : BaseController
    {
        private const string UploadPath = "/assets/uploads/events/";
        private const string PlaceholderImagePath = "/assets/images/placeholder.png";

        public EventController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        [HttpGet]
        public ActionResult MyEvents()
        {
            return View();
        }

        [AllowAnonymous]
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
                Date = eventObj.StartDate.Date != eventObj.EndDate.Date
                    ? string.Format("{0} - {1}", eventObj.StartDate.ToLongDateString(),
                        eventObj.EndDate.ToLongDateString())
                    : eventObj.StartDate.ToLongDateString(),
                Time =
                    string.Format("{0} - {1}", eventObj.StartDate.ToShortTimeString(),
                        eventObj.EndDate.ToShortTimeString()),
                OrganizerName = eventObj.Organizer.Name,
                VenueName = eventObj.Venue.Name,
                VenueAddress =
                    string.Format("{0} {1}, {2}", eventObj.Venue.Address, eventObj.Venue.City, eventObj.Venue.Country),
                TopicName = eventObj.Topic.Name,
                SubTopicName = eventObj.SubTopic.Name,
                IsLive = eventObj.StartDate >= DateTime.UtcNow,
                Tickets = eventObj.Tickets.Select(t => new TicketViewModel()
                {
                    TicketId = t.TicketId,
                    Title = t.Title,
                    Quantity = t.Quantity,
                    Price = t.Price,
                    Type = t.Type
                }).ToList()
            });
        }

        [HttpGet]
        public async Task<ActionResult> UpdateEvent(Guid id, string message)
        {
            var eventObj = await UnitOfWork.EventRepository.GetUserEventAsync(id, GetGuid(User.Identity.GetUserId()));
            if (eventObj == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            ViewBag.StatusMessage = message;

            return View(new EventEditModel()
            {
                Organizers =
                    new SelectList(await UnitOfWork.OrganizerRepository.GetAllAsync(), "OrganizerId", "Name",
                        eventObj.OrganizerId),
                Categories =
                    new SelectList(await UnitOfWork.CategoryRepository.GetAllAsync(), "CategoryId", "Name",
                        eventObj.CategoryId),
                Topics =
                    new SelectList(await UnitOfWork.TopicRepository.GetAllAsync(), "TopicId", "Name", eventObj.TopicId),
                SubTopics =
                    new SelectList(await UnitOfWork.SubTopicRepository.GetSubTopicsAsync(eventObj.TopicId), "SubTopicId",
                        "Name", eventObj.SubTopicId),
                EventId = eventObj.EventId,
                OrganizerId = eventObj.Organizer.OrganizerId,
                VenueId = eventObj.Venue.VenueId,
                CategoryId = eventObj.Category.CategoryId,
                TopicId = eventObj.Topic.TopicId,
                SubTopicId = eventObj.SubTopic.SubTopicId,
                Title = eventObj.Title,
                Description = eventObj.Description,
                Image = eventObj.Image,
                Slug = eventObj.Slug,
                Published = eventObj.Published,
                StartDate = eventObj.StartDate,
                EndDate = eventObj.EndDate,
                VenueName = eventObj.Venue.Name,
                VenueAddress = eventObj.Venue.Address,
                VenueCity = eventObj.Venue.City,
                VenueCountry = eventObj.Venue.Country,
                Tickets = eventObj.Tickets.Select(t => new TicketViewModel()
                {
                    TicketId = t.TicketId,
                    Title = t.Title,
                    Quantity = t.Quantity,
                    Price = t.Price,
                    Type = t.Type
                }).ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateEvent(Guid id, EventEditModel model)
        {
            var eventObj = await UnitOfWork.EventRepository.GetUserEventAsync(id, GetGuid(User.Identity.GetUserId()));
            if (eventObj == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Tickets.Any())
                    {
                        if (await IsEventSlugUnique(model.Slug, eventObj.EventId))
                        {
                            eventObj.OrganizerId = model.OrganizerId;
                            eventObj.CategoryId = model.CategoryId;
                            eventObj.TopicId = model.TopicId;
                            eventObj.SubTopicId = model.SubTopicId;

                            eventObj.Title = model.Title;
                            eventObj.Description = model.Description;
                            eventObj.Image = model.Image ?? PlaceholderImagePath;
                            eventObj.Slug = model.Slug;
                            eventObj.Published = model.Published;
                            eventObj.StartDate = model.StartDate;
                            eventObj.EndDate = model.EndDate;

                            foreach (var ticketVm in model.Tickets)
                            {
                                if (ticketVm.TicketId == Guid.Empty)
                                {
                                    UnitOfWork.TicketRepository.Add(new Ticket()
                                    {
                                        TicketId = Guid.NewGuid(),
                                        EventId = eventObj.EventId,
                                        Title = ticketVm.Title,
                                        Quantity = ticketVm.Quantity,
                                        Price = ticketVm.Price,
                                        Type = ticketVm.Price == decimal.Zero ? TicketType.Free : TicketType.Paid
                                    });
                                }
                                else
                                {
                                    var ticket =
                                        eventObj.Tickets.FirstOrDefault(t => t.TicketId == ticketVm.TicketId);
                                    if (ticket != null)
                                    {
                                        ticket.EventId = eventObj.EventId;
                                        ticket.Title = ticketVm.Title;
                                        ticket.Quantity = ticketVm.Quantity;
                                        ticket.Price = ticketVm.Price;
                                        ticket.Type = ticketVm.Price == decimal.Zero ? TicketType.Free : TicketType.Paid;

                                        UnitOfWork.TicketRepository.Update(ticket);
                                    }
                                }
                            }

                            UnitOfWork.EventRepository.Update(eventObj);
                            await UnitOfWork.SaveChangesAsync();

                            return RedirectToAction("UpdateEvent", "Event",
                                new {id = eventObj.EventId, message = "Etkinlik başarıyla güncellendi"});
                        }
                        else
                        {
                            ModelState.AddModelError("slug", "Url kısaltması özel olmalıdır");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Etkinliğe en az bir bilet eklenmelidir");
                    }
                }
                catch
                {
                    //TODO: Log error
                    ModelState.AddModelError("", "Etkinlik güncellenirken bir hata oluştu");
                }
            }
            else
            {
                ModelState.AddModelError("", "Lütfen tüm alanları doğru şekilde doldurunuz");
            }

            model.Organizers = new SelectList(await UnitOfWork.OrganizerRepository.GetAllAsync(), "OrganizerId", "Name",
                eventObj.OrganizerId);
            model.Categories = new SelectList(await UnitOfWork.CategoryRepository.GetAllAsync(), "CategoryId", "Name",
                eventObj.CategoryId);
            model.Topics = new SelectList(await UnitOfWork.TopicRepository.GetAllAsync(), "TopicId", "Name",
                eventObj.TopicId);
            model.SubTopics = new SelectList(await UnitOfWork.SubTopicRepository.GetSubTopicsAsync(eventObj.TopicId),
                "SubTopicId", "Name", eventObj.SubTopicId);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> UploadEventImage(Guid id)
        {
            var eventObj = await UnitOfWork.EventRepository.GetUserEventAsync(id, GetGuid(User.Identity.GetUserId()));
            if (eventObj == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            try
            {
                var file = Request.Files[0];
                if (file != null)
                {
                    const string fileName = "event-image.jpg";

                    var absolutePath = Server.MapPath(Path.Combine(UploadPath, id.ToString()));

                    if (!Directory.Exists(absolutePath))
                    {
                        Directory.CreateDirectory(absolutePath);
                    }

                    DiskUtils.SaveImage(file.InputStream, Path.Combine(absolutePath, fileName));

                    return Json(new
                    {
                        success = true,
                        path =
                            VirtualPathUtility.Combine(UploadPath,
                                string.Format("{0}/{1}?{2}", id, fileName, DateTime.UtcNow.ToBinary()))
                    });
                }

                return Json(new {success = false});
            }
            catch
            {
                //TODO: Log error
                return Json(new {success = false});
            }
        }

        [HttpGet]
        public ActionResult AddTicketItem(string type)
        {
            return PartialView("_TicketItem", new TicketViewModel()
            {
                Type = type == "free" ? TicketType.Free : TicketType.Paid
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

        [OutputCache(Duration = 3600, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "topicId")]
        [HttpGet]
        public async Task<ActionResult> PopulateSubTopics(Guid topicId)
        {
            if (!Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var subTopics = await UnitOfWork.SubTopicRepository.GetSubTopicsAsync(topicId);
            var json = subTopics.Select(s => new
            {
                subtopicid = s.SubTopicId,
                name = s.Name
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

        /// <summary>
        /// Checks if event slug is unique
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        private async Task<bool> IsEventSlugUnique(string slug, Guid eventId)
        {
            var events = await UnitOfWork.EventRepository.GetAllAsync();
            var eventObj = events.FirstOrDefault(e => e.EventId == eventId);
            if (eventObj != null && eventObj.Slug == slug)
            {
                return events.Count(e => e.Slug == slug) <= 1;
            }
            return events.Count(e => e.Slug == slug) == 0;
        }

        #endregion
    }
}
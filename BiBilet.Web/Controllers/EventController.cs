using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using BiBilet.Domain;
using BiBilet.Domain.Entities.Application;
using BiBilet.Web.Utils;
using BiBilet.Web.ViewModels;
using Microsoft.AspNet.Identity;
using PagedList;

namespace BiBilet.Web.Controllers
{
    [Authorize]
    public class EventController : BaseController
    {
        private const int SearchPageSize = 10;

        private const string UploadPath = "/assets/uploads/events/";
        private const string PlaceholderImagePath = "/assets/images/event-placeholder.png";

        public EventController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        [HttpGet]
        public ActionResult MyEvents()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SearchEvent(string q, string c, int page = 1)
        {
            var categories = await UnitOfWork.CategoryRepository.GetCategoriesWithEventsAsync();

            var events = c != null
                ? categories
                    .Where(cat => cat.Slug == c)
                    .SelectMany(
                        cat =>
                            cat.Events.Where(
                                e =>
                                    e.StartDate > DateTime.UtcNow &&
                                    e.Title.IndexOf(q ?? string.Empty, StringComparison.InvariantCultureIgnoreCase) >=
                                    0)).ToList()
                : categories
                    .SelectMany(
                        cat =>
                            cat.Events.Where(
                                e =>
                                    e.StartDate > DateTime.UtcNow &&
                                    e.Title.IndexOf(q ?? string.Empty, StringComparison.InvariantCultureIgnoreCase) >=
                                    0)).ToList();

            var pagedEvents = events
                .OrderBy(e => e.StartDate)
                .ToPagedList(page, SearchPageSize);

            return View(new SearchEventViewModel
            {
                Categories = categories,
                Events = pagedEvents
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Details(string slug)
        {
            var eventObj = await UnitOfWork.EventRepository.GetEventAsync(slug);
            if (eventObj == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(new EventViewModel
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
                    string.Format("{0} {1} / {2}", eventObj.Venue.Address, eventObj.Venue.City, eventObj.Venue.Country),
                TopicName = eventObj.Topic.Name,
                SubTopicName = eventObj.SubTopic.Name,
                IsLive = eventObj.StartDate >= DateTime.UtcNow,
                Tickets = eventObj.Tickets.Select(t => new TicketViewModel
                {
                    TicketId = t.TicketId,
                    Title = t.Title,
                    Quantity = t.Quantity - t.UserTickets.Count == 0
                        ? "Tükenmiş"
                        : (t.Quantity - t.UserTickets.Count).ToString(),
                    Price = t.Price.ToString(CultureInfo.DefaultThreadCurrentCulture),
                    Type = t.Type == TicketType.Free ? "Bedava" : "Ücretli",
                    IsPaid = t.Type == TicketType.Paid && t.Price > 0,
                    IsAvailable = t.Quantity - t.UserTickets.Count > 0
                }).ToList()
            });
        }

        [HttpGet]
        public async Task<ActionResult> CreateEvent()
        {
            var organizers = await UnitOfWork.OrganizerRepository.GetAllAsync();
            var categories = await UnitOfWork.CategoryRepository.GetAllAsync();
            var topics = await UnitOfWork.TopicRepository.GetAllAsync();
            var subTopics = await UnitOfWork.SubTopicRepository.GetSubTopicsAsync(topics[0].TopicId);

            return View(new EventEditModel
            {
                Organizers =
                    new SelectList(organizers, "OrganizerId", "Name"),
                Categories =
                    new SelectList(categories, "CategoryId", "Name"),
                Topics =
                    new SelectList(topics, "TopicId", "Name"),
                SubTopics =
                    new SelectList(subTopics, "SubTopicId",
                        "Name"),
                Image = PlaceholderImagePath,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEvent(EventEditModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Tickets.Any())
                    {
                        if (await IsEventSlugUnique(model.Slug, model.EventId))
                        {
                            var eventObj = new Event
                            {
                                OrganizerId = model.OrganizerId,
                                CategoryId = model.CategoryId,
                                TopicId = model.TopicId,
                                SubTopicId = model.SubTopicId,
                                EventId = Guid.NewGuid(),
                                Title = model.Title,
                                Description = model.Description,
                                Image = model.Image ?? PlaceholderImagePath,
                                Slug = model.Slug,
                                Published = model.Published,
                                StartDate = model.StartDate,
                                EndDate = model.EndDate,
                                Venue = new Venue
                                {
                                    VenueId = Guid.NewGuid(),
                                    Name = model.VenueName,
                                    Address = model.VenueAddress,
                                    City = model.VenueCity,
                                    Country = model.VenueCountry
                                }
                            };

                            UnitOfWork.EventRepository.Add(eventObj);

                            foreach (var ticketVm in model.Tickets)
                            {
                                eventObj.Tickets.Add(new Ticket
                                {
                                    TicketId = Guid.NewGuid(),
                                    EventId = eventObj.EventId,
                                    Title = ticketVm.Title,
                                    Quantity = ticketVm.Quantity,
                                    Price = ticketVm.Price,
                                    Type = ticketVm.Price == decimal.Zero ? TicketType.Free : TicketType.Paid
                                });
                            }

                            await UnitOfWork.SaveChangesAsync();

                            return RedirectToAction("MyEvents", "Event");
                        }
                        ModelState.AddModelError("", "Url kısaltması özel olmalıdır");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Etkinliğe en az bir bilet eklenmelidir");
                    }
                }
                catch
                {
                    //TODO: Log error
                    ModelState.AddModelError("", "Etkinlik oluşturulurken bir hata oluştu");
                }
            }
            else
            {
                ModelState.AddModelError("", "Lütfen tüm alanları doğru şekilde doldurunuz");
            }

            model.Organizers = new SelectList(await UnitOfWork.OrganizerRepository.GetAllAsync(), "OrganizerId", "Name",
                model.OrganizerId);
            model.Categories = new SelectList(await UnitOfWork.CategoryRepository.GetAllAsync(), "CategoryId", "Name",
                model.CategoryId);
            model.Topics = new SelectList(await UnitOfWork.TopicRepository.GetAllAsync(), "TopicId", "Name",
                model.TopicId);
            model.SubTopics = new SelectList(await UnitOfWork.SubTopicRepository.GetSubTopicsAsync(model.TopicId),
                "SubTopicId", "Name", model.SubTopicId);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> UpdateEvent(Guid? id, string message)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var eventObj =
                await UnitOfWork.EventRepository.GetUserEventAsync(id.Value, GetGuid(User.Identity.GetUserId()));
            if (eventObj == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            ViewBag.StatusMessage = message;

            return View(new EventEditModel
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
                Tickets = eventObj.Tickets.Select(t => new TicketEditModel
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
        public async Task<ActionResult> UpdateEvent(EventEditModel model)
        {
            var eventObj =
                await UnitOfWork.EventRepository.GetUserEventAsync(model.EventId, GetGuid(User.Identity.GetUserId()));
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

                            eventObj.Venue.Name = model.VenueName;
                            eventObj.Venue.Address = model.VenueAddress;
                            eventObj.Venue.City = model.VenueCity;
                            eventObj.Venue.Country = model.VenueCountry;

                            foreach (var ticketVm in model.Tickets)
                            {
                                if (ticketVm.TicketId == Guid.Empty)
                                {
                                    eventObj.Tickets.Add(new Ticket
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
                                    }
                                }
                            }

                            UnitOfWork.EventRepository.Update(eventObj);
                            await UnitOfWork.SaveChangesAsync();

                            return RedirectToAction("UpdateEvent", "Event",
                                new {id = eventObj.EventId, message = "Etkinlik başarıyla güncellendi"});
                        }
                        ModelState.AddModelError("", "Url kısaltması özel olmalıdır");
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

        [HttpGet]
        public async Task<ActionResult> DeleteEvent(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var eventObj =
                await UnitOfWork.EventRepository.GetUserEventAsync(id.Value, GetGuid(User.Identity.GetUserId()));
            if (eventObj == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(new EventEditModel
            {
                EventId = eventObj.EventId,
                Title = eventObj.Title
            });
        }

        [HttpPost, ActionName("DeleteEvent")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteEvent(Guid id)
        {
            var eventObj =
                await UnitOfWork.EventRepository.GetUserEventAsync(id, GetGuid(User.Identity.GetUserId()));
            if (eventObj == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            try
            {
                foreach (var ticket in eventObj.Tickets.ToList())
                {
                    UnitOfWork.TicketRepository.Remove(ticket);
                }

                UnitOfWork.EventRepository.Remove(eventObj);
                await UnitOfWork.SaveChangesAsync();

                if (eventObj.Image != PlaceholderImagePath)
                {
                    FileUtils.DeleteFile(Server.MapPath(eventObj.Image));
                }

                return RedirectToAction("MyEvents", "Event");
            }
            catch
            {
                //TODO: Log error
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ManageEvent(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var eventObj =
                await UnitOfWork.EventRepository.GetUserEventAsync(id.Value, GetGuid(User.Identity.GetUserId()));
            if (eventObj == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(new ManageEventViewModel
            {
                EventId = eventObj.EventId,
                Title = eventObj.Title,
                TotalTickets = eventObj.Tickets.Sum(t => t.Quantity),
                TotalPaidTickets = eventObj.Tickets.Where(t => t.Type == TicketType.Paid).Sum(t => t.Quantity),
                TotalFreeTickets = eventObj.Tickets.Where(t => t.Type == TicketType.Free).Sum(t => t.Quantity),
                TotalSold = eventObj.Tickets.SelectMany(t => t.UserTickets).Count(),
                TotalPaidSold =
                    eventObj.Tickets.Where(t => t.Type == TicketType.Paid).SelectMany(t => t.UserTickets).Count(),
                TotalFreeSold =
                    eventObj.Tickets.Where(t => t.Type == TicketType.Free).SelectMany(t => t.UserTickets).Count()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadEventImage(int x, int y, int w, int h)
        {
            try
            {
                var file = Request.Files[0];
                if (file != null)
                {
                    var absolutePath = Server.MapPath(Path.Combine(UploadPath));
                    if (!Directory.Exists(absolutePath))
                    {
                        Directory.CreateDirectory(absolutePath);
                    }

                    var fileName = FileUtils.GenerateFileName("e", "jpg");
                    FileUtils.CropSaveImage(file.InputStream, x, y, w, h, Path.Combine(absolutePath, fileName));

                    return Json(new
                    {
                        success = true,
                        path = Path.Combine(UploadPath, fileName)
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
            return PartialView("_TicketItem", new TicketEditModel
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

            var events = await UnitOfWork.EventRepository.GetAllAsync();
            var json = events.Where(e => e.Organizer.UserId == GetGuid(User.Identity.GetUserId()))
                .OrderBy(e => e.StartDate).Select(e => new
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

        [HttpGet]
        [OutputCache(Duration = 3600, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "topicId")]
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

        [HttpGet]
        public async Task<ActionResult> PopulateAttendees(Guid? id)
        {
            if (!Request.IsAjaxRequest() || id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var attendees = await UnitOfWork.UserTicketRepository.GetAttendeesAsync(id.Value);
            var json = attendees.Select(ut => new
            {
                name = ut.OwnerName,
                email = ut.OwnerEmail,
                address = ut.OwnerAddress,
                tickettitle = ut.Ticket.Title,
                tickettype = ut.Ticket.Type == TicketType.Free ? "Bedava" : "Ücretli",
                ordernumber = ut.OrderNumber,
                orderdate = ut.OrderDate
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
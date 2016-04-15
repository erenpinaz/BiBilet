using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using BiBilet.Domain;
using BiBilet.Domain.Entities.Application;
using BiBilet.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Rotativa;

namespace BiBilet.Web.Controllers
{
    [Authorize]
    public class TicketController : BaseController
    {
        public TicketController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        [HttpGet]
        public async Task<ActionResult> MyTickets()
        {
            var userTickets =
                await UnitOfWork.UserTicketRepository.GetUserTicketsAsync(GetGuid(User.Identity.GetUserId()));

            return View(userTickets.Select(ut => new UserTicketViewModel
            {
                Name = ut.OwnerName,
                Email = ut.OwnerEmail,
                Address = ut.OwnerAddress,
                EventTitle = ut.Ticket.Event.Title,
                EventStartDate = ut.Ticket.Event.StartDate.ToString("f"),
                EventImage = ut.Ticket.Event.Image,
                TicketTitle = ut.Ticket.Title,
                TicketType = ut.Ticket.Type == TicketType.Free ? "Bedava" : "Ücretsiz",
                OrderNumber = ut.OrderNumber,
                OrderDate = ut.OrderDate.ToString("f")
            }));
        }

        [HttpGet]
        public async Task<ActionResult> Register(Guid? id)
        {
            if (!Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ticket = await UnitOfWork.TicketRepository.GetTicketAsync(id.Value);
            if (ticket == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            if (ticket.Quantity - ticket.UserTickets.Count == 0 || ticket.Type != TicketType.Free)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return PartialView("_RegisterTicket", new RegisterTicketEditModel
            {
                TicketId = ticket.TicketId,
                EventTitle = ticket.Event.Title,
                TicketTitle = ticket.Title
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterTicketEditModel model)
        {
            try
            {
                var ticket = await UnitOfWork.TicketRepository.GetTicketAsync(model.TicketId);
                if (ticket == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                if (ticket.Quantity - ticket.UserTickets.Count == 0 || ticket.Type != TicketType.Free)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                var userTicket = new UserTicket
                {
                    TicketId = model.TicketId,
                    UserId = GetGuid(User.Identity.GetUserId()),
                    OwnerName = string.Format("{0} {1}", model.FirstName, model.LastName),
                    OwnerEmail = model.Email,
                    OwnerAddress = model.Address,
                    OrderNumber = Guid.NewGuid().ToString("N"),
                    OrderDate = DateTime.UtcNow
                };

                UnitOfWork.UserTicketRepository.Add(userTicket);
                await UnitOfWork.SaveChangesAsync();
            }
            catch
            {
                //TODO: Log error
            }

            return RedirectToAction("MyTickets", "Ticket");
        }

        [HttpGet]
        public async Task<ActionResult> PrintTicket(string orderNumber)
        {
            var userTicket = await UnitOfWork.UserTicketRepository.GetUserTicketAsync(orderNumber);
            if (userTicket == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return new ViewAsPdf(new UserTicketViewModel
            {
                Name = userTicket.OwnerName,
                Email = userTicket.OwnerEmail,
                Address = userTicket.OwnerAddress,
                EventTitle = userTicket.Ticket.Event.Title,
                EventAddress = string.Format("{0} {1} / {2}",
                    userTicket.Ticket.Event.Venue.Address, userTicket.Ticket.Event.Venue.City,
                    userTicket.Ticket.Event.Venue.Country),
                EventStartDate = userTicket.Ticket.Event.StartDate.ToString("f"),
                EventImage = userTicket.Ticket.Event.Image,
                TicketTitle = userTicket.Ticket.Title,
                TicketType = userTicket.Ticket.Type == TicketType.Free ? "Bedava" : "Ücretsiz",
                OrderNumber = userTicket.OrderNumber,
                OrderDate = userTicket.OrderDate.ToString("f")
            });
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
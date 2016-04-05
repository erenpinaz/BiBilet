using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BiBilet.Domain;
using BiBilet.Web.ViewModels;

namespace BiBilet.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var categories = await UnitOfWork.CategoryRepository.GetCategoriesWithEventsAsync();
            var events = categories.SelectMany(c => c.Events).ToList();

            var model = new HomeViewModel
            {
                HotEvents =
                    events.Where(e => e.EndDate >= DateTime.UtcNow)
                        .OrderBy(e => e.StartDate)
                        .Take(4)
                        .ToList(),
                UpcomingEvents =
                    events.Where(e => e.EndDate >= DateTime.UtcNow)
                        .OrderBy(e => e.StartDate)
                        .Skip(4)
                        .Take(20)
                        .ToList(),
                PastEvents =
                    events.Where(e => e.EndDate < DateTime.UtcNow)
                        .OrderByDescending(e => e.EndDate)
                        .Take(20)
                        .ToList(),
                Categories = categories.OrderByDescending(c => c.Events.Count).Take(8).ToList()
            };

            return View(model);
        }
    }
}
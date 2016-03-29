using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BiBilet.Domain;
using BiBilet.Domain.Entities.Application;
using BiBilet.Web.Identity;
using BiBilet.Web.Utils;
using BiBilet.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace BiBilet.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private const string UploadPath = "/assets/uploads/organizers/";
        private const string PlaceholderImagePath = "/assets/images/placeholder.png";

        private readonly UserManager<IdentityUser, Guid> _userManager;

        public AccountController(UserManager<IdentityUser, Guid> userManager, IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            userManager.UserValidator = new UserValidator<IdentityUser, Guid>(userManager)
            {
                RequireUniqueEmail = true,
                AllowOnlyAlphanumericUserNames = true
            };

            userManager.PasswordValidator = new PasswordValidator
            {
                RequireDigit = true,
                RequireNonLetterOrDigit = true,
                RequireLowercase = true,
                RequiredLength = 6
            };

            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginEditModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await _userManager.UpdateSecurityStampAsync(user.Id);

                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                ModelState.AddModelError("", "Hatalı kullanıcı adı veya şifre girdiniz");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterEditModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser {UserName = model.UserName, Name = model.Name, Email = model.Email};

                var organizer = new Organizer
                {
                    OrganizerId = Guid.NewGuid(),
                    Name = user.Name,
                    Description = string.Format("{0} kullanıcısının organizatör profili.", user.Name),
                    Image = PlaceholderImagePath,
                    IsDefault = true
                };
                organizer.Slug = string.Format("o-{0}", organizer.OrganizerId.ToString("N"));

                user.Organizers.Add(organizer);

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Settings(string message)
        {
            var user = await _userManager.FindByIdAsync(GetGuid(User.Identity.GetUserId()));
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var model = new SettingsEditModel
            {
                UserName = user.UserName,
                Name = user.Name,
                Email = user.Email
            };

            ViewBag.StatusMessage = message;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Settings(SettingsEditModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(GetGuid(User.Identity.GetUserId()));
                if (user == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                if (await _userManager.CheckPasswordAsync(user, model.OldPassword))
                {
                    user.Name = model.Name;
                    user.Email = model.Email;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        if (model.NewPassword != null && model.ConfirmPassword != null)
                        {
                            result =
                                await
                                    _userManager.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);
                            if (result.Succeeded)
                            {
                                AuthenticationManager.SignOut();
                                return RedirectToAction("Login", "Account");
                            }
                            AddErrors(result);

                            return View(model);
                        }

                        return RedirectToAction("Settings", "Account",
                            new {message = "Kullanıcı başarıyla güncellendi"});
                    }
                    AddErrors(result);
                }
                else
                {
                    ModelState.AddModelError("OldPassword", "Hatalı şifre girdiniz");
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> CreateOrganizer()
        {
            var organizer = new Organizer
            {
                OrganizerId = Guid.NewGuid(),
                Name = "İsimsiz organizatör",
                Image = PlaceholderImagePath,
                IsDefault = false,
                UserId = GetGuid(User.Identity.GetUserId())
            };
            organizer.Slug = string.Format("o-{0}", organizer.OrganizerId.ToString("N"));

            try
            {
                UnitOfWork.OrganizerRepository.Add(organizer);
                await UnitOfWork.SaveChangesAsync();
            }
            catch
            {
                //TODO: Log error
            }

            return RedirectToAction("UpdateOrganizer", "Account", new {id = organizer.OrganizerId});
        }

        [HttpGet]
        public async Task<ActionResult> UpdateOrganizer(Guid? id, string message)
        {
            var organizers =
                await UnitOfWork.OrganizerRepository.GetUserOrganizersAsync(GetGuid(User.Identity.GetUserId()));

            var organizer = id.HasValue
                ? organizers.FirstOrDefault(o => o.OrganizerId.Equals(id.Value))
                : organizers.First();

            if (organizer == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var model = new OrganizerEditModel
            {
                OrganizerId = organizer.OrganizerId,
                Name = organizer.Name,
                Description = organizer.Description,
                Image = organizer.Image,
                Website = organizer.Website,
                Slug = organizer.Slug,
                IsRemovable = !organizer.IsDefault
            };

            ViewBag.StatusMessage = message;
            ViewBag.Organizers = organizers;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateOrganizer(OrganizerEditModel model)
        {
            var organizers =
                await UnitOfWork.OrganizerRepository.GetUserOrganizersAsync(GetGuid(User.Identity.GetUserId()));

            if (ModelState.IsValid)
            {
                var organizer = organizers.FirstOrDefault(o => o.OrganizerId.Equals(model.OrganizerId));
                if (organizer == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                try
                {
                    organizer.Name = model.Name;
                    organizer.Description = model.Description;
                    organizer.Image = model.Image ?? PlaceholderImagePath;
                    organizer.Website = model.Website;
                    organizer.Slug = model.Slug;

                    if (await IsOrganizerSlugUnique(model.Slug, organizer.OrganizerId))
                    {
                        UnitOfWork.OrganizerRepository.Update(organizer);
                        await UnitOfWork.SaveChangesAsync();

                        return RedirectToAction("UpdateOrganizer", "Account",
                            new {id = organizer.OrganizerId, message = "Profil başarıyla güncellendi"});
                    }
                    ModelState.AddModelError("", "Url kısaltması özel olmalıdır");
                }
                catch
                {
                    //TODO: Log error
                    ModelState.AddModelError("", "Organizatör profili güncellenirken bir hata oluştu");
                }
            }
            else
            {
                ModelState.AddModelError("", "Lütfen tüm alanları doğru şekilde doldurunuz");
            }

            ViewBag.Organizers = organizers;

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteOrganizer(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var organizer =
                await UnitOfWork.OrganizerRepository.GetUserOrganizerAsync(id.Value, GetGuid(User.Identity.GetUserId()));
            if (organizer == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            if (organizer.IsDefault)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(new OrganizerEditModel
            {
                OrganizerId = organizer.OrganizerId,
                Name = organizer.Name,
                Description = organizer.Description,
                Image = organizer.Image,
                Website = organizer.Image,
                Slug = organizer.Slug
            });
        }

        [HttpPost, ActionName("DeleteOrganizer")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDeleteOrganizer(Guid id)
        {
            var organizer =
                await UnitOfWork.OrganizerRepository.GetUserOrganizerAsync(id, GetGuid(User.Identity.GetUserId()));
            if (organizer == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            if (organizer.IsDefault)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            try
            {
                foreach (var eventObj in organizer.Events.ToList())
                {
                    foreach (var ticket in eventObj.Tickets.ToList())
                    {
                        UnitOfWork.TicketRepository.Remove(ticket);
                    }

                    UnitOfWork.EventRepository.Remove(eventObj);

                    if (eventObj.Image != PlaceholderImagePath)
                    {
                        FileUtils.DeleteFile(Server.MapPath(eventObj.Image));
                    }
                }

                UnitOfWork.OrganizerRepository.Remove(organizer);
                await UnitOfWork.SaveChangesAsync();

                if (organizer.Image != PlaceholderImagePath)
                {
                    FileUtils.DeleteFile(Server.MapPath(organizer.Image));
                }

                return RedirectToAction("UpdateOrganizer", "Account");
            }
            catch
            {
                //TODO: Log error
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadOrganizerImage(int x, int y, int w, int h)
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

                    var fileName = FileUtils.GenerateFileName("o", "jpg");
                    FileUtils.CropSaveImage(file.InputStream, x, y, w, h, Path.Combine(absolutePath, fileName));

                    return Json(new
                    {
                        success = true,
                        path = Path.Combine(UploadPath,
                            string.Format("{0}?{1}", fileName, DateTime.UtcNow.ToBinary()))
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Helpers

        /// <summary>
        /// Returns the authentication middleware for the current request
        /// </summary>
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        /// <summary>
        /// Asynchronous user sign in
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        private async Task SignInAsync(IdentityUser user, bool isPersistent)
        {
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties {IsPersistent = isPersistent}, identity);
        }

        /// <summary>
        /// Adds identity result errors to model state
        /// </summary>
        /// <param name="result"></param>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        /// <summary>
        /// Redirects to the given return url and to default
        /// if no url is provided
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

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
        /// Checks if organizer slug is unique
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="organizerId"></param>
        /// <returns></returns>
        private async Task<bool> IsOrganizerSlugUnique(string slug, Guid organizerId)
        {
            var organizers = await UnitOfWork.OrganizerRepository.GetAllAsync();
            var organizer = organizers.FirstOrDefault(c => c.OrganizerId == organizerId);
            if (organizer != null && organizer.Slug == slug)
            {
                return organizers.Count(o => o.Slug == slug) <= 1;
            }
            return organizers.Count(o => o.Slug == slug) == 0;
        }

        #endregion
    }
}
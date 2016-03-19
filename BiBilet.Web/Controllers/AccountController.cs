using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BiBilet.Domain;
using BiBilet.Domain.Entities.Application;
using BiBilet.Web.Identity;
using BiBilet.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace BiBilet.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly UserManager<IdentityUser, Guid> _userManager;

        public AccountController(UserManager<IdentityUser, Guid> userManager, IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            // Create user policy
            userManager.UserValidator = new UserValidator<IdentityUser, Guid>(userManager)
            {
                RequireUniqueEmail = true
            };

            // Create password policy
            userManager.PasswordValidator = new PasswordValidator()
            {
                RequireDigit = true,
                RequireNonLetterOrDigit = true,
                RequireLowercase = true,
                RequiredLength = 6,
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
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Hatalı kullanıcı adı veya şifre girdiniz");
                }
            }

            // If we got this far, something failed, redisplay form
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
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser() { UserName = model.UserName, Name = model.Name, Email = model.Email };
                user.Organizers.Add(new Organizer()
                {
                    OrganizerId = Guid.NewGuid(),
                    Name = user.Name,
                    Slug = string.Format("organizer-{0}", user.UserName.ToLowerInvariant()),
                    Description = string.Format("{0} kullanıcısının organizatör profili.", user.Name),
                    Image = "/assets/images/event-placeholder.png",
                    Website = string.Empty,
                });

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Settings(string message)
        {
            ViewBag.StatusMessage = message;

            var user = await _userManager.FindByIdAsync(GetGuid(User.Identity.GetUserId()));
            var model = new SettingsViewModel()
            {
                UserName = user.UserName,
                Name = user.Name,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Settings(SettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.GetUserName());
                if (user != null)
                {
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
                                else
                                {
                                    AddErrors(result);
                                    return View(model);
                                }
                            }
                            return RedirectToAction("Settings", "Account",
                                new { message = "Kullanıcı başarıyla güncellendi" });
                        }
                        else
                        {
                            AddErrors(result);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("OldPassword", "Hatalı şifre girdiniz");
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Organizer(Guid? id, string message)
        {
            ViewBag.StatusMessage = message;

            var organizers =
                await UnitOfWork.OrganizerRepository.GetUserOrganizersAsync(GetGuid(User.Identity.GetUserId()));

            Organizer organizer;
            if (id.HasValue)
            {
                organizer = organizers.FirstOrDefault(o => o.OrganizerId.Equals(id.Value));
                if (organizer == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
            }
            else
            {
                organizer = organizers.First();
            }

            var model = new OrganizerViewModel()
            {
                OrganizerId = organizer.OrganizerId,
                Name = organizer.Name,
                Description = organizer.Description,
                Image = organizer.Image,
                Website = organizer.Website,
                Slug = organizer.Slug,
                EventCount = organizer.Events.Count(),
                IsRemovable = !organizer.IsDefault
            };

            ViewBag.Organizers = organizers;

            return View(model);
        }

        [HttpPost, ActionName("Organizer")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateOrganizer(OrganizerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var organizers =
                    await UnitOfWork.OrganizerRepository.GetUserOrganizersAsync(GetGuid(User.Identity.GetUserId()));

                var organizer = organizers.FirstOrDefault(o => o.OrganizerId.Equals(model.OrganizerId));
                if (organizer != null)
                {
                    try
                    {
                        organizer.Name = model.Name;
                        organizer.Description = model.Description;
                        organizer.Image = model.Image;
                        organizer.Website = model.Website;
                        organizer.Slug = model.Slug;

                        if (await IsOrganizerSlugExists(model.Slug, organizer.OrganizerId))
                        {
                            UnitOfWork.OrganizerRepository.Update(organizer);
                            await UnitOfWork.SaveChangesAsync();

                            return RedirectToAction("Organizer", "Account",
                                new { id = organizer.OrganizerId, message = "Profil başarıyla güncellendi" });
                        }
                        ModelState.AddModelError("slug", "Url kısaltması özel olmalıdır");
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Organizatör profili güncellenirken bir hata oluştu");
                    }
                }

                ViewBag.Organizers = organizers;
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult CreateOrganizer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrganizer(OrganizerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var organizer = new Organizer()
                    {
                        OrganizerId = Guid.NewGuid(),
                        Name = model.Name,
                        Description = model.Description,
                        Image = model.Image,
                        Website = model.Website,
                        Slug = model.Slug,
                        IsDefault = false,
                        UserId = GetGuid(User.Identity.GetUserId())
                    };

                    if (await IsOrganizerSlugExists(model.Slug, organizer.OrganizerId))
                    {
                        UnitOfWork.OrganizerRepository.Add(organizer);
                        await UnitOfWork.SaveChangesAsync();

                        return RedirectToAction("Organizer", "Account", new { id = organizer.OrganizerId });
                    }
                    ModelState.AddModelError("slug", "Url kısaltması özel olmalıdır");
                }
                catch
                {
                    ModelState.AddModelError("", "Organizatör profili oluşturulurken bir hata oluştu");
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteOrganizer(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var organizer = await UnitOfWork.OrganizerRepository.FindByIdAsync(id);
            if (organizer == null || organizer.UserId != GetGuid(User.Identity.GetUserId()) || organizer.IsDefault)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(new OrganizerViewModel()
            {
                OrganizerId = organizer.OrganizerId,
                Name = organizer.Name,
                Description = organizer.Description,
                Image = organizer.Image,
                Website = organizer.Image,
                Slug = organizer.Slug
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteOrganizer(Guid id)
        {
            try
            {
                var organizer = await UnitOfWork.OrganizerRepository.FindByIdAsync(id);
                if (organizer == null || organizer.UserId != GetGuid(User.Identity.GetUserId()) || organizer.IsDefault)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                UnitOfWork.OrganizerRepository.Remove(organizer);
                await UnitOfWork.SaveChangesAsync();

                return RedirectToAction("Organizer", "Account");
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
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

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private async Task SignInAsync(IdentityUser user, bool isPersistent)
        {
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
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
        private async Task<bool> IsOrganizerSlugExists(string slug, Guid organizerId)
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
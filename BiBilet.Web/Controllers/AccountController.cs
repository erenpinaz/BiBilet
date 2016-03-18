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
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser, Guid> _userManager;

        public AccountController(UserManager<IdentityUser, Guid> userManager, IUnitOfWork unitOfWork)
        {
            // Create password policy
            userManager.PasswordValidator = new PasswordValidator()
            {
                RequireDigit = true,
                RequireNonLetterOrDigit = true,
                RequireLowercase = true,
                RequiredLength = 6
            };

            _unitOfWork = unitOfWork;
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
                    Slug = string.Format("organizer-profile-{0}", user.UserName.ToLowerInvariant()),
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
        public async Task<ActionResult> MyProfile(Guid? id, string message)
        {
            ViewBag.StatusMessage = message;

            var organizers =
                await _unitOfWork.OrganizerRepository.GetUserOrganizersAsync(GetGuid(User.Identity.GetUserId()));

            ViewBag.Organizers = organizers;

            Organizer organizer;
            if (id.HasValue)
            {
                organizer = await _unitOfWork.OrganizerRepository.GetUserOrganizerAsync(id.Value, GetGuid(User.Identity.GetUserId()));
                if (organizer == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
            }
            else
            {
                organizer = organizers.First();
            }

            var model = new MyProfileViewModel()
            {
                OrganizerId = organizer.OrganizerId,
                Name = organizer.Name,
                Description = organizer.Description,
                Image = organizer.Image,
                Website = organizer.Website,
                Slug = organizer.Slug
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MyProfile(MyProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var organizer =
                    await _unitOfWork.OrganizerRepository.GetUserOrganizerAsync(model.OrganizerId, GetGuid(User.Identity.GetUserId()));
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
                            _unitOfWork.OrganizerRepository.Update(organizer);
                            await _unitOfWork.SaveChangesAsync();

                            return RedirectToAction("Index", "Home");
                        }
                        ModelState.AddModelError("slug", "Url kısaltması özel olmalıdır");
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Organizatör profili güncellenirken bir sorun oluştu");
                    }
                }
            }

            return View(model);
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

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            Error
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

        private static Guid GetGuid(string value)
        {
            Guid result;
            Guid.TryParse(value, out result);
            return result;
        }

        private async Task<bool> IsOrganizerSlugExists(string slug, Guid organizerId)
        {
            var organizers = await _unitOfWork.OrganizerRepository.GetAllAsync();
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
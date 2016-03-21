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
            // Create user policy
            userManager.UserValidator = new UserValidator<IdentityUser, Guid>(userManager)
            {
                RequireUniqueEmail = true,
                AllowOnlyAlphanumericUserNames = true
            };

            // Create password policy
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
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Get the user from the database by user name and password
                var user = await _userManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    // If exists, sign in
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
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create user
                var user = new IdentityUser {UserName = model.UserName, Name = model.Name, Email = model.Email};

                // Create default organizer profile for the user
                var organizer = new Organizer
                {
                    OrganizerId = Guid.NewGuid(),
                    Name = user.Name,
                    Description = string.Format("{0} kullanıcısının organizatör profili.", user.Name),
                    Image = PlaceholderImagePath,
                    IsDefault = true
                };
                organizer.Slug = string.Format("o-{0}", organizer.OrganizerId.ToString("N"));

                // Add default organizer profile to the user
                user.Organizers.Add(organizer);

                // Save user to database
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // If succeeded, sign in
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
            // Get the current user from the database by user name
            var user = await _userManager.FindByIdAsync(GetGuid(User.Identity.GetUserId()));

            // Map the user to view model
            var model = new SettingsViewModel
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
        public async Task<ActionResult> Settings(SettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the current user from the database by user name
                var user = await _userManager.FindByIdAsync(GetGuid(User.Identity.GetUserId()));
                if (user != null)
                {
                    // Check the password
                    if (await _userManager.CheckPasswordAsync(user, model.OldPassword))
                    {
                        user.Name = model.Name;
                        user.Email = model.Email;

                        // If the password is correct, update the user record
                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            if (model.NewPassword != null && model.ConfirmPassword != null)
                            {
                                // Change the old password if new password field has value
                                result =
                                    await
                                        _userManager.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);
                                if (result.Succeeded)
                                {
                                    // If succeeded, sign out
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
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Organizer(Guid? id, string message)
        {
            // Get all the user organizers from the database to populate profile list
            var organizers =
                await UnitOfWork.OrganizerRepository.GetUserOrganizersAsync(GetGuid(User.Identity.GetUserId()));

            // Return the first organizer if no organizer id provided
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

            // Map the organizer to view model
            var model = new OrganizerViewModel
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

            ViewBag.StatusMessage = message;
            ViewBag.Organizers = organizers;

            return View(model);
        }

        [HttpPost, ActionName("Organizer")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateOrganizer(Guid id, OrganizerViewModel model)
        {
            // Get all the user organizers from the database to populate profile list
            var organizers =
                await UnitOfWork.OrganizerRepository.GetUserOrganizersAsync(GetGuid(User.Identity.GetUserId()));

            if (ModelState.IsValid)
            {
                // Get the selected organizer profile
                var organizer = organizers.FirstOrDefault(o => o.OrganizerId.Equals(id));
                if (organizer != null)
                {
                    try
                    {
                        organizer.Name = model.Name;
                        organizer.Description = model.Description;
                        organizer.Image = model.Image ?? PlaceholderImagePath;
                        organizer.Website = model.Website;
                        organizer.Slug = model.Slug;

                        if (await IsOrganizerSlugExists(model.Slug, organizer.OrganizerId))
                        {
                            // If the organizer slug is unique, update the organizer record
                            UnitOfWork.OrganizerRepository.Update(organizer);
                            await UnitOfWork.SaveChangesAsync();

                            return RedirectToAction("Organizer", "Account",
                                new {id = organizer.OrganizerId, message = "Profil başarıyla güncellendi"});
                        }
                        ModelState.AddModelError("slug", "Url kısaltması özel olmalıdır");
                    }
                    catch
                    {
                        //TODO: Log error
                        ModelState.AddModelError("", "Organizatör profili güncellenirken bir hata oluştu");
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
            }

            ViewBag.Organizers = organizers;

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> CreateOrganizer()
        {
            // Create organizer
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
                // Save organizer to the database
                UnitOfWork.OrganizerRepository.Add(organizer);
                await UnitOfWork.SaveChangesAsync();
            }
            catch
            {
                //TODO: Log error
            }

            return RedirectToAction("Organizer", "Account", new {id = organizer.OrganizerId});
        }

        [HttpGet]
        public async Task<ActionResult> DeleteOrganizer(Guid id)
        {
            // Get the user organizer by id from the database
            var organizer =
                await UnitOfWork.OrganizerRepository.GetUserOrganizerAsync(id, GetGuid(User.Identity.GetUserId()));
            if (organizer == null)
            {
                // If organizer is null or not belong to the current user
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            if (organizer.IsDefault)
            {
                // If the organizer is default organizer
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            // Map organizer to view model
            return View(new OrganizerViewModel
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
            try
            {
                // Get the user organizer by id from the database
                var organizer =
                    await UnitOfWork.OrganizerRepository.GetUserOrganizerAsync(id, GetGuid(User.Identity.GetUserId()));
                if (organizer == null)
                {
                    // If organizer is null or not belong to the current user
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                if (organizer.IsDefault)
                {
                    // If the organizer is default organizer
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                // Delete organizer record from the database
                UnitOfWork.OrganizerRepository.Remove(organizer);
                await UnitOfWork.SaveChangesAsync();

                if (organizer.Image != PlaceholderImagePath)
                {
                    // If not the placeholder image, delete the organizer image upload directory
                    DiskUtils.DeleteDirectory(Server.MapPath(Path.GetDirectoryName(organizer.Image)));
                }

                return RedirectToAction("Organizer", "Account");
            }
            catch
            {
                //TODO: Log error
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadOrganizerImage(Guid id)
        {
            // Get the user organizer from the database
            var organizer =
                await UnitOfWork.OrganizerRepository.GetUserOrganizerAsync(id, GetGuid(User.Identity.GetUserId()));
            if (organizer == null)
            {
                // If organizer is null or not belong to the current user
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            try
            {
                // Get the file from the current HTTP request
                var file = Request.Files[0];
                if (file != null)
                {
                    const string fileName = "organizer-image.jpg";

                    var absolutePath = Server.MapPath(Path.Combine(UploadPath, id.ToString()));

                    // If not exists, create the physical upload directory
                    if (!Directory.Exists(absolutePath))
                    {
                        Directory.CreateDirectory(absolutePath);
                    }

                    // Save image to the physical upload directory
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
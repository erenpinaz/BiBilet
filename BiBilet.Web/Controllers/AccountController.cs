﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
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
                RequireUniqueEmail = true
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
                var user = await _userManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                ModelState.AddModelError("", "Hatalı kullanıcı adı veya şifre girdiniz");
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
                var user = new IdentityUser {UserName = model.UserName, Name = model.Name, Email = model.Email};
                var organizer = new Organizer()
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

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Settings(string message)
        {
            var user = await _userManager.FindByIdAsync(GetGuid(User.Identity.GetUserId()));
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
            var organizers =
                await UnitOfWork.OrganizerRepository.GetUserOrganizersAsync(GetGuid(User.Identity.GetUserId()));

            if (ModelState.IsValid)
            {
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
                            UnitOfWork.OrganizerRepository.Update(organizer);
                            await UnitOfWork.SaveChangesAsync();

                            return RedirectToAction("Organizer", "Account",
                                new {id = organizer.OrganizerId, message = "Profil başarıyla güncellendi"});
                        }
                        ModelState.AddModelError("slug", "Url kısaltması özel olmalıdır");
                    }
                    catch
                    {
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
            var organizer = new Organizer()
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

            return RedirectToAction("Organizer", "Account", new {id = organizer.OrganizerId});
        }

        [HttpGet]
        public async Task<ActionResult> DeleteOrganizer(Guid id)
        {
            var organizer = await UnitOfWork.OrganizerRepository.FindByIdAsync(id);
            if (organizer == null || organizer.UserId != GetGuid(User.Identity.GetUserId()) || organizer.IsDefault)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

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
                var organizer = await UnitOfWork.OrganizerRepository.FindByIdAsync(id);
                if (organizer == null || organizer.UserId != GetGuid(User.Identity.GetUserId()) || organizer.IsDefault)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                UnitOfWork.OrganizerRepository.Remove(organizer);
                await UnitOfWork.SaveChangesAsync();

                if (organizer.Image != PlaceholderImagePath)
                {
                    DiskUtils.DeleteDirectory(Server.MapPath(Path.GetDirectoryName(organizer.Image)));
                }

                return RedirectToAction("Organizer", "Account");
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult UploadOrganizerImage(Guid id)
        {
            var organizer = UnitOfWork.OrganizerRepository.GetUserOrganizer(id, GetGuid(User.Identity.GetUserId()));
            if (organizer == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            try
            {
                var file = Request.Files[0];
                if (file != null)
                {
                    var fileName = "organizer-image.jpg";

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
        ///     Returns the authentication middleware for the current request
        /// </summary>
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        /// <summary>
        ///     Asynchronous user sign in
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
        ///     Adds identity result errors to model state
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
        ///     Redirects to the given return url and to default
        ///     if no url is provided
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
        ///     Converts string to Guid
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
        ///     Checks if organizer slug is unique
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
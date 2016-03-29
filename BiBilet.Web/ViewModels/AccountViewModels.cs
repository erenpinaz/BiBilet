using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BiBilet.Web.ViewModels
{
    public class OrganizerEditModel
    {
        public Guid OrganizerId { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [StringLength(256, MinimumLength = 3)]
        [DataType(DataType.Url)]
        [Display(Name = "Website")]
        public string Website { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "Resim")]
        public string Image { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Url Kısaltması")]
        public string Slug { get; set; }

        public bool IsRemovable { get; set; }
    }

    public class SettingsEditModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Kullanıcı adı")]
        public string UserName { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Required]
        [StringLength(320, MinimumLength = 3)]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Şimdiki şifre")]
        public string OldPassword { get; set; }

        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni şifre")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni şifre onayı")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginEditModel
    {
        [Required]
        [Display(Name = "Kullanıcı adı")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Display(Name = "Beni hatırla?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterEditModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Kullanıcı adı")]
        public string UserName { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Required]
        [StringLength(320, MinimumLength = 3)]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre onayı")]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
using BiBilet.Domain.Entities.Application;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BiBilet.Web.ViewModels
{
    //TODO: Update temporary string length annotations

    public class EventEditModel
    {
        public EventEditModel()
        {
            Tickets = new List<TicketViewModel>();
        }

        public SelectList Organizers { get; set; }
        public SelectList Categories { get; set; }
        public SelectList Topics { get; set; }
        public SelectList SubTopics { get; set; }

        public Guid EventId { get; set; }

        [Display(Name = "Organizatör")]
        public Guid OrganizerId { get; set; }

        public Guid VenueId { get; set; }

        [Display(Name = "Kategori")]
        public Guid CategoryId { get; set; }

        [Display(Name = "Konu")]
        public Guid TopicId { get; set; }

        [Display(Name = "Alt Konu")]
        public Guid SubTopicId { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Başlık")]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "Resim")]
        public string Image { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Url Kısaltması")]
        public string Slug { get; set; }

        [Display(Name = "Yayınla")]
        public bool Published { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Başlangıç")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Bitiş")]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "İsim")]
        public string VenueName { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Adres")]
        public string VenueAddress { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Şehir")]
        public string VenueCity { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Ülke")]
        public string VenueCountry { get; set; }

        [Required]
        [Display(Name = "Biletler")]
        public List<TicketViewModel> Tickets { get; set; }
    }

    public class EventViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Date { get; internal set; }
        public string Time { get; set; }
        public string OrganizerName { get; set; }
        public string VenueName { get; set; }
        public string VenueAddress { get; set; }
        public string CategoryName { get; set; }
        public string TopicName { get; set; }
        public string SubTopicName { get; set; }
        public bool IsLive { get; set; }

        public List<TicketViewModel> Tickets { get; set; }
    }

    public class TicketViewModel
    {
        public Guid TicketId { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Başlık")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Adet")]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Ücret")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Tip")]
        public TicketType Type { get; set; }
    }
}
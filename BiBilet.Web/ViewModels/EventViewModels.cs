using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using BiBilet.Domain.Entities.Application;
using PagedList;

namespace BiBilet.Web.ViewModels
{
    public class SearchEventEditModel
    {
        [Display(Name = "Etkinlik ara")]
        public string q { get; set; }
    }

    public class SearchEventViewModel
    {
        public List<Category> Categories { get; set; }
        public IPagedList<Event> Events { get; set; }
    }

    public class EventEditModel
    {
        public EventEditModel()
        {
            Tickets = new List<TicketEditModel>();
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
        [StringLength(256, MinimumLength = 3)]
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
        [StringLength(256, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Url kısaltması")]
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
        [StringLength(256, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "İsim")]
        public string VenueName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Adres")]
        public string VenueAddress { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Şehir")]
        public string VenueCity { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Ülke")]
        public string VenueCountry { get; set; }

        [Required]
        [Display(Name = "Biletler")]
        public List<TicketEditModel> Tickets { get; set; }
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

    public class TicketEditModel
    {
        public Guid TicketId { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Başlık")]
        public string Title { get; set; }

        [Required]
        [Range(1, 500)]
        [Display(Name = "Adet")]
        public int Quantity { get; set; }

        [Required]
        [Range(0, 500)]
        [Display(Name = "Ücret")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Tip")]
        public TicketType Type { get; set; }
    }

    public class TicketViewModel
    {
        public Guid TicketId { get; set; }

        public string Title { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string Type { get; set; }
        public bool IsPaid { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class ManageEventViewModel
    {
        public Guid EventId { get; set; }

        public string Title { get; set; }
        public int TotalTickets { get; set; }
        public int TotalPaidTickets { get; set; }
        public int TotalFreeTickets { get; set; }
        public int TotalSold { get; set; }
        public int TotalPaidSold { get; set; }
        public int TotalFreeSold { get; set; }
    }

    public class RegisterTicketEditModel
    {
        public Guid TicketId { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Ad")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Soyad")]
        public string LastName { get; set; }

        [Required]
        [StringLength(320, MinimumLength = 3)]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Adres")]
        public string Address { get; set; }

        public string EventTitle { get; set; }
        public string TicketTitle { get; set; }
    }

    public class UserTicketViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public string EventTitle { get; set; }
        public string EventAddress { get; set; }
        public string EventImage { get; set; }
        public string EventStartDate { get; set; }

        public string TicketTitle { get; set; }
        public string TicketType { get; set; }

        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
    }
}
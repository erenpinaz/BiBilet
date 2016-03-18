﻿using BiBilet.Domain.Entities.Application;
using System.Collections.Generic;

namespace BiBilet.Web.ViewModels
{
    public class HomeViewModel
    {
        public List<Event> HotEvents { get; set; }

        public List<Event> UpcomingEvents { get; set; }

        public List<Event> PastEvents { get; set; }

        public List<Category> Categories { get; set; }
    }
}
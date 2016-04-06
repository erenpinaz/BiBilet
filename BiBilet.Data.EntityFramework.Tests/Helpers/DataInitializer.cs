using System;
using System.Collections.Generic;
using BiBilet.Domain.Entities.Application;

namespace BiBilet.Data.EntityFramework.Tests.Helpers
{
    public class DataInitializer
    {
        /// <summary>
        /// Creates dummy events
        /// </summary>
        /// <returns></returns>
        public static List<Event> GetAllEvents()
        {
            var events = new List<Event>
            {
                new Event
                {
                    EventId = Guid.Parse("454a94a7-8de3-48be-b75e-cb99b18bc0d0"),
                    Title = "Test Event",
                    Description = "Test event description",
                    Image = "/assets/images/test.png",
                    Slug = "test-event",
                    Published = true,
                    StartDate = new DateTime(2016, 6, 12),
                    EndDate = new DateTime(2016, 6, 15)
                },
                new Event
                {
                    EventId = Guid.Parse("64e1129e-c700-4805-92a9-caac1ad9ccbf"),
                    Title = "Test Event 2",
                    Description = "Test event 2 description",
                    Image = "/assets/images/test2.png",
                    Slug = "test-event-2",
                    Published = true,
                    StartDate = new DateTime(2016, 7, 9),
                    EndDate = new DateTime(2016, 7, 13)
                }
            };

            return events;
        }
    }
}
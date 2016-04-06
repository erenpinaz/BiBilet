using System;
using System.Collections.Generic;
using BiBilet.Data.EntityFramework.Repositories.Application;
using BiBilet.Data.EntityFramework.Tests.Helpers;
using BiBilet.Domain.Entities.Application;
using BiBilet.Domain.Repositories.Application;
using Moq;
using NUnit.Framework;

namespace BiBilet.Data.EntityFramework.Tests
{
    public class EventRepositoryTest
    {
        /// <summary>
        /// Initial setup (one-time)
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            _expectedEvents = DataInitializer.GetAllEvents();
        }

        /// <summary>
        /// Final teardown (one-time)
        /// </summary>
        [OneTimeTearDown]
        public void DisposeAll()
        {
            _expectedEvents = null;
        }

        /// <summary>
        /// Re-initializes test data (repeating)
        /// </summary>
        [SetUp]
        public void ReInitializeTest()
        {
            _dbContext = new Mock<BiBiletContext>().Object;
            _eventRepository = SetUpEventRepository();
        }

        /// <summary>
        /// Tears down all the test data
        /// </summary>
        [TearDown]
        public void DisposeTest()
        {
            _eventRepository = null;
            _dbContext?.Dispose();
        }

        /// <summary>
        /// Create mock event repository
        /// </summary>
        /// <returns></returns>
        private IEventRepository SetUpEventRepository()
        {
            var mockRepository = new Mock<EventRepository>(_dbContext);

            // GetAll() method will return a list of dummy events
            mockRepository.Setup(e => e.GetAll()).Returns(_expectedEvents);

            // FindById() method will return a single dummy event
            mockRepository.Setup(e => e.FindById(It.IsAny<Guid>())).Returns(new Func<Guid, Event>(
                id => _expectedEvents.Find(e => e.EventId.Equals(id))));

            // Remove() method will delete a single dummy event
            mockRepository.Setup(e => e.Remove(It.IsAny<Event>())).Callback(new Action<Event>(returnedEvent =>
            {
                var eventToRemove = _expectedEvents.Find(x => x.EventId == returnedEvent.EventId);

                if (eventToRemove != null)
                    _expectedEvents.Remove(eventToRemove);
            }));

            return mockRepository.Object;
        }

        [Test]
        public void GetAll_should_return_all_events()
        {
            var actualEvents = _eventRepository.GetAll();

            CollectionAssert.AreEqual(actualEvents, _expectedEvents);
        }

        [Test]
        public void FindById_should_return_correct_event()
        {
            var eventId = Guid.Parse("64e1129e-c700-4805-92a9-caac1ad9ccbf");
            var actualEvent = _eventRepository.FindById(eventId);

            Assert.AreEqual(actualEvent, _expectedEvents.Find(e => e.EventId.Equals(eventId)));
        }

        [Test]
        public void Remove_should_remove_correct_event()
        {
            var eventId = Guid.Parse("64e1129e-c700-4805-92a9-caac1ad9ccbf");
            var eventToRemove = _eventRepository.FindById(eventId);
            _eventRepository.Remove(eventToRemove);

            Assert.IsNull(_expectedEvents.Find(e => e.EventId.Equals(eventId)));
        }

        #region Variables

        private BiBiletContext _dbContext;
        private IEventRepository _eventRepository;
        private List<Event> _expectedEvents;

        #endregion
    }
}
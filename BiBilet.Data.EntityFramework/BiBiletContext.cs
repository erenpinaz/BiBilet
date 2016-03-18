using System.Data.Entity;
using BiBilet.Data.EntityFramework.Configuration.Application;
using BiBilet.Data.EntityFramework.Configuration.Identity;
using BiBilet.Domain.Entities.Application;
using BiBilet.Domain.Entities.Identity;

namespace BiBilet.Data.EntityFramework
{
    public class BiBiletContext : DbContext
    {
        static BiBiletContext()
        {
            Database.SetInitializer(new BiBiletDbInitializer());
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BiBiletContext()
            : base("BiBiletContext")
        {
        }

        /// <summary>
        /// DI Constructor
        /// </summary>
        /// <param name="nameOrConnectionString"></param>
        public BiBiletContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        // Identity
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public IDbSet<ExternalLogin> ExternalLogins { get; set; }

        // Application
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<SubTopic> SubTopics { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Organizer> Organizers { get; set; }
        public DbSet<UserTicket> UserTickets { get; set; }

        /// <summary>
        /// Configures values & table relationships
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Identity
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new RoleConfiguration());
            modelBuilder.Configurations.Add(new ExternalLoginConfiguration());
            modelBuilder.Configurations.Add(new ClaimConfiguration());

            // Application
            modelBuilder.Configurations.Add(new EventConfiguration());
            modelBuilder.Configurations.Add(new TicketConfiguration());
            modelBuilder.Configurations.Add(new CategoryConfiguration());
            modelBuilder.Configurations.Add(new TopicConfiguration());
            modelBuilder.Configurations.Add(new SubTopicConfiguration());
            modelBuilder.Configurations.Add(new VenueConfiguration());
            modelBuilder.Configurations.Add(new OrganizerConfiguration());
            modelBuilder.Configurations.Add(new UserTicketConfiguration());
        }
    }
}
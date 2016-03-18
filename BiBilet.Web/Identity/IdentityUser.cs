using System;
using System.Collections.Generic;
using BiBilet.Domain.Entities.Application;
using Microsoft.AspNet.Identity;

namespace BiBilet.Web.Identity
{
    public class IdentityUser : IUser<Guid>
    {
        public IdentityUser()
        {
            this.Id = Guid.NewGuid();
            this.Organizers = new List<Organizer>();
        }

        public IdentityUser(string userName)
            : this()
        {
            this.UserName = userName;
            this.Organizers = new List<Organizer>();
        }

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string SecurityStamp { get; set; }
        public List<Organizer> Organizers { get; set; }
    }
}
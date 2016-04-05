using System;
using System.Collections.Generic;
using BiBilet.Domain.Entities.Application;

namespace BiBilet.Domain.Entities.Identity
{
    public class User
    {
        #region Fields

        private ICollection<Claim> _claims;
        private ICollection<ExternalLogin> _externalLogins;
        private ICollection<Role> _roles;
        private ICollection<UserTicket> _userTickets;
        private ICollection<Organizer> _organizers;

        #endregion

        #region Scalar Properties

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string SecurityStamp { get; set; }

        #endregion

        #region Navigation Properties

        public virtual ICollection<Claim> Claims
        {
            get { return _claims ?? (_claims = new List<Claim>()); }
            set { _claims = value; }
        }

        public virtual ICollection<ExternalLogin> Logins
        {
            get
            {
                return _externalLogins ??
                       (_externalLogins = new List<ExternalLogin>());
            }
            set { _externalLogins = value; }
        }

        public virtual ICollection<Role> Roles
        {
            get { return _roles ?? (_roles = new List<Role>()); }
            set { _roles = value; }
        }

        public virtual ICollection<UserTicket> UserTickets
        {
            get { return _userTickets ?? (_userTickets = new List<UserTicket>()); }
            set { _userTickets = value; }
        }

        public virtual ICollection<Organizer> Organizers
        {
            get { return _organizers ?? (_organizers = new List<Organizer>()); }
            set { _organizers = value; }
        }

        #endregion
    }
}
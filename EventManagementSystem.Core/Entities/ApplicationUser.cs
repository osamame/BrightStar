using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace EventManagementSystem.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Registration> Registrations { get; set; }
    }
}
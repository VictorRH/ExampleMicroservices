using Microsoft.AspNetCore.Identity;
using System;

namespace Microservices.API.Security.Core.Entities
{
    public class Users : IdentityUser
    {
        public string IpUser { get; set; }
        public DateTime? RegisterDate { get; set; }
    }
}

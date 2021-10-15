using Microsoft.AspNetCore.Identity;

namespace Microservices.API.Security.Core.Entities
{
    public class Users : IdentityUser
    {
        public string IpUser { get; set; }
        public string RegisterDate { get; set; }
    }
}

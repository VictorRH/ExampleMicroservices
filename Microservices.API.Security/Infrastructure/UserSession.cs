using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace Microservices.API.Security.Infrastructure
{
    public class UserSession : IUserSession
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserSession(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public string GetUserSession()
        {
            var userName = httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            return userName;
        }
    }
}

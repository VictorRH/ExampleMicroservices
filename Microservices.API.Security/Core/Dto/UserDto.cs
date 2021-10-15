namespace Microservices.API.Security.Core.Dto
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string RolUser { get; set; }
        public string Email { get; set; }
        public string IpUser { get; set; }
    }
}

using Microservices.API.Security.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Microservices.API.Security.Core.Persistence
{
    public class SecurityDbContext : IdentityDbContext<Users>
    {
        public SecurityDbContext(DbContextOptions<SecurityDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

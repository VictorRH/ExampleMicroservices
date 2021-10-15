using Microservices.API.Security.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservices.API.Security.Infrastructure.JwtLogic
{
    public interface IJwtGenerator
    {
        string CreateToken(Users users, List<string>rols);

    }
}

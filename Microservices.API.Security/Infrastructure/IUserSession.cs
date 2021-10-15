using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservices.API.Security.Infrastructure
{
    public interface IUserSession
    {
        string GetUserSession();

    }
}

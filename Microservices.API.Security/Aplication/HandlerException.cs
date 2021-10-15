using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microservices.API.Security.Aplication
{
    public class HandlerException : Exception
    {
        public HttpStatusCode Code { get; }

        public object Error { get; }

        public HandlerException(HttpStatusCode code, object error = null)
        {
            Code = code;
            Error = error;

        }
    }
}

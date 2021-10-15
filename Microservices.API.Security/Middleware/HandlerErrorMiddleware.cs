using Microservices.API.Security.Aplication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microservices.API.Security.Middleware
{
    public class HandlerErrorMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<HandlerErrorMiddleware> _logger;

        public HandlerErrorMiddleware(RequestDelegate next, ILogger<HandlerErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                await HandlerExceptionAsync(context, ex, _logger);
            }

        }

        private async static Task HandlerExceptionAsync(HttpContext context, Exception ex, ILogger<HandlerErrorMiddleware> logger)
        {
            object error = null;

            switch (ex)
            {
                case HandlerException me:
                    logger.LogError(ex, "Handler Error");
                    error = me.Error;
                    context.Response.StatusCode = (int)me.Code;
                    break;

                case Exception e:
                    logger.LogError(ex, "Error Server");
                    error = string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;

            }

            context.Response.ContentType = "application/json";
            if (error != null)
            {
                var result = JsonConvert.SerializeObject(new { error });
                await context.Response.WriteAsync(result);
            }


        }
    }
}

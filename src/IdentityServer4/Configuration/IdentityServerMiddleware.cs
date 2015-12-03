﻿using IdentityServer4.Core.Endpoints;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IdentityServer4.Core.Configuration
{
    public class IdentityServerMiddleware
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly RequestDelegate _next;
        private readonly IdentityServerOptions _options;

        public IdentityServerMiddleware(RequestDelegate next, IdentityServerOptions options, ILoggerFactory loggerFactory)
        {
            _next = next;
            _options = options;
            _logger = loggerFactory.CreateLogger<IdentityServerMiddleware>();
            _loggerFactory = loggerFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(new PathString("/token")))
            {
                var obj = context.ApplicationServices.GetService(typeof(TokenEndpoint));
                var endpoint = obj as IEndpoint;

                if (endpoint != null)
                {
                    var result = await endpoint.ProcessAsync(context);

                    if (result != null)
                    {
                        await result.ExecuteAsync(context, _loggerFactory.CreateLogger(result.GetType().FullName));
                    }

                    return;
                }
            }

            await _next(context);
        }
    }
}
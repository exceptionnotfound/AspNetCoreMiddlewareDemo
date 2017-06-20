using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreMiddlewareDemo.Middleware
{
    public class CustomHeaderMiddleware
    {
        private RequestDelegate _next;

        public CustomHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context);
            context.Response.Headers.Add("Custom-Middleware-Value", "CustomValue");
        }
    }
}

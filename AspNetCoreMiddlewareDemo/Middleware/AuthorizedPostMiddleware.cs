using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreMiddlewareDemo.Middleware
{
public class AuthorizedPostMiddleware
{
    private RequestDelegate _next;

    public AuthorizedPostMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        if(!context.User.Identity.IsAuthenticated && context.Request.Method == "POST")
        {
            context.Response.StatusCode = 401;
            return context.Response.WriteAsync("You are not permitted to perform POST actions.");
        }

        return _next.Invoke(context);
    }
}
}

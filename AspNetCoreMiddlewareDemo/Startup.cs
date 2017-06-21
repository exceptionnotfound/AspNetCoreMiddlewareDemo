using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AspNetCoreMiddlewareDemo.Middleware;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreMiddlewareDemo
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(15);
                options.CookieHttpOnly = true;
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //Uncomment this to see how app.Run() works.  The pipeline ends at the first call to Run().
            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync("Here's the last call in the pipeline.");
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //Session is an example of built-in middleware.  In ASP.NET Core, unlike in ASP.NET Framework, you must
            //explicity enable Session in order to use it in your apps.
            app.UseSession();

            //Static File handlers are another example of built-in middleware.  They enable the serving of static files
            //(e.g. JS, CSS, images), and by default those files are served from the wwwroot folder.
            app.UseStaticFiles();

            //If you want to invoke a piece of middleware but not end the pipeline, use app.Use()
            //This method chains middleware so that you can modify the response without immediately returning it.
            app.Use(async (context, next) =>
            {
                await next.Invoke();
                context.Response.Headers.Add("AppUseRan", "yes");
            });

            //Uncomment to see how MapWhen works.  MapWhen causes a split in the pipeline under certain conditions.
            //In this case, the pipeline splits when a "style" parameter exists in the query string.
            //app.MapWhen(context => context.Request.Query.ContainsKey("style"), HandleStyle);

            //This custom middleware rejects requests if they are POST request and the user is not authenticated.
            app.UseMiddleware<AuthorizedPostMiddleware>();

            //This custom middleware adds a Custom-Middleware-Value header to the response.
            app.UseMiddleware<CustomHeaderMiddleware>();

            //Finally, the MVC Routing middleware establishes the routes needed to access our MVC application.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void HandleStyle(IApplicationBuilder app)
        {
            //A call to app.Run ends the pipeline.  In this case, we want to return a response with a value from the query string.
            app.Run(async context =>
            {
                var style = context.Request.Query["style"];
                await context.Response.WriteAsync($"Style requested = {style}");
            });
        }
    }
}

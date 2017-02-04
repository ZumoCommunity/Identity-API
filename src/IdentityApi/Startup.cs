using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

using IdentityApi.Models;
using IdentityApi.Services;

namespace IdentityApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<User, string>(opts => {
                opts.Cookies.ApplicationCookie.LoginPath = new PathString("/Account/SignIn");
                opts.Cookies.ApplicationCookie.LogoutPath = new PathString("/Account/SignOut");
                opts.Cookies.ApplicationCookie.AccessDeniedPath = new PathString("/Account/AccessDenied");
                opts.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromHours(2);
                opts.Cookies.ApplicationCookie.SlidingExpiration = true;

                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            })
            .AddIdentityUserServices();

            // Add framework services.
            services.AddMvc();


            //var pathToDoc = Configuration["Swagger:Path"];
            var pathToDoc = @"bin\Debug\netcoreapp1.0\IdentityApi.xml";

            services.AddMvc();

            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Swashbuckle.Swagger.Model.Info {
                    Version = "v1",
                    Title = "Identity API",
                    Description = "Identity microservice",
                    TermsOfService = "None"
                });
                options.IncludeXmlComments(pathToDoc);
                options.DescribeAllEnumsAsStrings();
            });

            services.AddScoped<IUserService, UserService>();
            services.AddTransient<ITestDataInitializer, TestDataInitializer>(); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUi();

            var dataInitializer = app.ApplicationServices.GetRequiredService<ITestDataInitializer>();
            dataInitializer.InitTestDataAsync().Wait();
        }
    }
}

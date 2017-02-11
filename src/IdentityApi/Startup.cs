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
using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Security.Cryptography.X509Certificates;

namespace IdentityApi
{
    public class Startup
    {

        string _homeFolder;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            _homeFolder = env.ContentRootPath;
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


            services.AddSingleton<IClientStore, CustomClientStore>();


            string certFilePath = System.IO.Path.Combine(_homeFolder, "IdentityApi.pfx");
            string certPassword = Configuration.GetValue<string>("CERT_PWD");

            services.AddIdentityServer()
                //.AddTemporarySigningCredential()
                .AddDefaultEndpoints()
                .AddSigningCredential(new X509Certificate2(certFilePath, certPassword))
                .AddInMemoryIdentityResources(IdentityServerData.GetIdentityResources())
                .AddInMemoryApiResources(IdentityServerData.GetApiResources())
                .AddAspNetIdentity<User>();


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
            });

            services.AddScoped<IUserService, UserService>();
            services.AddTransient<ITestDataInitializer, TestDataInitializer>(); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }


            app.UseSwagger();
            app.UseSwaggerUi();

            app.UseIdentity();
            app.UseIdentityServer();

            //app.UseMvc();


            var dataInitializer = app.ApplicationServices.GetRequiredService<ITestDataInitializer>();
            dataInitializer.InitTestDataAsync().Wait();
        }
    }
}

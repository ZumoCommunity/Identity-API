using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using IdentityApi.Models;
using IdentityApi.Services;
using IdentityServer4.Stores;
using System.Security.Cryptography.X509Certificates;

using Korzh.WindowsAzure.Storage;

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
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment()) {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            AzureStorageConfig.ConnectionString = Configuration.GetConnectionString("AzureStorage");
            _homeFolder = env.ContentRootPath;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Add configuration service
            services.AddSingleton<IConfiguration>(Configuration);       


            // Add ASP.NET Identity services.
            services.AddIdentity<User, string>(opts => {
                //opts.Cookies.ApplicationCookie.LoginPath = new PathString("/Account/SignIn");
                //opts.Cookies.ApplicationCookie.LogoutPath = new PathString("/Account/SignOut");
                //opts.Cookies.ApplicationCookie.AccessDeniedPath = new PathString("/Account/AccessDenied");
                //opts.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromHours(2);
                //opts.Cookies.ApplicationCookie.SlidingExpiration = true;

                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            })
            .AddIdentityUserServices()
            .AddDefaultTokenProviders();



            //Add IdentityServer4 services
            services.AddSingleton<IClientStore, ZumoClientStore>();


            string certFilePath = System.IO.Path.Combine(_homeFolder, "IdentityApi.pfx");
            string certPassword = Configuration.GetValue<string>("CERT_PWD");

            services.AddIdentityServer(opts => {
                opts.UserInteraction.LoginUrl = "/Account/Login";
            })
                .AddSigningCredential(new X509Certificate2(certFilePath, certPassword))
                .AddDefaultEndpoints()
                .AddInMemoryIdentityResources(IdentityServerData.GetIdentityResources())
                .AddInMemoryApiResources(IdentityServerData.GetApiResources())

                .AddAspNetIdentity<User>();


            //services.AddIdentityServer()
            //    .AddTemporarySigningCredential()
            //    .AddInMemoryPersistedGrants()
            //    .AddInMemoryIdentityResources(Config.GetIdentityResources())
            //    .AddInMemoryApiResources(Config.GetApiResources())
            //    .AddInMemoryClients(Config.GetClients())
            //    .AddAspNetIdentity<ApplicationUser>();


            services.AddMvc();

            //Add swagger services
            var pathToDoc = Configuration["SwaggerPath"];

            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options => {
                options.SingleApiVersion(new Swashbuckle.Swagger.Model.Info {
                    Version = "v1",
                    Title = "Identity API",
                    Description = "Identity microservice",
                    TermsOfService = "None"
                });
                options.IncludeXmlComments(pathToDoc);
            });

            services.AddScoped<IUserService, UserService>();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddTransient<ITestDataInitializer, TestDataInitializer>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUi();

            app.UseIdentity();
            app.UseIdentityServer();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            app.UseGoogleAuthentication(new GoogleOptions
            {
                AuthenticationScheme = "Google",
                SignInScheme = "Identity.External", // this is the name of the cookie middleware registered by UseIdentity()
                ClientId = "998042782978-s07498t8i8jas7npj4crve1skpromf37.apps.googleusercontent.com",
                ClientSecret = "HsnwJri_53zn7VcO1Fm7THBb",
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            var dataInitializer = app.ApplicationServices.GetRequiredService<ITestDataInitializer>();
            dataInitializer.InitTestDataAsync().Wait();
        }
    }
}

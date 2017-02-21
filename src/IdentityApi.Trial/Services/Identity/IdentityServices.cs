using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;

using IdentityServer4;
using IdentityServer4.Models;

using IdentityApi.Models;

namespace IdentityApi.Services
{
    public static class WebExtensions
    {
        public static IdentityBuilder AddIdentityUserServices(this IdentityBuilder builder) {
            builder.Services.AddScoped<IUserStore<User>, UserService>();
            builder.Services.AddScoped<IRoleStore<string>, IdentityRoleStore>();
            builder.Services.AddScoped<IUserClaimsPrincipalFactory<User>, IdentityClaimsPrincipalFactory<User, string>>();
            return builder;
        }
    }

    public class IdentityClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser, TRole>
    where TUser : class
    where TRole : class
    {
        public IdentityClaimsPrincipalFactory(
            UserManager<TUser> userManager,
            RoleManager<TRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor) {
        }


        public override async Task<ClaimsPrincipal> CreateAsync(TUser usr) {
            var principal = await base.CreateAsync(usr);
            User user = usr as User;
            var identity = principal.Identities.First();
            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FullName ?? ""));
            return principal;
        }

    }


}

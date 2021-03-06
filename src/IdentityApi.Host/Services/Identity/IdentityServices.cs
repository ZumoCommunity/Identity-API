﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using IdentityApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using System.Security.Claims;

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
            identity.AddClaim(new Claim(ClaimTypes.Name, user.FullName ?? ""));
            identity.AddClaim(new Claim("given_name", user.FullName ?? ""));
            identity.AddClaim(new Claim("gender", user.Gender.ToString()));

            return principal;
        }

    }

    /// <summary>
    /// Implements <see cref="ILookupNormalizer"/> by converting keys to their lower cased invariant culture representation.
    /// </summary>
    public class LowerInvariantLookupNormalizer : Microsoft.AspNetCore.Identity.ILookupNormalizer
    {
        /// <summary>
        /// Returns a normalized representation of the specified <paramref name="key"/>
        /// by converting keys to their lower cased invariant culture representation.
        /// </summary>
        /// <param name="key">The key to normalize.</param>
        /// <returns>A normalized representation of the specified <paramref name="key"/>.</returns>
        public virtual string Normalize(string key) {
            if (key == null) {
                return null;
            }
            return key.Normalize().ToLowerInvariant();
        }
    }
}

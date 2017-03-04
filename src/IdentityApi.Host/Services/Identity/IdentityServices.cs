using System;
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

        public static string GetUserName(this ClaimsPrincipal principal) {
            if (principal == null) {
                throw new ArgumentNullException(nameof(principal));
            }
            return principal.FindFirstValue(ClaimTypes.Name);
        }

        public static string GetUserFirstName(this ClaimsPrincipal principal) {
            if (principal == null) {
                throw new ArgumentNullException(nameof(principal));
            }
            return principal.FindFirstValue(ClaimTypes.GivenName);
        }

        public static string GetUserLastName(this ClaimsPrincipal principal) {
            if (principal == null) {
                throw new ArgumentNullException(nameof(principal));
            }
            return principal.FindFirstValue(ClaimTypes.Surname);
        }

        public static string GetUserFullName(this ClaimsPrincipal principal) {
            if (principal == null) {
                throw new ArgumentNullException(nameof(principal));
            }
            string fullName = principal.FindFirstValue(ClaimTypes.GivenName);
            string lastName = principal.FindFirstValue(ClaimTypes.Surname);
            if (!string.IsNullOrEmpty(lastName)) {
                if (!string.IsNullOrEmpty(fullName))
                    fullName += " ";
                fullName += lastName;
            }
            return fullName;

        }

        public static string GetUserId(this ClaimsPrincipal principal) {
            if (principal == null) {
                throw new ArgumentNullException(nameof(principal));
            }
            return principal.FindFirstValue(ClaimTypes.NameIdentifier);

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
            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName ?? ""));
            identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName ?? ""));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.FullName ?? ""));


            //add OpenID Connect claims (full list is listed here: https://openid.net/specs/openid-connect-core-1_0.html#Claims)
            identity.AddClaim(new Claim("given_name", user.FirstName ?? ""));
            identity.AddClaim(new Claim("surname", user.LastName ?? ""));
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

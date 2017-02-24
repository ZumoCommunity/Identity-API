using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityModel;

namespace IdentityApi.Services
{


    public class ZumoClientStore : IClientStore
    {
        public static IEnumerable<Client> AllClients { get; } = new[] {
            new Client {
                ClientId = "myClient",
                ClientName = "My Custom Client",
                AccessTokenLifetime = 60 * 60 * 24,
                //AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                //RequireClientSecret = false,

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = new List<Secret> {
                        new Secret("superSecretPassword".Sha256())},
                AllowedScopes = {"myAPIs"}
            },
            new Client {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                ClientSecrets = {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = { "api1" }
            },

            // resource owner password grant client
            new Client {
                ClientId = "ro.client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                ClientSecrets = {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = { "api1" }
            },

            // OpenID Connect hybrid flow and client credentials client (MVC)
            new Client {
                ClientId = "mvc",
                ClientName = "Web Client",
                AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                RequireConsent = false,

                ClientSecrets = {
                    new Secret("secret".Sha256())
                },

                RedirectUris = { "http://localhost:5002/signin-oidc" },
                PostLogoutRedirectUris = { "http://localhost:5002" },

                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "api1"
                },
                AllowOfflineAccess = true
            }

        };

        public Task<Client> FindClientByIdAsync(string clientId) {
            return Task.FromResult(AllClients.FirstOrDefault(c => c.ClientId == clientId));
        }
    }


    internal class IdentityServerData
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() {
            return new List<IdentityResource> {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource {
                    Name = "role",
                    UserClaims = new List<string> {"role"}
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources() {
            var apiSecret = "scopeSecret".Sha256();

            return new List<ApiResource> {
                //new ApiResource {
                //    Name = "api1",
                //    DisplayName = "Custom API 01",
                //    Description = "Custom API Access",
                //    UserClaims = new List<string> {
                //        JwtClaimTypes.Name,
                //        JwtClaimTypes.Role,
                //        JwtClaimTypes.GivenName
                //    },
                //    ApiSecrets = new List<Secret> { new Secret(apiSecret) },
                //    Scopes = new List<Scope> {
                //        new Scope("api1")                    }
                //}
                new ApiResource(
                    "api1",                                       // Api resource name
                    "Custom API Set #1",                          // Display name
                    new[] { JwtClaimTypes.Name, JwtClaimTypes.Role, JwtClaimTypes.GivenName, "office" }) // Claims to be included in access token

            };
        }
    }

    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources() {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources() {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API 1")
            };
        }

        // clients want to access resources (aka scopes)
        //public static IEnumerable<Client> GetClients() {
        //    // client credentials client
        //}
    }

}

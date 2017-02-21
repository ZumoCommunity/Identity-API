using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            }
        };

        public Task<Client> FindClientByIdAsync(string clientId) {
            return Task.FromResult(AllClients.FirstOrDefault(c => c.ClientId == clientId));
        }
    }


    internal class IdentityServerData
    {
        public static IEnumerable<Client> GetAllClients() {
            return new List<Client> {
                new Client {
                    ClientId = "oauthClient",
                    ClientName = "Example Client Credentials Client Application",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = new List<Secret> {
                        new Secret("superSecretPassword".Sha256())},
                    AllowedScopes = new List<string> {"customAPI.read"}
                }
            };
        }

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
                new ApiResource {
                    Name = "myAPIs",
                    DisplayName = "Custom API",
                    Description = "Custom API Access",
                    UserClaims = new List<string> {
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Role,
                        JwtClaimTypes.GivenName
                    },
                    ApiSecrets = new List<Secret> { new Secret(apiSecret) },
                    Scopes = new List<Scope> {
                        new Scope("myAPIs"),
                        new Scope("myAPIs.write")
                    }
                }
                //,new ApiResource(
                //    "myAPIs",                                       // Api resource name
                //    "My API Set #1",                                // Display name
                //    new[] { JwtClaimTypes.Name, JwtClaimTypes.Role, "office" }) // Claims to be included in access token

            };
        }
    }
}

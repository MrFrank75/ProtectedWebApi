using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer
{
    public class Config
    {
        // scopes define the API resources in your system
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("scope.readaccess", "Example API"),
                new ApiResource("scope.fullaccess", "Example API"),
                new ApiResource("YouCanActuallyDefineTheScopesAsYouLike", "Example API")
            };
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
        new Client
                    {
                        ClientId = "ClientIdThatCanOnlyRead",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,

                        ClientSecrets =
                        {
                            new Secret("secret1".Sha256())
                        },
                        AllowedScopes = { "scope.readaccess" }
                    },
        new Client
                    {
                        ClientId = "ClientIdWithFullAccess",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,

                        ClientSecrets =
                        {
                            new Secret("secret2".Sha256())
                        },
                        AllowedScopes = { "scope.fullaccess" }
                    }
            };
        }
    }

}

using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace MySound.Auth.IdentityServer
{
    public class Config
    {
        /// <summary>
        /// IdentityResourses - это ресурсы клиента. которые нужно защитить. Ресурсы будут входить в токен как claims
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        /// <summary>
        /// Api, к которым будет предоставляться доступ
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>()
            {
                new ApiResource("api1", "api1")
                {
                    // claims, которые будут входить в токен для данного api
                    UserClaims =
                    {
                        JwtClaimTypes.Role,
                        JwtClaimTypes.Name,
                        JwtClaimTypes.NickName,
                        JwtClaimTypes.Email
                    }
                }
            };
        }

        /// <summary>
        /// Клиенты, которые будут запрашивать токен
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client()
                {
                    ClientId = "browser",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    RequireClientSecret = false,
                    AllowedScopes =
                    {
                        "api1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email
                    },
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true
                    //RedirectUris = { "https://localhost44338/index.html" }
                }
            };
        }
    }
}
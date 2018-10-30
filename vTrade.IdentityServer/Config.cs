// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace vTrade.IdentityServer
{
    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Description = "role",
                    DisplayName = "role",
                    Name = "role",
                    UserClaims = new List<string>
                    {
                        "role"
                    },
                    Enabled = true,
                    Required = true
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("vTradeAPI", "vTrade API")
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients(List<string> validRedirects)
        {
            // client credentials client
            return new List<Client>
            {
                // OpenID Connect implicit flow client (MVC)
                new Client
                {
                    ClientId = "vTradeWeb",
                    ClientName = "vTradeWeb",
                    AllowedGrantTypes = GrantTypes.Hybrid,

                    RedirectUris = validRedirects,
                    PostLogoutRedirectUris = validRedirects,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "role"
                    },

                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenType = AccessTokenType.Jwt,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    EnableLocalLogin = true
                }
            };
        }
    }
}
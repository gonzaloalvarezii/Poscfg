// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServerAspNetIdentity
{
    public static class Config
    {
        /*public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };*/

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResource(
                        name: "openid",
                        userClaims: new[] { "sub" },
                        displayName: "Your user identifier"
                ){ Enabled=true,Required=true },
                new IdentityResource(
                        name: "profile",
                        userClaims: new[] { 
                                    "name",
                                    "family_name",
                                    "given_name",
                                    "middle_name",
                                    "nickname",
                                    "preferred_username",
                                    "profile",
                                    "picture",
                                    "website",
                                    "gender",
                                    "birthdate",
                                    "zoneinfo",
                                    "locale",
                                    "updated_at"
                         },
                        displayName: "User profile"       
                ){ Enabled=true, Emphasize=true, Description = "Your user profile information (first name, last name, etc.)"},
                new IdentityResource(
                        name: "roles",
                        userClaims: new[] { "role" },
                        displayName: "Roles"
                ){ Enabled=true },
                new IdentityResource(
                        name: "email",
                        userClaims: new[] { 
                                "email",
                                "email_verified"
                                },
                        displayName: "Your email address"
                ){Enabled=true, Emphasize=true},
                new IdentityResource(
                        name: "address",
                        userClaims: new[] { 
                                "address"
                                },
                        displayName: "Your address"
                ){Enabled=true, Emphasize=true},
                               
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
            
                // invoice API specific scopes
                new ApiScope(name: "terminals.read",   displayName: "Terminals, lectura."),
                new ApiScope(name: "terminals.write",    displayName: "Terminals, escritura."),
                new ApiScope(name: "terminals.delete",    displayName: "Terminals, eliminar."),

                // customer API specific scopes
                new ApiScope(name: "system.read",    displayName: "System, lectura."),
                new ApiScope(name: "system.write",  displayName: "System, escritura."),
                new ApiScope(name: "system.delete", displayName: "System, eliminar."),

                // shared scope
                new ApiScope(name: "manage", displayName: "Privilegios administrativos sobre System y Terminals."),
    
                new ApiScope("test"),

                new ApiScope( 
                    name: "webui_api", 
                    displayName: "webui_api",
                    userClaims: new [] {"role", "name"}
                ){Required=true}

                //new ApiScope("scope2"),
                //new ApiScope("api1", "My API"),
                //new ApiScope("api2", "My API 2")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "scope1" }
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:44300/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "scope2" }
                },
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "test", "openid", "profile" }
                },
                new Client
                {
                    ClientId = "handy",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("testapi".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "terminals.read", "system.read", "openid", "profile" }
                },
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    // where to redirect to after login
                    RedirectUris = { "https://localhost:5002/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServer4.IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Profile
                    }
                },
                new Client
                {
                    ClientId = "webui",
                    ClientName = "webui",
                    ClientUri = "http://localhost:9000",
                    ClientSecrets = { new Secret("KXVEDhPXI22NojUG8teMXWM9uS".Sha256()) },

                    ////AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowedGrantTypes = GrantTypes.Code,

                    RequirePkce=true,

                    // where to redirect to after login
                    RedirectUris = { "http://localhost:9000/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:9000/signout-callback-oidc" },

                    FrontChannelLogoutUri = "http://localhost:9000/signout-oidc",

                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "email",
                        "profile",
                        "roles"
                    }
                },
                new Client
                {
                    ClientId = "webui_api_swaggerui",
                    ClientName = "webui_api_swaggerui",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RedirectUris = {"http://localhost:9000/swagger/oauth2-redirect.html"},
                    AllowedScopes = new List<string>
                    {
                        "webui_api"
                    },
                    AllowAccessTokensViaBrowser=true
                }
            };

            public static IEnumerable<ApiResource> ApiResources =>
                new ApiResource[]
                {
                    new ApiResource("terminals", "Terminals API")
                    {
                        Scopes = { "terminals.read", "terminals.write", "terminals.delete", "manage" }
                    },

                    new ApiResource("system", "Customer API")
                    {
                        Scopes = { "system.read", "system.write", "system.delete", "manage" }
                    },

                    new ApiResource("webui_api","webui_api")
                    {
                        Scopes = { "webui_api"}
                    }

 
                };
            

    }
}
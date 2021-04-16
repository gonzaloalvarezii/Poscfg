using System.Collections.Generic;
using IdentityServerAdminUI.Admin.Configuration.Identity;

namespace IdentityServerAdminUI.Admin.Configuration.IdentityServer
{
    public class Client : global::IdentityServer4.Models.Client
    {
        public List<Claim> ClientClaims { get; set; } = new List<Claim>();
    }
}







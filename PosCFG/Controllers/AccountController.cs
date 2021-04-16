using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace PosCFG.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        
        public IActionResult Logout()
        {
            return new SignOutResult(new List<string> { "oidc", "Cookies" });
        }
    }
}
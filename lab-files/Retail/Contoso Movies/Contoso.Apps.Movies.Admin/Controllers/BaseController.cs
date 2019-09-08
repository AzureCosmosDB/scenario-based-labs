using System;
using System.Web.Mvc;

namespace Contoso.Apps.Movies.Admin.Controllers
{
    public class BaseController : Controller
    {
        public string DisplayName { get; set; }

        protected override void EndExecute(IAsyncResult asyncResult)
        {
            if (Request.IsAuthenticated)
            {
                var claimsIdentity = User.Identity as System.Security.Claims.ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    string displayNameClaim = "http://schemas.microsoft.com/identity/claims/displayname";
                    var claim = claimsIdentity.FindFirst(displayNameClaim);
                    if (claim != null)
                    {
                        DisplayName = claim.Value;
                    }
                }
                //DisplayName = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst("http://schemas.microsoft.com/identity/claims/displayname").Value;
            }
            base.EndExecute(asyncResult);
        }
    }
}
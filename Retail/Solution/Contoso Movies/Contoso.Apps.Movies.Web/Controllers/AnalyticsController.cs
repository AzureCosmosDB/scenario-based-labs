using Contoso.Apps.Common.Controllers;
using Contoso.Apps.Movies.Web.Models;
using System.Web.Mvc;


namespace Contoso.Apps.Movies.Controllers
{
    [AllowAnonymous]
    public class AnalyticsController : BaseController
     {
        public ActionResult User()
        {
            UserAnalyticsModel m = new UserAnalyticsModel();

            return View(m);
        }

        public ActionResult Content()
        {
            UserAnalyticsModel m = new UserAnalyticsModel();

            return View(m);
        }

    }
}
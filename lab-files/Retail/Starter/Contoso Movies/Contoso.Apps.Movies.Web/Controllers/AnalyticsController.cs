using Contoso.Apps.Common.Controllers;
using Contoso.Apps.Movies.Data.Models;
using Contoso.Apps.Movies.Logic;
using Contoso.Apps.Movies.Web.Controllers;
using Contoso.Apps.Movies.Web.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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
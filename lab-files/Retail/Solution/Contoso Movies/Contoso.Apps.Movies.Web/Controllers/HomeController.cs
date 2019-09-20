using Contoso.Apps.Common.Controllers;
using Contoso.Apps.Movies.Data.Models;
using Contoso.Apps.Movies.Logic;
using Contoso.Apps.Movies.Web.Controllers;
using Contoso.Apps.Movies.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace Contoso.Apps.Movies.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        public ActionResult Index() {
            
            var vm = new HomeModel();

            Contoso.Apps.Movies.Data.Models.User user = (Contoso.Apps.Movies.Data.Models.User)Session["User"];

            if (user != null)
            {
                vm.RecommendProductsBought = RecommendationHelper.GetViaFunction("assoc", user.UserId, 0);
                vm.RecommendProductsLiked = RecommendationHelper.GetViaFunction("collab", user.UserId, 0);
            }
            else
            {
                vm.RecommendProductsBought = RecommendationHelper.GetViaFunction("top", 0, 0);
            }

            return View(vm);
        }

        public ActionResult Login()
        {
            List<User> activeUsers = users.ToList();
            return View(activeUsers);
        }

        public ActionResult About() {
            

            return View();
        }

        public ActionResult Contact() {
            
            return View();
        }

    }
}
using Contoso.Apps.Common.Controllers;
using Contoso.Apps.Movies.Data.Logic;
using Contoso.Apps.Movies.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Contoso.Apps.Movies.Admin.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            //var orderId = 2;
            //var order = new Order();
            List<Order> orders = new List<Order>();
            using (var orderActions = new OrderActions(orders))
            {
                //order = orderActions.GetOrder(orderId);
                orders = orderActions.GetCompletedOrders();
            }

            var vm = new Models.HomeModel
            {
                DisplayName = base.DisplayName,
                Orders = orders
            };

            if (Request.IsAuthenticated)
            {
                var user = User.Identity.Name;
            }

            return View(vm);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            var vm = new Models.BaseModel
            {
                DisplayName = base.DisplayName
            };

            return View(vm);
        }

        public ActionResult Details(int Id)
        {
            var order = new Order();
            using (var orderActions = new OrderActions(orders.ToList()))
            {
                order = orderActions.GetOrder(Id);
            }

            var vm = new Models.DetailsModel
            {
                DisplayName = base.DisplayName,
                Order = order
            };

            return View(vm);
        }
    }
}
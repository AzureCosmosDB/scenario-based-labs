using AutoMapper;
using Contoso.Apps.Common;
using Contoso.Apps.Movies.Data.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Contoso.Apps.Movies.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);

            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            MovieHelper.ApiKey = ConfigurationManager.AppSettings["movieApiKey"];

            string endpointUrl = ConfigurationManager.AppSettings["dbConnectionUrl"];
            string authorizationKey = ConfigurationManager.AppSettings["dbConnectionKey"];
            string databaseId = ConfigurationManager.AppSettings["databaseId"];

            DbHelper.client = new CosmosClient(endpointUrl, authorizationKey);
            DbHelper.databaseId = databaseId;

            // Automapper configuration.
            // Products:
            Mapper.CreateMap<Data.Models.Item, Models.ProductModel>();
            Mapper.CreateMap<IList<Data.Models.Item>, IList<Models.ProductModel>>();
            // Product list (subset of full product data):
            Mapper.CreateMap<Data.Models.Item, Models.ProductListModel>();
            Mapper.CreateMap<IList<Data.Models.Item>, IList<Models.ProductListModel>>();
            // Cart Items:
            Mapper.CreateMap<Data.Models.CartItem, Models.CartItemModel>();
            Mapper.CreateMap<IList<Data.Models.CartItem>, IList<Models.CartItemModel>>();
            // Categories:
            Mapper.CreateMap<Data.Models.Category, Models.CategoryModel>();
            Mapper.CreateMap<IList<Data.Models.Category>, IList<Models.CategoryModel>>();
            // Orders:
            Mapper.CreateMap<Data.Models.Order, Models.OrderModel>();
            Mapper.CreateMap<IList<Data.Models.Order>, IList<Models.OrderModel>>();
            Mapper.CreateMap<Models.OrderModel, Data.Models.Order>();
        }
    }
}

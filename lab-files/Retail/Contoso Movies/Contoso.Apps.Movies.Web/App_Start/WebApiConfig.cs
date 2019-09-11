using Contoso.Apps.Movies.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.WebHost;

namespace Contoso.Apps.Movies.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Enable Cross-Origin Requests (CORS)
            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();

            //add session state
            var httpControllerRouteHandler = typeof(HttpControllerRouteHandler).GetField("_instance",
        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            if (httpControllerRouteHandler != null)
            {
                httpControllerRouteHandler.SetValue(null,
                    new Lazy<HttpControllerRouteHandler>(() => new SessionHttpControllerRouteHandler(), true));
            }

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
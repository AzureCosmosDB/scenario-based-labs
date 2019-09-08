using Contoso.Apps.PaymentGateway.Formatters;
using Contoso.Apps.PaymentGateway.Throttling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Contoso.Apps.PaymentGateway
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Enable CORS globally. Normally we'd restrict this to certain requesting domains.
            var corsAttr = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(corsAttr);

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Need the text/plain mime type formatter in order to accept parameters that are native strings.
            config.Formatters.Add(new TextPlainFormatter());

            // Implement our custom throttling handler to limit API method calls.
            // Specify the throttle store, max number of allowed requests within specified timespan,
            // and message displayed in the error response when exceeded.
            config.MessageHandlers.Add(new ThrottlingHandler(
                new InMemoryThrottleStore(),
                id => 20,
                TimeSpan.FromMinutes(1),
                "You have exceeded the maximum number of allowed calls. Please wait until after the cooldown period to try again."
            ));
        }
    }
}

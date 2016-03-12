using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Scrumfish.WebApi.v2.Filters;

namespace Scrumfish.WebApi.v2.ApiSite
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Filters.Add(new ClientCertificateAuthenticateFilter());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{message}",
                defaults: new { message = RouteParameter.Optional}
            );
        }
    }
}

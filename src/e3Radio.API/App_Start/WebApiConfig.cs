using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace e3Radio.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Removed api/ so I can put this app in api folder
            //routeTemplate: "api/{controller}/{id}",

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{type}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // force json
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.Indent = true;
        }
    }
}

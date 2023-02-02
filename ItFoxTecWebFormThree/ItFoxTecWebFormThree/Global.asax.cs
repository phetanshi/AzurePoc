using ItFoxTecWebFormThree.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Configuration;

namespace ItFoxTecWebFormThree
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            IdentityConfig.RegisterIdentity();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        void Application_BeginRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            if (app.Context.Request.Url.LocalPath.EndsWith("/"))
            {
                string defaultPage = ConfigurationManager.AppSettings["DefaultPage"];
                app.Context.RewritePath(string.Concat(app.Context.Request.Url.LocalPath, defaultPage));
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Icasie
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest(Object objectSender, EventArgs e)
        {
            HttpContext currentContext = HttpContext.Current;
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        FormsIdentity id = HttpContext.Current.User.Identity as FormsIdentity;
                        FormsAuthenticationTicket ticket = id.Ticket;
                        List<string> userData = new List<string>();
                        userData.Add(ticket.UserData);
                        HttpContext.Current.User = new GenericPrincipal(id, userData.ToArray());
                    }
                }
            }

        }
    }
}

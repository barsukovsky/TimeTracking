using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TimeTracking2
{
    public class RouteConfig
    {
        private static string yearExp = @"^[0-9]{4}$";
        private static string monthExp = @"^0?[1-9]|1[012]$";
        private static string nameExp = @"(?i)^[a-z0-9]{1,100}$";

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: null,
                url: "reports/{year}/{month}/{username}",
                defaults: new { controller = "Report", action = "Edit" },
                constraints: new { year = yearExp, month = monthExp, username = nameExp }
            );

            routes.MapRoute(
                name: null,
                url: "reports/{year}/{month}",
                defaults: new { controller = "Report", action = "FetchByYearAndMonth" },
                constraints: new { year = yearExp, month = monthExp }
            );

            routes.MapRoute(
                name: null,
                url: "reports/{year}/{username}",
                defaults: new { controller = "Report", action = "FetchByYearAndUser" },
                constraints: new { year = yearExp, username = nameExp }
            );

            routes.MapRoute(
                name: null,
                url: "reports/{month}/{username}",
                defaults: new { controller = "Report", action = "FetchByMonthAndUser" },
                constraints: new { month = monthExp, username = nameExp }
            );

            routes.MapRoute(
                name: null,
                url: "reports/{year}/",
                defaults: new { controller = "Report", action = "FetchByYear" },
                constraints: new { year = yearExp }
            );

            routes.MapRoute(
                name: null,
                url: "reports/{month}/",
                defaults: new { controller = "Report", action = "FetchByMonth" },
                constraints: new { month = monthExp }
            );

            routes.MapRoute(
                name: null,
                url: "reports/{username}/",
                defaults: new { controller = "Report", action = "FetchByUser" },
                constraints: new { username = nameExp }
            );

            routes.MapRoute(
                name: null,
                url: "reports",
                defaults: new { controller = "Report", action = "Index" }
            );

            routes.MapRoute(
                name: null,
                url: "new/{username}/",
                defaults: new { controller = "Report", action = "CreateForUser" },
                constraints: new { username = nameExp }
            );

            routes.MapRoute(
                name: null,
                url: "new",
                defaults: new { controller = "Report", action = "Create" }
            );

            routes.MapRoute(
                name: null,
                url: "delete",
                defaults: new { controller = "Report", action = "Delete" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );

            routes.MapRoute(
                name: null,
                url: "login",
                defaults: new { controller = "Account", action = "Login" }
            );

            routes.MapRoute(
                name: null,
                url: "logoff",
                defaults: new { controller = "Account", action = "LogOff" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );

            routes.MapRoute(
                name: null,
                url: "register",
                defaults: new { controller = "Account", action = "Register" }
            );

            routes.MapRoute(
                name: null,
                url: "changepassword",
                defaults: new { controller = "Account", action = "ChangePassword" }
            );

            routes.MapRoute(
                name: null,
                url: "staff/edit/{username}/",
                defaults: new { controller = "Account", action = "Edit" },
                constraints: new { username = nameExp }
            );

            routes.MapRoute(
                name: null,
                url: "staff/delete/",
                defaults: new { controller = "Account", action = "Delete" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );

            routes.MapRoute(
                name: null,
                url: "staff",
                defaults: new { controller = "Account", action = "Index" }
            );

            routes.MapRoute(
                name: null,
                url: "contacts",
                defaults: new { controller = "Home", action = "Contact" }
            );

            routes.MapRoute(
                name: null,
                url: "about",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: null,
                url: "",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}
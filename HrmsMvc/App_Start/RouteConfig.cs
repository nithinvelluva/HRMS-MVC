using System.Web.Mvc;
using System.Web.Routing;

namespace HrmsMvc
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "admin/users",
               "admin/users",
                new { controller = "Admin", action = "Users", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "user/profile",
                 "user/profile",
                new { controller = "User", action = "UserProfile", id = UrlParameter.Optional }
            );            

            routes.MapRoute(
                "login",
                 "login",
                new { controller = "Login", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "calendar",
                 "calendar",
                new { controller = "calendar", action = "calendar", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               "downloadTaskFile",
                "downloadTaskFile",
               new { controller = "calendar", action = "downloadTaskFile", id = UrlParameter.Optional }
           );
            routes.MapRoute(
                name: "hrms",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}

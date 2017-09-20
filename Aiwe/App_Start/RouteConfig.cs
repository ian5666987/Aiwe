using System.Web.Mvc;
using System.Web.Routing;

namespace Aiwe {
  public class RouteConfig {
    public static void RegisterRoutes(RouteCollection routes) {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute(
        name: "FinalErrorRoute",
        url: "Home/Error",
        defaults: new { controller = "Home", action = "Error" }
      );

      routes.MapRoute(
        name: "CommonRoute",
        url: "Common/{action}/{tablename}/{id}",
        defaults: new { controller = "Common", tablename = "NotFound", action = "Index", id = UrlParameter.Optional }
      );

      routes.MapRoute(
        name: "Default",
        url: "{controller}/{action}/{id}",
        defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}

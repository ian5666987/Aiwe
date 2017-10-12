using System.Web.Mvc;
using System.Web.Routing;

namespace Aiwe {
  public class RouteConfig {
    public static void RegisterRoutes(RouteCollection routes) {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute(
        name: "FinalErrorRoute",
        url: Aiwe.DH.MvcHomeControllerName + "/" + Aiwe.DH.ErrorActionName,
        defaults: new { controller = Aiwe.DH.MvcHomeControllerName, action = Aiwe.DH.ErrorActionName }
      );

      routes.MapRoute(
        name: "CommonRoute",
        url: Aiwe.DH.MvcCommonControllerName + "/{action}/{commonDataTableName}/{id}",
        defaults: new { controller = Aiwe.DH.MvcCommonControllerName, commonDataTableName = "NotFound", action = Aibe.DH.IndexActionName, id = UrlParameter.Optional }
      );

      routes.MapRoute(
        name: "Default",
        url: "{controller}/{action}/{id}",
        defaults: new { controller = Aiwe.DH.MvcHomeControllerName, action = Aibe.DH.IndexActionName, id = UrlParameter.Optional }
      );
    }
  }
}

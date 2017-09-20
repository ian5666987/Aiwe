using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Aibe.Helpers;
using System.Web.Http;

namespace Aiwe {
  public class MvcApplication : System.Web.HttpApplication {
    protected void Application_Start() {
      AreaRegistration.RegisterAllAreas();
      GlobalConfiguration.Configure(WebApiConfig.Register);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
      CryptographyHelper.Init();

#if DEBUG
      TableHelper.PrepareMetas();
#else
      string folderPath = Server.MapPath("~/Settings");
      TableHelper.DecryptMetaItems(folderPath); //get data from the files when not debugging
#endif
    }

    protected void Application_Error(object sender, EventArgs e) {      
      Exception exception = Server.GetLastError();
      string exStr = exception.ToString();
      LogHelper.Error("Global", "Final", null, null, null, null, null, exception.ToString());
      Server.ClearError();
      Response.RedirectToRoute("FinalErrorRoute");      
    }
  }
}

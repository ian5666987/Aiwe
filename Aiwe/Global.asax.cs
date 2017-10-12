using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Aibe.Helpers;
using Aiwe.Helpers;
using System.Web.Http;

namespace Aiwe {
  public class MvcApplication : System.Web.HttpApplication {
    protected void Application_Start() {
      AreaRegistration.RegisterAllAreas();
      GlobalConfiguration.Configure(WebApiConfig.Register);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
      CryptographyHelper.Init(ASTrio.DH.PredefinedKeyName, ASTrio.DH.PredefinedExtension, ASTrio.DH.PredefinedPassword);
      LCZ.Init(); //For localization

#if DEBUG
      AiweTableHelper.PrepareMetas();
#else
      string folderPath = Server.MapPath("~/Settings");
      AiweTableHelper.DecryptMetaItems(folderPath); //get data from the files when not debugging
#endif
    }

    protected void Application_Error(object sender, EventArgs e) {      
      Exception exception = Server.GetLastError();
      string exStr = exception.ToString();
      LogHelper.Error(Aibe.LCZ.W_Global, Aibe.LCZ.W_Final, null, null, null, null, null, exception.ToString());
      Server.ClearError();
      Response.RedirectToRoute("FinalErrorRoute");      
    }
  }
}

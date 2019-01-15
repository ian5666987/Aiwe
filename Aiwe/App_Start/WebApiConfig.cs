using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using System.Net.Http.Headers;

namespace Aiwe {
  public static class WebApiConfig {
    public static void Register(HttpConfiguration config) {
      // Web API configuration and services
      // Configure Web API to use only bearer token authentication.
      config.SuppressDefaultHostAuthentication();
      config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

      // Web API routes
      config.MapHttpAttributeRoutes();

      if (PH.IsJson) //until version 1.4.1.0, only supports JSON Web Api data type
        //To make the default as JSON instead of XML
        config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
      else { //for version 1.5.1.0 onwards, force to use XmlSerializer for XML data type since the default has been changed to DataContractSerializer
        var xml = config.Formatters.XmlFormatter;
        xml.UseXmlSerializer = true; //So that we will not use DataContractSerializer, but XmlSerializer instead
      }

      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );
    }
  }
}

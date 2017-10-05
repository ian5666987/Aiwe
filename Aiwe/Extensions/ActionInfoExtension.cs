using Aiwe.Models;
using System.Linq;
using System.Security.Principal;
using Extension.String;
using Aibe.Models.Core;

namespace Aiwe.Extensions {
  public static class ActionInfoExtension {

    public static bool IsAllowed(this ActionInfo actInfo, IPrincipal user) {
      return actInfo.Roles == null || actInfo.Roles.Count <= 0 || actInfo.Roles.Any(x => user.IsInRole(x)); //if user is found or is not defined
    }

    public static bool IsAllowed(this ActionInfo actInfo, ApplicationUser user, bool isWebApi = false) {
      if (actInfo.Roles == null || actInfo.Roles.Count <= 0)
        return true;
      bool allowed = actInfo.Roles.Any(x => x.EqualsIgnoreCase(user.WorkingRole) || x.EqualsIgnoreCase(user.AdminRole));
      if (isWebApi) {
        bool appliedToMobile = actInfo.Roles.Any(x => x.EqualsIgnoreCase(Aibe.DH.MobileAppRole));
        return allowed || appliedToMobile; //the relationship here is singularly OR
      }
      return allowed;
    }
  }
}
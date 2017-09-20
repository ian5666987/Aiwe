using Aiwe.Models;
using System.Linq;
using System.Security.Principal;
using Extension.String;

namespace Aibe.Models {
  public partial class ActionInfo : CommonBaseInfo {
    public bool IsAllowed(IPrincipal user) {
      return Roles == null || Roles.Count <= 0 || Roles.Any(x => user.IsInRole(x)); //if user is found or is not defined
    }

    public bool IsAllowed(ApplicationUser user, bool isWebApi = false) {
      if (Roles == null || Roles.Count <= 0)
        return true;
      bool allowed = Roles.Any(x => x.EqualsIgnoreCase(user.WorkingRole) || x.EqualsIgnoreCase(user.AdminRole));
      if (isWebApi) {
        bool appliedToMobile = Roles.Any(x => x.EqualsIgnoreCase(DH.MobileAppRole));
        return allowed || appliedToMobile; //the relationship here is singularly OR
      }
      return allowed;
    }
  }
}
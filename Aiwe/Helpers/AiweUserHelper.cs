using Aiwe.Models;
using Aiwe.Models.ViewModels;
using Extension.String;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Security.Principal;

namespace Aiwe.Helpers {
  public class AiweUserHelper {
    //TODO this one is project specific! Beware!
    public static void DeleteAllUserRoles(string id, ApplicationUserManager userManager, ApplicationDbContext context) {
      var rolesForUser = userManager.GetRoles(id);
      using (var transaction = context.Database.BeginTransaction())
        if (rolesForUser.Count() > 0)
          foreach (var item in rolesForUser.ToList())
            userManager.RemoveFromRole(id, item);
    }

    public static bool UserHasAdminRight(IPrincipal user) {
      return user != null && user.Identity != null && 
        user.Identity.IsAuthenticated && Aibe.DH.AdminRoles.Any(x => user.IsInRole(x));
    }

    public static bool UserHasAdminRight(ApplicationUser user) {
      return user != null && user.AdminRole != null && Aibe.DH.AdminRoles.Contains(user.AdminRole);
    }

    public static bool UserHasAdminRight(ApplicationUserViewModel user) {
      return user != null && user.AdminRole != null && Aibe.DH.AdminRoles.Contains(user.AdminRole);
    }

    public static bool UserHasMainAdminRight(IPrincipal user) {
      return user != null && user.Identity != null &&
        user.Identity.IsAuthenticated && Aibe.DH.MainAdminRoles.Any(x => user.IsInRole(x));
    }

    public static bool UserHasMainAdminRight(ApplicationUser user) {
      return user != null && user.AdminRole != null &&
        Aibe.DH.MainAdminRoles.Contains(user.AdminRole);
    }

    public static bool UserHasMainAdminRight(ApplicationUserViewModel user) {
      return user != null && user.AdminRole != null &&
        Aibe.DH.MainAdminRoles.Contains(user.AdminRole);
    }

    public static bool UserIsDeveloper(IPrincipal user) {
      return user != null && user.Identity != null &&
        user.Identity.IsAuthenticated && user.IsInRole(Aibe.DH.DevRole);
    }

    public static bool UserIsDeveloper(ApplicationUser user) {
      return user != null && user.AdminRole != null &&
        user.AdminRole.EqualsIgnoreCase(Aibe.DH.DevRole);
    }

    public static bool UserIsDeveloper(ApplicationUserViewModel user) {
      return user != null && user.AdminRole != null &&
        user.AdminRole.EqualsIgnoreCase(Aibe.DH.DevRole);
    }
  }
}
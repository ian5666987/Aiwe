using Aiwe.Models;
using Aiwe.Models.ViewModels;
using Extension.String;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    public static Dictionary<string, object> GetUserParameters (ApplicationUser user, string prefix) {
      Type type = user.GetType();
      PropertyInfo[] properties = type.GetProperties(); //TODO check how to exclude some parameters
      Dictionary<string, object> result = new Dictionary<string, object>();
      foreach (PropertyInfo property in properties) {
        if (defaultExcludedPropertyNames.Contains(property.Name)) //case insensitive
          continue;
        result.Add(prefix + property.Name, property.GetValue(user, null));
      }
      return result;
    }

    private static List<string> defaultExcludedPropertyNames = new List<string> {
      "Claims", "Logins", "Roles", //all these are collection
      "PasswordHash", "SecurityStamp", "Id", //all these are not to be displayed or used
    }; 
    public static Dictionary<string, object> GetUserParameters(IPrincipal userInput, string prefix) {
      ApplicationDbContext context = new ApplicationDbContext();
      ApplicationUser user = string.IsNullOrWhiteSpace(userInput.Identity.Name) ? null :
        context.Users.FirstOrDefault(x => x.UserName == userInput.Identity.Name); //cannot use EqualsIgnoreCase because of LINQ-to-Entities
      if (user == null)
        return null;
      return GetUserParameters(user, prefix);
    }
  }
}
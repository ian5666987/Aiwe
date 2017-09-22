using Microsoft.AspNet.Identity.EntityFramework;
using Aiwe.Models;
using Aiwe.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Aibe.Models.Core;
using Aibe.Helpers;

namespace Aiwe.Helpers {
  public class AiweViewHelper {
    public static IEnumerable<ApplicationUserViewModel> PrepareUserViewModels(int? page, IOrderedQueryable<ApplicationUser> users, dynamic viewBag) {
      NavDataModel navDataModel;
      List<ApplicationUser> filteredUsers = ViewHelper.PrepareFilteredModels(page, users, out navDataModel);
      viewBag.NavData = navDataModel;
      if (filteredUsers != null) {
        IEnumerable<ApplicationUserViewModel> userViewModels = filteredUsers
          .Select(x => new ApplicationUserViewModel(x));
        return userViewModels;
      }
      return null;
    }

    public static IEnumerable<RoleViewModel> PrepareRoleViewModels(int? page, IOrderedQueryable<IdentityRole> roles, dynamic viewBag) {
      NavDataModel navDataModel;
      List<IdentityRole> filteredRoles = ViewHelper.PrepareFilteredModels(page, roles, out navDataModel);
      viewBag.NavData = navDataModel;
      if (filteredRoles != null) {
        IEnumerable<RoleViewModel> userViewModels = filteredRoles
          .ToList()
          .Select(x => new RoleViewModel(x));
        return userViewModels;
      }
      return null;
    }
  }
}
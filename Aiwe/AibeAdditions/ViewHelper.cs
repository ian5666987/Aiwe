using Microsoft.AspNet.Identity.EntityFramework;
using Aiwe.Models;
using Aiwe.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Aibe.Helpers {
  public partial class ViewHelper {
    public static IEnumerable<ApplicationUserViewModel> PrepareUserViewModels(int? page, IOrderedQueryable<ApplicationUser> users, dynamic viewBag) {
      List<ApplicationUser> filteredUsers = PrepareFilteredModels<ApplicationUser>(page, users, viewBag);
      if (filteredUsers != null) {
        IEnumerable<ApplicationUserViewModel> userViewModels = filteredUsers
          .Select(x => new ApplicationUserViewModel(x));
        return userViewModels;
      }
      return null;
    }

    public static IEnumerable<RoleViewModel> PrepareRoleViewModels(int? page, IOrderedQueryable<IdentityRole> roles, dynamic viewBag) {
      List<IdentityRole> filteredRoles = PrepareFilteredModels<IdentityRole>(page, roles, viewBag);
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
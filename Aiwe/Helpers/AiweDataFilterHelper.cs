using Aibe.Models.Filters;
using Aiwe.Models;
using Aiwe.Models.Filters;
using Extension.String;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;

namespace Aiwe.Helpers {
  public class AiweDataFilterHelper {
    public static IQueryable<ApplicationUser> ApplyUserFilter(IQueryable<ApplicationUser> unfiltered, ApplicationUserFilter filter) {
      IQueryable<ApplicationUser> filtered = unfiltered;

      if (!string.IsNullOrWhiteSpace(filter.FullName))
        filtered = filtered
          .Where(x => x.FullName != null &&
            x.FullName.ToLower().Contains(filter.FullName.ToLower()));

      if (!string.IsNullOrWhiteSpace(filter.DisplayName))
        filtered = filtered
          .Where(x => x.DisplayName != null &&
            x.DisplayName.ToLower().Contains(filter.DisplayName.ToLower()));

      if (!string.IsNullOrWhiteSpace(filter.Email))
        filtered = filtered
          .Where(x => x.Email != null &&
            x.Email.ToLower().Contains(filter.Email.ToLower()));

      if (!string.IsNullOrWhiteSpace(filter.Team))
        filtered = filtered
          .Where(x => x.Team != null &&
            x.Team.ToLower().Contains(filter.Team.ToLower()));

      if (!string.IsNullOrWhiteSpace(filter.AdminRole))
        filtered = filtered
          .Where(x => x.AdminRole != null &&
            x.AdminRole.EqualsIgnoreCase(filter.AdminRole));

      if (!string.IsNullOrWhiteSpace(filter.WorkingRole))
        filtered = filtered
          .Where(x => x.WorkingRole != null &&
            x.WorkingRole.EqualsIgnoreCase(filter.WorkingRole));

      return filtered;
    }

    public static IQueryable<Team> ApplyTeamFilter(IQueryable<Team> unfiltered, TeamFilter filter) {
      IQueryable<Team> filtered = unfiltered;

      if (!string.IsNullOrWhiteSpace(filter.Name))
        filtered = filtered
          .Where(x => x.Name != null &&
            x.Name.ToLower().Contains(filter.Name.ToLower()));

      return filtered;
    }

    public static IQueryable<IdentityRole> ApplyRoleFilter(IQueryable<IdentityRole> unfiltered, RoleFilter filter) {
      IQueryable<IdentityRole> filtered = unfiltered;

      if (!string.IsNullOrWhiteSpace(filter.Name))
        filtered = filtered
          .Where(x => x.Name != null &&
            x.Name.ToLower().Contains(filter.Name.ToLower()));

      return filtered;
    }
  }
}

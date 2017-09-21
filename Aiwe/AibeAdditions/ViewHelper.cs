using Microsoft.AspNet.Identity.EntityFramework;
using Aiwe.Models;
using Aiwe.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System;
using Aibe.Models.Core;

namespace Aibe.Helpers {
  public partial class ViewHelper {
    public static List<T> PrepareFilteredModels<T>(int? page, IOrderedQueryable<T> filtereds, dynamic viewBag) {
      List<T> filteredModels = null;
      int itemsPerPage;
      bool resultParse = int.TryParse(ConfigurationManager.AppSettings["itemsPerPage"], out itemsPerPage);
      if (!resultParse)
        itemsPerPage = 20; //default
      int queryCount = 0;
      if (filtereds.Any()) {
        ViewPerPageQueryResult<T> result = ViewHelper.ProcessView(page, filtereds, itemsPerPage);
        filteredModels = result.Results.ToList();
        queryCount = result.QueryCount;
        itemsPerPage = result.ItemsPerPage;
      }
      int pageValue = page.HasValue ? page.Value : 1;
      int maxPage = ((int)queryCount + itemsPerPage - 1) / itemsPerPage;
      int currentPage = Math.Max(Math.Min(pageValue, maxPage), 1);
      viewBag.NavData = new NavDataModel(currentPage, itemsPerPage, queryCount);
      return filteredModels;
    }

    public static IEnumerable<ApplicationUserViewModel> PrepareUserViewModels(int? page, IOrderedQueryable<ApplicationUser> users, dynamic viewBag) {
      List<ApplicationUser> filteredUsers = ViewHelper.PrepareFilteredModels<ApplicationUser>(page, users, viewBag);
      if (filteredUsers != null) {
        IEnumerable<ApplicationUserViewModel> userViewModels = filteredUsers
          .Select(x => new ApplicationUserViewModel(x));
        return userViewModels;
      }
      return null;
    }

    public static IEnumerable<RoleViewModel> PrepareRoleViewModels(int? page, IOrderedQueryable<IdentityRole> roles, dynamic viewBag) {
      List<IdentityRole> filteredRoles = ViewHelper.PrepareFilteredModels<IdentityRole>(page, roles, viewBag);
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
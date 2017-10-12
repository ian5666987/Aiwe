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

//public static IQueryable<MetaItem> ApplyMetaFilter(IQueryable<MetaItem> unfiltered, MetaFilter filter) {
//  IQueryable<MetaItem> filtered = unfiltered;

//  if (!string.IsNullOrWhiteSpace(filter.TableName))
//    filtered = filtered
//      .Where(x => x.TableName != null &&
//        x.TableName.ToLower().Contains(filter.TableName.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.DisplayName))
//    filtered = filtered
//      .Where(x => x.DisplayName != null &&
//        x.DisplayName.ToLower().Contains(filter.DisplayName.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.OrderBy))
//    filtered = filtered
//      .Where(x => x.OrderBy != null &&
//        x.OrderBy.ToLower().Contains(filter.OrderBy.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.ActionList))
//    filtered = filtered
//      .Where(x => x.ActionList != null &&
//        x.ActionList.ToLower().Contains(filter.ActionList.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.DefaultActionList))
//    filtered = filtered
//      .Where(x => x.DefaultActionList != null &&
//        x.DefaultActionList.ToLower().Contains(filter.DefaultActionList.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.TableActionList))
//    filtered = filtered
//      .Where(x => x.TableActionList != null &&
//        x.TableActionList.ToLower().Contains(filter.TableActionList.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.DefaultTableActionList))
//    filtered = filtered
//      .Where(x => x.DefaultTableActionList != null &&
//        x.DefaultTableActionList.ToLower().Contains(filter.DefaultTableActionList.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.TextFieldColumns))
//    filtered = filtered
//      .Where(x => x.TextFieldColumns != null &&
//        x.TextFieldColumns.ToLower().Contains(filter.TextFieldColumns.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.PictureColumns))
//    filtered = filtered
//      .Where(x => x.PictureColumns != null &&
//        x.PictureColumns.ToLower().Contains(filter.PictureColumns.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.IndexShownPictureColumns))
//    filtered = filtered
//      .Where(x => x.IndexShownPictureColumns != null &&
//        x.IndexShownPictureColumns.ToLower().Contains(filter.IndexShownPictureColumns.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.RequiredColumns))
//    filtered = filtered
//      .Where(x => x.RequiredColumns != null &&
//        x.RequiredColumns.ToLower().Contains(filter.RequiredColumns.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.NumberLimitColumns))
//    filtered = filtered
//      .Where(x => x.NumberLimitColumns != null &&
//        x.NumberLimitColumns.ToLower().Contains(filter.NumberLimitColumns.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.RegexCheckedColumns))
//    filtered = filtered
//      .Where(x => x.RegexCheckedColumns != null &&
//        x.RegexCheckedColumns.ToLower().Contains(filter.RegexCheckedColumns.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.RegexCheckedColumnExamples))
//    filtered = filtered
//      .Where(x => x.RegexCheckedColumnExamples != null &&
//        x.RegexCheckedColumnExamples.ToLower().Contains(filter.RegexCheckedColumnExamples.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.UserRelatedFilters))
//    filtered = filtered
//      .Where(x => x.UserRelatedFilters != null &&
//        x.UserRelatedFilters.ToLower().Contains(filter.UserRelatedFilters.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.ColumnExclusionList))
//    filtered = filtered
//      .Where(x => x.ColumnExclusionList != null &&
//        x.ColumnExclusionList.ToLower().Contains(filter.ColumnExclusionList.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.FilterExclusionList))
//    filtered = filtered
//      .Where(x => x.FilterExclusionList != null &&
//        x.FilterExclusionList.ToLower().Contains(filter.FilterExclusionList.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.DetailsExclusionList))
//    filtered = filtered
//      .Where(x => x.DetailsExclusionList != null &&
//        x.DetailsExclusionList.ToLower().Contains(filter.DetailsExclusionList.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.CreateEditExclusionList))
//    filtered = filtered
//      .Where(x => x.CreateEditExclusionList != null &&
//        x.CreateEditExclusionList.ToLower().Contains(filter.CreateEditExclusionList.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.AccessExclusionList))
//    filtered = filtered
//      .Where(x => x.AccessExclusionList != null &&
//        x.AccessExclusionList.ToLower().Contains(filter.AccessExclusionList.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.ColoringList))
//    filtered = filtered
//      .Where(x => x.ColoringList != null &&
//        x.ColoringList.ToLower().Contains(filter.ColoringList.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.FilterDropDownLists))
//    filtered = filtered
//      .Where(x => x.FilterDropDownLists != null &&
//        x.FilterDropDownLists.ToLower().Contains(filter.FilterDropDownLists.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.CreateEditDropDownLists))
//    filtered = filtered
//      .Where(x => x.CreateEditDropDownLists != null &&
//        x.CreateEditDropDownLists.ToLower().Contains(filter.CreateEditDropDownLists.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.PrefixesOfColumns))
//    filtered = filtered
//      .Where(x => x.PrefixesOfColumns != null &&
//        x.PrefixesOfColumns.ToLower().Contains(filter.PrefixesOfColumns.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.PostfixesOfColumns))
//    filtered = filtered
//      .Where(x => x.PostfixesOfColumns != null &&
//        x.PostfixesOfColumns.ToLower().Contains(filter.PostfixesOfColumns.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.ListColumns))
//    filtered = filtered
//      .Where(x => x.ListColumns != null &&
//        x.ListColumns.ToLower().Contains(filter.ListColumns.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.TimeStampColumns))
//    filtered = filtered
//      .Where(x => x.TimeStampColumns != null &&
//        x.TimeStampColumns.ToLower().Contains(filter.TimeStampColumns.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.HistoryTable))
//    filtered = filtered
//      .Where(x => x.HistoryTable != null &&
//        x.HistoryTable.ToLower().Contains(filter.HistoryTable.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.HistoryTrigger))
//    filtered = filtered
//      .Where(x => x.HistoryTrigger != null &&
//        x.HistoryTrigger.ToLower().Contains(filter.HistoryTrigger.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.AutoGeneratedColumns))
//    filtered = filtered
//      .Where(x => x.AutoGeneratedColumns != null &&
//        x.AutoGeneratedColumns.ToLower().Contains(filter.AutoGeneratedColumns.ToLower()));

//  if (filter.ItemsPerPageFrom.HasValue || filter.ItemsPerPageTo.HasValue) {
//    int from = filter.ItemsPerPageFrom.HasValue ? filter.ItemsPerPageFrom.Value : 0;
//    int to = filter.ItemsPerPageTo.HasValue ? filter.ItemsPerPageTo.Value : int.MaxValue;
//    filtered = filtered
//      .Where(x => x.ItemsPerPage >= from && x.ItemsPerPage <= to);
//  }

//  if (!string.IsNullOrWhiteSpace(filter.DisableFilter)) {
//    if (filter.DisableFilter.EqualsIgnoreCase(Aibe.DH.True)) {
//      filtered = filtered.Where(x => x.DisableFilter.Value);
//    } else {
//      filtered = filtered.Where(x => !x.DisableFilter.Value);
//    }
//  }

//  if (!string.IsNullOrWhiteSpace(filter.ColumnSequence))
//    filtered = filtered
//      .Where(x => x.ColumnSequence != null &&
//        x.ColumnSequence.ToLower().Contains(filter.ColumnSequence.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.ColumnAliases))
//    filtered = filtered
//      .Where(x => x.ColumnAliases != null &&
//        x.ColumnAliases.ToLower().Contains(filter.ColumnAliases.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.EditShowOnlyColumns))
//    filtered = filtered
//      .Where(x => x.EditShowOnlyColumns != null &&
//        x.EditShowOnlyColumns.ToLower().Contains(filter.EditShowOnlyColumns.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.ScriptConstructorColumns))
//    filtered = filtered
//      .Where(x => x.ScriptConstructorColumns != null &&
//        x.ScriptConstructorColumns.ToLower().Contains(filter.ScriptConstructorColumns.ToLower()));

//  if (!string.IsNullOrWhiteSpace(filter.ScriptColumns))
//    filtered = filtered
//      .Where(x => x.ScriptColumns != null &&
//        x.ScriptColumns.ToLower().Contains(filter.ScriptColumns.ToLower()));

//  return filtered;
//}
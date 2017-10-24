using Aibe.Models;
using Aibe.Models.Core;
using Aiwe.Extensions;
using Extension.String;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Aiwe.Models {
  public class AiweBaseTableModel : BaseTableModel {
    public AiweBaseTableModel(MetaInfo meta, Dictionary<string, string> stringDictionary) : base(meta, stringDictionary) { }

    public string GetListColumnDetailsHTML(string columnName, string dataValue) {
      if (Meta.ListColumns == null || Meta.ListColumns.Count <= 0 || string.IsNullOrWhiteSpace(dataValue))
        return null;

      ListColumnInfo info = Meta.GetListColumnInfo(columnName);
      if (info == null)
        return null;

      return info.GetDetailsHTML(dataValue);
    }

    protected bool IsColumnIncluded(List<ExclusionInfo> exclusions, string columnName, IPrincipal user) {
      if (exclusions == null || exclusions.Count <= 0) //if there is no column exclusion, then the column is definitely included
        return true;
      ExclusionInfo exInfo = exclusions.FirstOrDefault(x => x.Name.EqualsIgnoreCaseTrim(columnName));
      if (exInfo == null) //non specified column exclusion is allowed
        return true;
      //explicitly excluded if the column is specified, without any roles specified
      bool isExplicitlyExcluded = exInfo.Roles == null || !exInfo.Roles.Any() || exInfo.Roles.Any(x => user.IsInRole(x));
      return !isExplicitlyExcluded; //if user is not explicitly excluded, then he is definitely included.
    }

    public virtual bool IsColumnIncludedInIndex(string columnName, IPrincipal user) {
      //if (AiweUserHelper.UserHasMainAdminRight(user)) //if user is in main admin rights, it is always true
      //  return true;
      return IsColumnIncluded(Meta.ColumnExclusions, columnName, user);
    }

    public virtual bool IsColumnIncludedInCsv(string columnName, IPrincipal user) {
      return IsColumnIncluded(Meta.CsvExclusions, columnName, user);
    }

    public virtual List<string> GetExcludedColumnsInCsv(List<string> columnNames, IPrincipal user) {
      return columnNames
        .Where(x => !IsColumnIncludedInCsv(x, user))
        .ToList();
    }
  }
}
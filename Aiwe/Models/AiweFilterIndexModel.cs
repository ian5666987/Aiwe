using System.Collections.Generic;
using System.Linq;
using Extension.String;
using System.Data;
using System.Security.Principal;
using Aibe.Models;
using Aibe.Models.Core;
using Aiwe.Helpers;

namespace Aiwe.Models {
  //To be used to display filter and index
  public class AiweFilterIndexModel : AiweBaseTableModel {
    private IPrincipal user { get; set; }
    public DataTable Table { get; private set; }
    public bool HasTable { get { return Table != null; } }
    public FilterIndexModel FiModel { get; private set; }

    //: this (meta, userInput, model.Data, model.NavData) 
    //AiweFilterIndexModel does not have dictionaryString since eachColumn will have its own "dictionaryString"
    public AiweFilterIndexModel (MetaInfo meta, IPrincipal userInput, FilterIndexModel model, 
      Dictionary<string, string> stringDictionary) : base(meta, stringDictionary){
      FiModel = model;
      user = userInput;
      Table = model.Data;
      NavData = model.NavData;

      if (Table == null || meta == null)
        return;

      //Handle columns
      List<DataColumn> columns = new List<DataColumn>();
      foreach (DataColumn column in Table.Columns)
        columns.Add(column);
      var arrangedDataColumns = meta.GetColumnSequenceFor(columns);
      ColumnInfos = arrangedDataColumns.Select(x => meta.CreateColumnInfo(
          x, IsColumnIncludedInIndex(x.ColumnName, user), IsColumnIncludedInFilter(x.ColumnName, user),
          IsColumnForcelyIncludedInFilter(x.ColumnName, user)
        )).ToList();
      FilterColumns = ColumnInfos.Where(x => x.IsFilterIncluded)
        .Select(x => x.Column).ToList();

      //Handle rows
      RowNo = Table.Rows.Count;
      IndexRows.Clear();
      foreach (DataRow row in Table.Rows)
        IndexRows.Add(row);

      //Specific filter label portion usage
      List<string> filterColumnNames = FilterColumns.Select(x => Meta.GetColumnDisplayName(x.ColumnName)).ToList();
      List<string> filterDateTimeColumnNames = FilterColumns.Where(x =>
        x.DataType.ToString().Substring(Aibe.DH.SharedPrefixDataType.Length).EqualsIgnoreCase(Aibe.DH.DateTimeDataType))
        .Select(x => Meta.GetColumnDisplayName(x.ColumnName)).ToList();
      List<string> filterNumberColumnNames = FilterColumns.Where(x =>
        Aibe.DH.NumberDataTypes.Contains(x.DataType.ToString().Substring(Aibe.DH.SharedPrefixDataType.Length)))
        .Select(x => Meta.GetColumnDisplayName(x.ColumnName)).ToList();

      //TODO, currently all these are hardcoded, not put in DH
      FilterLabelPortion = filterColumnNames.Any(x => x.Length >= 26) ||
        filterNumberColumnNames.Any(x => x.Length >= 16) || filterDateTimeColumnNames.Any(x => x.Length >= 23) ? 5 : 4;
    }

    //Table related add info (combined usage)
    public List<ColumnInfo> ColumnInfos { get; private set; } = new List<ColumnInfo>();
    //Index usage
    public int RowNo { get; private set; }
    public List<DataRow> IndexRows { get; private set; } = new List<DataRow>();

    //Taken directly from Meta
    public List<DropDownInfo> DropDowns { get { return Meta.FilterDropDowns; } }

    //Filter usage, just for nice look (adjustable label portion)!
    public List<DataColumn> FilterColumns { get; private set; } = new List<DataColumn>();
    public int FilterLabelPortion { get; private set; }

    //Displays
    public NavDataModel NavData { get; set; }

    public bool IsColumnIncludedInFilter(string columnName, IPrincipal user, bool isWebApi = false) {
      if (AiweUserHelper.UserHasMainAdminRight(user)) //if user is in main admin rights, it is always true
        return true;
      return IsColumnIncluded(Meta.FilterExclusions, columnName, user);
    }

    public bool IsColumnForcelyIncludedInFilter(string columnName, IPrincipal user) {
      InclusionInfo inInfo = Meta.GetForcedFilterColumn(columnName);
      if (inInfo == null) //not specified, means it is not forced
        return false;
      return inInfo.Roles == null || !inInfo.Roles.Any() || inInfo.Roles.Any(x => user.IsInRole(x));
    }

    public bool IsActionAllowed(string actionName, IPrincipal user, bool isWebApi = false) {
      if (AiweUserHelper.UserHasMainAdminRight(user)) //if user is in main admin rights, it is always true
        return true;
      ActionInfo acInfo = Meta.Actions.FirstOrDefault(x => x.Name.EqualsIgnoreCaseTrim(actionName));
      if (acInfo == null) //such action is not found, then it is definitely false
        return false;
      bool isExplicitlyAllowed = acInfo.Roles.Any(x => user.IsInRole(x));
      return acInfo.Roles == null || acInfo.Roles.Count <= 0 || isExplicitlyAllowed; //if it is explicitly allowed or there isn't role specified, then it is true
    }

    public bool IsTableActionAllowed(string tableActionName, IPrincipal user, bool isWebApi = false) {
      if (AiweUserHelper.UserHasMainAdminRight(user)) //if user is in main admin rights, it is always true
        return true;
      ActionInfo acInfo = Meta.TableActions.FirstOrDefault(x => x.Name.EqualsIgnoreCaseTrim(tableActionName));
      if (acInfo == null) //such action is not found, then it is definitely false
        return false;
      bool isExplicitlyAllowed = acInfo.Roles.Any(x => user.IsInRole(x));
      return acInfo.Roles == null || acInfo.Roles.Count <= 0 || isExplicitlyAllowed; //if it is explicitly allowed or there isn't role specified, then it is true
    }

    public bool IsAllowedToCallAction(string actionName) {
      return IsActionAllowed(actionName, user);
    }

    public bool IsAllowedToCallTableAction(string tableActionName) {
      return IsTableActionAllowed(tableActionName, user);
    }
  }
}
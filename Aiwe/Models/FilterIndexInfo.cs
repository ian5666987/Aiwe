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
  public class FilterIndexInfo : BaseTableInfo {
    private IPrincipal user { get; set; }
    public DataTable Table { get; private set; }
    public bool HasTable { get { return Table != null; } }

    public FilterIndexInfo (MetaInfo meta, IPrincipal userInput, FilterIndexModel model) : 
      this (meta, userInput, model.Data, model.NavData) { }

    public FilterIndexInfo (MetaInfo meta, IPrincipal userInput, DataTable tableInput, NavDataModel navData) : base(meta) {
      user = userInput;
      Table = tableInput;
      NavData = navData;

      if (Table == null || meta == null)
        return;

      columnNo = Table.Columns.Count;
      RowNo = Table.Rows.Count;
      ColumnInfos.Clear();

      IndexExcludedColumnNos.Clear();
      int count = 0;

      FilterColumns.Clear(); //filter
      //List<DataColumn> columns = new List<DataColumn>();
      //foreach (DataColumn column in Table.Columns)
      //  columns.Add(column);

      //var arrangedDataColumns = meta.GetColumnSequenceFor(columns);

      foreach (DataColumn column in Table.Columns) {
        //Index
        bool isColumnIncluded = IsColumnIncludedInIndex(column.ColumnName, user);
        bool isPictureColumn = meta.IsPictureColumn(column.ColumnName);
        ColumnInfo columnInfo = new ColumnInfo(column) { Name = column.ColumnName, DisplayName = meta.GetColumnDisplayName(column.ColumnName) };
        columnInfo.IsIndexIncluded = isColumnIncluded;
        columnInfo.IsIndexShowImage = isPictureColumn && meta.IsIndexShownPictureColumn(column.ColumnName);
        if (columnInfo.IsIndexShowImage)
          columnInfo.ImageWidth = meta.GetImageWidth(column.ColumnName);
        columnInfo.IsListColumn = meta.IsListColumn(column.ColumnName);

        if (meta.Colorings != null)
          columnInfo.Colorings = meta.Colorings;

        if (!columnInfo.IsIndexIncluded)
          IndexExcludedColumnNos.Add(count);
        count++;

        //Filter
        //picture link columns cannot be used as filter (naturally)
        if (isColumnIncluded && IsColumnIncludedInFilter(column.ColumnName, user) && !isPictureColumn) {
          FilterColumns.Add(column); //only things which are not excluded by column, by picture link, and by filters can be filtered
          columnInfo.IsFilterIncluded = true;
        }

        //All
        ColumnInfos.Add(columnInfo);
      }

      //Specific index usage
      IndexRows.Clear();
      foreach (DataRow row in Table.Rows)
        IndexRows.Add(row);

      //Specific filter usage
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
    private int columnNo;
    public int RowNo { get; private set; }
    public List<DataRow> IndexRows { get; private set; } = new List<DataRow>();
    public List<int> IndexExcludedColumnNos { get; private set; } = new List<int>();

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

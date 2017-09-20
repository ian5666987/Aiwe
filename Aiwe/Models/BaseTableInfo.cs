using System;
using System.Linq;
using Extension.String;
using Aibe.Helpers;
using Aibe.Models;
using Aiwe.Extensions;
using System.Security.Principal;

namespace Aiwe.Models {
  public class BaseTableInfo {
    public MetaInfo Meta { get; private set; }

    public BaseTableInfo(MetaInfo meta) {
      if (meta == null)
        throw new ArgumentNullException("MetaInfo", "MetaInfo cannot be null");
      this.Meta = meta;
    }

    //taken directly from meta
    public string TableName { get { return Meta.TableName; } }
    public string TableDisplayName { get { return Meta.TableDisplayName; } }
    //public List<ListColumnInfo> ListColumns { get { return Meta.ListColumns; } }

    //public bool IsListColumn(string columnName) {
    //  return ListColumns != null && ListColumns.Count > 0 && 
    //    ListColumns.Any(x => x.Name.EqualsIgnoreCaseTrim(columnName));
    //}

    ////Format: ServiceType=default|RefTableName:RefColumnName:RefOtherColumnName:ServiceStatus
    ////there are two columns on the same table:
    //// -> this column
    //// -> changed Column name
    ////The goal is to know if the changedColumn affect the column
    ////if it changes to column, then the changed column must be at the last of the column
    //public bool IsListColumnAffectedBy(string columnName, string changedColumnName) {
    //  if (string.IsNullOrWhiteSpace(columnName) || string.IsNullOrWhiteSpace(changedColumnName))
    //    return false;
    //  return ListColumns != null && ListColumns
    //    .Where(x => x.IsValid && x.RefInfo != null && !string.IsNullOrWhiteSpace(x.RefInfo.CrossTableCondColumn))
    //    .Any(x => x.Name.EqualsIgnoreCaseTrim(columnName) &&
    //      x.RefInfo.CrossTableCondColumn.EqualsIgnoreCaseTrim(changedColumnName));
    //}

    ////Must contain keyword "check" in the column description
    //public bool IsListColumnOfType(string columnName, string type) {
    //  return ListColumns != null && ListColumns.Count > 0 && ListColumns.Any(
    //    x => x.Name.EqualsIgnoreCaseTrim(columnName) && x.ListType.EqualsIgnoreCaseTrim(type));
    //}

    //public string GetListColumnType(string columnName) {
    //  return ListColumns != null && ListColumns.Count > 0 && ListColumns.Any(
    //      x => x.Name.EqualsIgnoreCase(columnName)) ?
    //    ListColumns.FirstOrDefault(x => x.Name.EqualsIgnoreCaseTrim(columnName))?.ListType : null;
    //}

    //public bool ListColumnHasStaticTemplate(string columnName) {
    //  return ListColumns != null && ListColumns.Count > 0 && ListColumns.Any(
    //    x => x.RefInfo != null && //RInfo cannot be null
    //    !string.IsNullOrWhiteSpace(x.RefInfo.CondColumn) && //The condition column also cannot be null
    //    !string.IsNullOrWhiteSpace(x.RefInfo.StaticCrossTableCondColumn)); //lastly, there must be static value
    //}

    //public string GetColumnStaticTemplate(string columnName) {
    //  string dataValue = string.Empty;
    //  ListColumnInfo info = GetListColumnInfo(columnName);
    //  if (info == null)
    //    return null;
    //  bool result = info.GetRefDataValue(null, null, out dataValue);
    //  return result ? dataValue : null;
    //}

    //public ListColumnInfo GetListColumnInfo(string columnName) {
    //  return ListColumns.FirstOrDefault(x => x.Name.EqualsIgnoreCaseTrim(columnName));
    //}

    public string GetListColumnDetailsHTML(string columnName, string dataValue) {
      if (Meta.ListColumns == null || Meta.ListColumns.Count <= 0 || string.IsNullOrWhiteSpace(dataValue))
        return null;

      ListColumnInfo info = Meta.GetListColumnInfo(columnName);
      if (info == null)
        return null;

      return info.GetDetailsHTML(dataValue);
    }

    //public bool IsPictureColumn(string columnName) {
    //  return Meta.PictureColumns != null && Meta.PictureColumns.Count > 0 &&
    //    Meta.PictureColumns.Any(x => x.Name.EqualsIgnoreCaseTrim(columnName));
    //}

    //public bool IsIndexShownPictureColumn(string columnName) {
    //  return Meta.IndexShownPictureColumns != null && Meta.IndexShownPictureColumns.Count > 0 &&
    //    Meta.IndexShownPictureColumns.Any(x => x.EqualsIgnoreCaseTrim(columnName));
    //}

    public virtual bool IsColumnIncludedInIndex(string columnName, IPrincipal user, bool isWebApi = false) {
      if (UserHelper.UserHasMainAdminRight(user)) //if user is in main admin rights, it is always true
        return true;
      if (Meta.ColumnExclusions == null || Meta.ColumnExclusions.Count <= 0) //if there is no column exclusion, then the column is definitely included
        return true;
      ExclusionInfo exInfo = Meta.ColumnExclusions.FirstOrDefault(x => x.Name.EqualsIgnoreCaseTrim(columnName));
      if (exInfo == null) //non specified column exclusion is allowed
        return true;
      bool isExplicitlyExcluded = exInfo.Roles.Any(x => user.IsInRole(x));
      return !isExplicitlyExcluded; //if user is not explicitly excluded, then he is definitely included.
    }

    //public int GetImageWidth(string columnName) {
    //  PictureColumnInfo info = Meta.PictureColumns.FirstOrDefault(x => x.Name.EqualsIgnoreCaseTrim(columnName));
    //  return info == null ? PictureColumnInfo.DefaultWidth : info.Width;
    //}

    //public int GetTextFieldRowSize(string columnName) {
    //  TextFieldColumnInfo info = Meta.TextFieldColumns.FirstOrDefault(x => x.Name.EqualsIgnoreCaseTrim(columnName));
    //  return info == null ? TextFieldColumnInfo.DefaultRowSize : info.RowSize;
    //}
  }
}
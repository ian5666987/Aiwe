using System;
using System.Linq;
using Extension.String;
using Aibe.Models;
using Aibe.Models.Core;
using Aiwe.Extensions;
using Aiwe.Helpers;
using System.Security.Principal;
using System.Collections.Generic;
using System.Data;

namespace Aiwe.Models {
  public class BaseTableInfo {
    public MetaInfo Meta { get; private set; }

    public BaseTableInfo(MetaInfo meta, Dictionary<string, string> stringDictionary) {
      if (meta == null)
        throw new ArgumentNullException("MetaInfo", "MetaInfo cannot be null");
      this.Meta = meta;
      UnorderedData = stringDictionary;
      if (Meta.HasScriptColumn) //Now, based on this object dictionary, we can actually send it to the script column, if there is any
        ScTables = Meta.ScriptColumns.Select(x => new ScTableInfo(x, stringDictionary))
          .Where(x => x.IsValid).ToList();
      if (stringDictionary != null) //make sequence of data as long as this is not null
        SequencedItems = AiweTranslationHelper.SequenceDataFromStringDictionary(Meta.ColumnSequence, stringDictionary);      
    }

    public string GetData(string columnName, bool nullAllowed = true) {
      if (string.IsNullOrWhiteSpace(columnName))
        return null;
      //"Cid" must be "Equals"
      //only if columnName is "Cid" then we will return "0" instead of null
      return UnorderedData != null && UnorderedData.ContainsKey(columnName) ? UnorderedData[columnName] : columnName.Equals("Cid") ? "0" : nullAllowed ? null : string.Empty;
    }

    public string GetTime(string columnName, DateTime? dtVal) {
      if (UnorderedData != null && UnorderedData.ContainsKey(columnName)) //if it exists in the dictionary, take it from there first
        return UnorderedData[columnName];
      return dtVal.HasValue ? dtVal.Value.ToString("HH:mm:ss") : null; //otherwise, check if the dtVal has value. If it has, pass it, otherwise, leave it as null.
    }

    //taken directly from meta
    public string TableName { get { return Meta.TableName; } }
    public string TableDisplayName { get { return Meta.TableDisplayName; } }
    public List<KeyValuePair<string, string>> SequencedItems { get; private set; }
    public Dictionary<string, string> UnorderedData { get; private set; }

    //Only applied for ScriptColumns
    public List<ScTableInfo> ScTables { get; set; } = new List<ScTableInfo>();
    //public Dictionary<string, DataTable> ScTables { get; set; } = new Dictionary<string, DataTable>();

    public ScTableInfo GetScTable(string columnName) {
      if (ScTables == null || !ScTables.Any(x => x.ScInfo.Name.EqualsIgnoreCase(columnName)))
        return null;
      return ScTables.FirstOrDefault(x => x.ScInfo.Name.EqualsIgnoreCase(columnName));
    }

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
      if (AiweUserHelper.UserHasMainAdminRight(user)) //if user is in main admin rights, it is always true
        return true;
      return IsColumnIncluded(Meta.ColumnExclusions, columnName, user);
    }
  }
}
using System.Collections.Generic;
using System.Linq;
using Aibe.Helpers;
using Aibe.Models;
using Aibe.Models.Core;

namespace Aiwe.Helpers {
  public class AiweDropDownHelper {
    public static List<string> CreateLiveCreateEditDropDownFor(string tableName, string tableColumn, object originalValue,
      string dataType = "number", Dictionary<string, DropdownPassedArguments> passedColumnsAndValues = null) {
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);

      if (meta == null || meta.CreateEditDropDowns == null || !meta.CreateEditDropDowns.Any())
        return null; //fails to enumerate, please handle without dropdown

      //This is to get "Info1" string
      DropDownInfo dropDownInfo = meta.GetCreateEditDropDownColumnInfo(tableColumn);
      if (dropDownInfo == null || dropDownInfo.Items == null || !dropDownInfo.Items.Any())
        return null;

      //This is to process 1,2,3,[RInfo1],[RInfo2],... to distinguish between "Item" and "TableValued"
      if (dropDownInfo.Items.Any(x => !x.IsItem)) //table-valued
        return DropDownHelper.GetDropDownStringsFor(dropDownInfo, originalValue?.ToString(), dataType, filterApplied: true, passedColumnsAndValues: passedColumnsAndValues);
      return DropDownHelper.GetDropDownStringsFor(dropDownInfo, originalValue?.ToString(), dataType);
    }

    public static List<string> GetStaticCreateEditDropDownFor(string tableName, string tableColumn, string originalValue, string dataType = "number") {
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);

      if (meta == null || meta.CreateEditDropDowns == null || !meta.CreateEditDropDowns.Any())
        return null; //fails to enumerate, please handle without dropdown

      DropDownInfo dropDownInfo = meta.GetCreateEditDropDownColumnInfo(tableColumn);
      if (dropDownInfo == null || dropDownInfo.Items == null || !dropDownInfo.Items.Any())
        return null;

      return DropDownHelper.GetDropDownStringsFor(dropDownInfo, originalValue, dataType, filterApplied: false, passedColumnsAndValues: null);
    }

    public static List<string> GetStaticFilterDropDownFor(string tableName, string tableColumn, string dataType = "number") {
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);

      if (meta == null || meta.FilterDropDowns == null || !meta.FilterDropDowns.Any())
        return null; //fails to enumerate, please handle without dropdown

      DropDownInfo dropDownInfo = meta.GetFilterDropDownColumnInfo(tableColumn);
      if (dropDownInfo == null || dropDownInfo.Items == null || !dropDownInfo.Items.Any())
        return null;

      return DropDownHelper.GetDropDownStringsFor(dropDownInfo, null, dataType, filterApplied: false, passedColumnsAndValues: null);
    }
  }
}
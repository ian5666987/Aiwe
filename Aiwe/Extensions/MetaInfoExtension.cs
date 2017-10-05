//using Aibe.Models;
//using Aibe.Models.Core;
//using System.Linq;
//using System.Collections.Generic;

//namespace Aiwe.Extensions { //only for debugging
//  public static class MetaInfoExtension {

//    public static List<ListColumnResult> GetLiveListColumnResults(this MetaInfo minfo, string changedColumnName, string changedColumnValue) {
//      //for the given table and column, first find in the
//      List<ListColumnResult> results = new List<ListColumnResult>();
//      if (string.IsNullOrWhiteSpace(changedColumnValue))
//        return results; //return empty result if not possible to proccess further

//      var affectedColumnNames = minfo.ArrangedDataColumns
//        .Where(x => minfo.IsListColumn(x.ColumnName) && minfo.IsListColumnAffectedBy(x.ColumnName, changedColumnName))
//        .Select(x => x.ColumnName)
//        .ToList();

//      foreach (var affectedColumnName in affectedColumnNames) {
//        //I need to get the info from the affected columnName
//        ListColumnInfo info = minfo.GetListColumnInfo(affectedColumnName);
//        if (info == null || !info.IsValid)
//          continue;

//        ListColumnResult result = new ListColumnResult(affectedColumnName, null); //no need to assign data value here
//        if (result.UpdateLiveSubcolumnsDataValue(info, changedColumnName, changedColumnValue)) {
//          result.UsedListColumnInfo = info;
//          results.Add(result);
//        }
//      }

//      return results;
//    }
//  }
//}
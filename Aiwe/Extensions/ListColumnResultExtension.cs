//using Aibe.Models;
//using Aibe.Models.Core;
//using System.Linq;
//using System.Collections.Generic;

//namespace Aiwe.Extensions { //only for debugging
//  public static class ListColumnResultExtension {

//    public static bool UpdateLiveSubcolumnsDataValue(this ListColumnResult result, ListColumnInfo info, string changedColumnName, string changedColumnValue) {
//      result.IsSuccessful = false;
//      string newDataValue = string.Empty;
//      bool extractResult = info.GetRefDataValue(changedColumnName, changedColumnValue, out newDataValue);
//      if (!extractResult)
//        return false;
//      result.DataValue = newDataValue;
//      result.IsSuccessful = true; //This is going to be used by Javascript, so leave this be
//      return true; //not need to update IsSuccessfulHere
//    }
//  }
//}
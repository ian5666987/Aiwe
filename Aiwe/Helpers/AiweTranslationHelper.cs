using Extension.String;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Aiwe.Helpers {
  public class AiweTranslationHelper {
    public static Dictionary<string, string> FormCollectionToDictionary(FormCollection collections) {
      Dictionary<string, string> dict = new Dictionary<string, string>();
      foreach(var key in collections.AllKeys)
        dict.Add(key, collections[key]);
      return dict;
    }

    public static void FillModelStateWithErrorDictionary(ModelStateDictionary modelState, Dictionary<string, string> errorDict) {
      foreach(var item in errorDict) {
        if (!modelState.Keys.Contains(item.Key))//new item
          modelState.Add(item.Key, new ModelState());
        modelState.AddModelError(item.Key, item.Value);
      }
    }

    public static void FillTempDataFromCollections(TempDataDictionary tempData, Dictionary<string, string> collections, List<string> exclusionList) {
      tempData.Clear();
      foreach (var key in collections.Keys)
        if (exclusionList == null || !exclusionList.Contains(key))
          tempData.Add(key, collections[key]);
    }

    public static void FillTempDataFromObjectDictionary(List<string> columnSequence, TempDataDictionary tempData, Dictionary<string, object> objectDict) {
      tempData.Clear();
      if (columnSequence != null && columnSequence.Any()) {
        var arrangedColumns = columnSequence.Where(x => objectDict.ContainsKey(x));
        foreach (var item in arrangedColumns)
          tempData.Add(item, objectDict[item]);
        foreach (var key in objectDict.Keys.Except(arrangedColumns))
          tempData.Add(key, objectDict[key]);
      } else
        foreach (var key in objectDict.Keys)
          tempData.Add(key, objectDict[key]);
    }

    public static void FillTempDataFromDictionary(TempDataDictionary tempData, Dictionary<string, string> tempDataDict) {
      tempData.Clear();
      foreach (var key in tempDataDict.Keys)
        tempData.Add(key, tempDataDict[key]);
    }
  }
}

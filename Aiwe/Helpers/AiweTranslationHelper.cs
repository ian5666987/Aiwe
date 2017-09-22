using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

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

    public static void FillTempDataFromObjectDictionary(TempDataDictionary tempData, Dictionary<string, object> objectDict) {
      tempData.Clear();
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

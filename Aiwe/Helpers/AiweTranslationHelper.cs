using System.Collections.Generic;
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
  }
}

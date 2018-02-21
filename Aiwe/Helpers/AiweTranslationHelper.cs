using Aibe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Aiwe.Helpers {
  public class AiweTranslationHelper {
    public static Dictionary<string, string> FormCollectionToDictionary(FormCollection collections) {
      Dictionary<string, string> dict = new Dictionary<string, string>();
      foreach (var key in collections.AllKeys) {
        if (Aiwe.DH.IdentifierNames.Any(x => key.StartsWith(x)))
          continue;
        dict.Add(key, collections[key]);
      }
      return dict;
    }

    public static void FillModelStateWithErrorDictionary(ModelStateDictionary modelState, Dictionary<string, string> errorDict) {
      foreach (var item in errorDict) {
        if (!modelState.Keys.Contains(item.Key))//new item
          modelState.Add(item.Key, new ModelState());
        modelState.AddModelError(item.Key, item.Value);
      }
    }

    public static RouteValueDictionary GetRouteValuesForRedirection(string tableName, List<KeyValuePair<string, object>> identifiers) {
      RouteValueDictionary routeValues = new RouteValueDictionary();
      int index = 0;
      routeValues.Add(Aiwe.DH.CommonDataTableName, tableName);
      foreach (var identifier in identifiers) {
        routeValues.Add(Aiwe.DH.IdentifierKeyName + "[" + index + "]", identifier.Key);
        routeValues.Add(Aiwe.DH.IdentifierValueName + "[" + index + "]", identifier.Value);
        ++index;
      }
      return routeValues;
    }

    public static void AdjustModelState(ModelStateDictionary modelState, Dictionary<string, string> dictCollections) {
      List<KeyValuePair<string, ModelState>> removedItems = new List<KeyValuePair<string, ModelState>>();
      foreach (var item in modelState)
        if (Aiwe.DH.IdentifierNames.Any(x => item.Key.StartsWith(x)))
          removedItems.Add(item);
      foreach (var item in removedItems)
        modelState.Remove(item);
      foreach (var item in dictCollections)
        modelState.Add(item.Key, new ModelState());
    }

    public static List<KeyValuePair<string, object>> GetIdentifiers(MetaInfo meta, string[] identifierKeys, string[] identifierValues) {
      List<KeyValuePair<string, object>> identifiers = new List<KeyValuePair<string, object>>();
      if (identifierKeys == null || identifierValues == null)
        return identifiers;
      int maxLength = Math.Min(identifierKeys.Length, identifierValues.Length);
      for (int i = 0; i < maxLength; ++i) {
        object value = meta.GetGroupByColumnValueFromString(identifierKeys[i], identifierValues[i]);
        identifiers.Add(new KeyValuePair<string, object>(identifierKeys[i], value));
      }
      return identifiers;
    }

    public static string GetIdentifiersUrl(List<KeyValuePair<string, object>> identifiers) {
      StringBuilder frontSb = new StringBuilder();
      StringBuilder backSb = new StringBuilder();
      for (int i = 0; i < identifiers.Count; ++i) {
        if (i > 0) {
          frontSb.Append("&");
          backSb.Append("&");
        }
        var identifier = identifiers[i];
        frontSb.Append(string.Concat(Aiwe.DH.IdentifierKeyName, "=", identifier.Key));
        backSb.Append(string.Concat(Aiwe.DH.IdentifierValueName, "=", identifier.Value.ToString()));
      }
      string frontStr = frontSb.ToString();
      string backStr = backSb.ToString();
      return string.Concat(frontStr, string.IsNullOrWhiteSpace(backStr) ? string.Empty : "&", backStr);
    }

    public static string GetIdentifierColumnsUrl(List<string> identifierColumns) {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < identifierColumns.Count; ++i) {
        if (i > 0)
          sb.Append("&");
        var identifierColumn = identifierColumns[i];
        sb.Append(string.Concat(Aiwe.DH.IdentifierColumnName, "=", identifierColumn));
      }
      return sb.ToString();
    }
  }
}

using Extension.String;
using System;
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

    //public static void FillTempDataFromCollections(TempDataDictionary tempData, Dictionary<string, string> collections, List<string> exclusionList) {
    //  tempData.Clear();
    //  foreach (var key in collections.Keys)
    //    if (exclusionList == null || !exclusionList.Contains(key))
    //      tempData.Add(key, collections[key]);
    //}

    //SequencedData cannot use Dictionary, unfortunately
    public static List<KeyValuePair<string, string>> SequenceDataFromStringDictionary(List<string> columnSequence, Dictionary<string, string> stringDict) {
      List<KeyValuePair<string, string>> sequenceData = new List<KeyValuePair<string, string>>();
      if (columnSequence != null && columnSequence.Any()) {
        var arrangedColumns = columnSequence.Where(x => stringDict.ContainsKey(x));
        foreach (var item in arrangedColumns)
          sequenceData.Add(new KeyValuePair<string, string>(item, stringDict[item]));
        foreach (var key in stringDict.Keys.Except(arrangedColumns))
          sequenceData.Add(new KeyValuePair<string, string>(key, stringDict[key]));
      } else
        foreach (var key in stringDict.Keys)
          sequenceData.Add(new KeyValuePair<string, string>(key, stringDict[key]));
      return sequenceData;
    }

    //public static List<KeyValuePair<string, string>> SequenceDataFromObjectDictionary(List<string> columnSequence, Dictionary<string, object> objectDict) {
    //  List<KeyValuePair<string, string>> tempData = new List<KeyValuePair<string, string>>();
    //  if (columnSequence != null && columnSequence.Any()) {
    //    var arrangedColumns = columnSequence.Where(x => objectDict.ContainsKey(x));
    //    foreach (var item in arrangedColumns)
    //      tempData.Add(new KeyValuePair<string, string>(item, objectDict[item] == null ? string.Empty : objectDict[item].ToString()));
    //    foreach (var key in objectDict.Keys.Except(arrangedColumns))
    //      tempData.Add(new KeyValuePair<string, string>(key, objectDict[key] == null ? string.Empty : objectDict[key].ToString()));
    //  } else
    //    foreach (var key in objectDict.Keys)
    //      tempData.Add(new KeyValuePair<string, string>(key, objectDict[key] == null ? string.Empty : objectDict[key].ToString()));
    //  return tempData;
    //}

    //public static void FillTempDataFromObjectDictionary(List<string> columnSequence, TempDataDictionary tempData, Dictionary<string, object> objectDict) {
    //  tempData.Clear();
    //  if (columnSequence != null && columnSequence.Any()) {
    //    var arrangedColumns = columnSequence.Where(x => objectDict.ContainsKey(x));
    //    foreach (var item in arrangedColumns)
    //      tempData.Add(item, objectDict[item]);
    //    foreach (var key in objectDict.Keys.Except(arrangedColumns))
    //      tempData.Add(key, objectDict[key]);
    //  } else
    //    foreach (var key in objectDict.Keys)
    //      tempData.Add(key, objectDict[key]);
    //}

    //public static void FillTempDataFromDictionary(TempDataDictionary tempData, Dictionary<string, string> tempDataDict) {
    //  tempData.Clear();
    //  foreach (var key in tempDataDict.Keys)
    //    tempData.Add(key, tempDataDict[key]);
    //}

    public static Dictionary<string, string> ObjectDictionaryToStringDictionary(Dictionary<string, object> objectDictionary) {
      if (objectDictionary == null)
        return null;
      Dictionary<string, string> stringDictionary = new Dictionary<string, string>();
      foreach (var item in objectDictionary)
        stringDictionary.Add(item.Key, item.Value == null || item.Value is DBNull ? null : item.Value.ToString());
      return stringDictionary;
    }
  }
}

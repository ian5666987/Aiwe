using Aibe.Helpers;
using Aibe.Models;
using Aibe.Models.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Aiwe.Helpers {
  public class AiweForeignHelper { //v1.4.1.0
    //From dictCollections without foreign assignments to become dictCollections with foreign assignments
    public static void HandleForeignAssignment(MetaInfo meta, Dictionary<string, string> dictCollections) {
      var iterator = dictCollections.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToList();
      foreach (var item in iterator) {
        if (!meta.IsForeignInfoColumn(item.Key))  //only if it is a foreign info column we need to take care of it
          continue;
        ForeignInfoColumnInfo fi = meta.GetForeignInfoColumn(item.Key);
        if (fi == null || fi.IsFullForeignInfo || !fi.HasAnyAssignedColumn()) //full fi cannot be processed, fi without assigned column need not to be processed
          continue;
        var assignedDataDictionary = fi.GetAssignedDataDictionary(item.Value);
        foreach(var data in assignedDataDictionary) {
          if (!dictCollections.ContainsKey(data.Key))
            continue;
          dictCollections[data.Key] = data.Value?.ToString();
        }
      }
    }
  }
}

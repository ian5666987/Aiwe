using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Aibe.Helpers;
using Aibe.Models;
using Aibe.Models.DB;
using Extension.Cryptography;
using Extension.String;
using Aiwe.Models.DB;

namespace Aiwe.Helpers {
  public class AiweTableHelper {
    private static List<MetaItem> metaList = new List<MetaItem>();
    private static List<MetaInfo> metaInfoList = new List<MetaInfo>();
    private static IEnumerable<MetaItem> getMetaItems() {
#if DEBUG
      CoreDataModel db = new CoreDataModel();
      return db.MetaItems;
#else
      return metaList;
#endif
    }

    public static IEnumerable<MetaInfo> GetMetas() {
      return metaInfoList;
    }

    public static void PrepareMetas() {
      metaInfoList = getMetaItems().ToList().Select(x => new MetaInfo(x)).Where(x => x.IsValid).ToList();
    }

    public static void AddMeta(MetaInfo meta) {
      if (meta.IsValid)
        metaInfoList.Add(meta);
    }

    public static void DeleteMeta(string tableName) {
      MetaInfo removedItem = GetMeta(tableName);
      if (removedItem != null)
        metaInfoList.Remove(removedItem);
    }

    public static MetaInfo GetMeta(string tableName) {
      MetaInfo meta = GetMetas()
        .FirstOrDefault(x => x.TableName.EqualsIgnoreCase(tableName));
      return meta;
    }

    public static void UpdateMeta(MetaItem metaItem) {
      MetaInfo editedItem = GetMeta(metaItem.TableName);
      editedItem.AssignParameters(metaItem);
    }

    public static int DecryptMetaItems(string folderPath) {
      int count = 0;
      try {
        List<MetaItem> metaItems = Cryptography.DecryptoSerializeAll<MetaItem>(folderPath);
        count = metaItems.Count;

#if DEBUG
        CoreDataModel db = new CoreDataModel();
        foreach (var metaItem in metaItems)
          db.MetaItems.AddOrUpdate(metaItem);
        db.SaveChanges();
#else
        metaList.Clear();
        metaList.AddRange(metaItems);
#endif
        PrepareMetas();

      } catch (Exception ex){ //TODO do something for error log later
        LogHelper.Error(null, null, null, null, "Meta", "Decrypt", null, ex.ToString());
        throw ex;
      }
      return count;
    }
  }
}
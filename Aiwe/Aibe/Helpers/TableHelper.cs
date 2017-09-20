using System;
using System.Collections.Generic;
using System.Linq;
using Aibe.Models.DB;
using System.Data.Entity.Migrations;
using Extension.Cryptography;
using Aibe.Models;
using Extension.String;

namespace Aibe.Helpers {
  public class TableHelper {
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
      //try {
      metaInfoList = getMetaItems().ToList().Select(x => new MetaInfo(x)).Where(x => x.IsValid).ToList();
        //Console.WriteLine("Successful amount: " + metaInfoList.Count);
      //} catch (Exception exc) {
      //  Console.WriteLine(exc.ToString());
      //}
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

    //public static MetaItem GetMeta(string tableName) {
    //  MetaItem meta = GetMetas()
    //    .FirstOrDefault(x => x.TableName.EqualsIgnoreCase(tableName));
    //  return meta;
    //}

    public static MetaInfo GetMeta(string tableName) {
      MetaInfo meta = GetMetas()
        .FirstOrDefault(x => x.TableName.EqualsIgnoreCase(tableName));
      return meta;
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
        metaList.AddRange(metas);
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
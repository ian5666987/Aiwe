using System.Linq;
using System.Data.Entity.Migrations;
using Extension.Cryptography;
using Aibe.Models.DB;

namespace Aibe.Helpers {
  public partial class UserHelper {
    public static void CreateUserMap(CoreDataModel db, string userName, string password) {
      string encryptedPassword = Cryptography.Encrypt(password);

      CoreUserMap userMap = new CoreUserMap {
        UserName = userName,
        EncryptedPassword = encryptedPassword,
      };

      db.CoreUserMaps.Add(userMap);
      db.SaveChanges();
    }

    public static void DeleteUserMap(CoreDataModel db, string userName) {
      var userMaps = db.CoreUserMaps.Where(x => x.UserName == userName);
      db.CoreUserMaps.RemoveRange(userMaps);
      db.SaveChanges();
    }

    public static void EditUserMapName(CoreDataModel db, string userName, string newUserName) {
      var userMaps = db.CoreUserMaps.Where(x => x.UserName == userName);
      foreach (var userMap in userMaps) {
        userMap.UserName = newUserName;
        db.CoreUserMaps.AddOrUpdate(userMap);
      }
      db.SaveChanges();
    }

    public static void SetUserMapPassword(CoreDataModel db, string userName, string newPassword) {
      var userMaps = db.CoreUserMaps.Where(x => x.UserName == userName);
      foreach (var userMap in userMaps) {
        userMap.EncryptedPassword = Cryptography.Encrypt(newPassword);
        db.CoreUserMaps.AddOrUpdate(userMap);
      }
      db.SaveChanges();
    }

    
    public static bool AuthenticateUser(CoreDataModel db, string logType, string userName, string password) {
      var userMap = db.CoreUserMaps.FirstOrDefault(x => x.UserName == userName);
      if (userMap == null) {
        LogHelper.Access(userName, logType, "Not found");
        return false; //not found
      }
      string decryptedPassword = Cryptography.Decrypt(userMap.EncryptedPassword);
      if (password != decryptedPassword) {
        LogHelper.Access(userName, logType, "Password does not match");
        return false; //not found
      }
      return true;
    }
  }
}
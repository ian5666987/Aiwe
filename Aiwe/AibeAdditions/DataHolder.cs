using System.Collections.Generic;

namespace Aiwe {
  public class DH { //can be extended as wanted
    //User related global items
    public const string SharedDevFullName = "Astrio Developers";
    public const string SharedDevName = "developer@astriotech.com";
    public const string SharedDevPass = "A5tr10T3ch";
    public const string MainAdminName = "admin@feinmetall.com";
    public const string MainAdminPass = "Adm1n15tr@t0r";
    public const string AdminAuthorizedRoles = Aibe.DH.AdminRole + "," + Aibe.DH.MainAdminRole + "," + Aibe.DH.DevRole;
    public readonly static List<string> WorkingRoles = new List<string> { "User", "Supervisor", "Manager" }; //TODO not in the right place, but move it here first for now...
    public readonly static List<string> DeveloperNames = new List<string> { Aibe.DH.DevName, SharedDevName };

    //Website requests
    public const string GetManyRequest = "GETMANY";
    public const string GetRequest = "GET";
    public const string PostRequest = "POST";
    public const string PutRequest = "PUT";
    public const string DeleteRequest = "DELETE";

    //Translations
    public readonly static Dictionary<string, string> RequestToActionDict = new Dictionary<string, string> {
      { GetManyRequest, Aibe.DH.IndexActionName },
      { GetRequest, Aibe.DH.DetailsActionName },
      { PostRequest, Aibe.DH.CreateActionName },
      { PutRequest, Aibe.DH.EditActionName },
      { DeleteRequest, Aibe.DH.DeleteActionName }
    };

    //Special items
    public readonly static List<string> OnlyAccessCheckingActions = new List<string> {
      Aibe.DH.IndexActionName,
      "GetLiveDropdownItems", "GetLiveSubcolumns", "GetSubcolumnItems", "UpdateSubcolumnItemsDescription"
    };
  }
}
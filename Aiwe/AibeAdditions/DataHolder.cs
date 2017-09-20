using System.Collections.Generic;

namespace Aibe {
  public partial class DH { //can be extended as wanted
    //User related global items
    public const string SharedDevFullName = "Astrio Developers";
    public const string SharedDevName = "developer@astriotech.com";
    public const string SharedDevPass = "A5tr10T3ch";
    public const string MainAdminName = "admin@feinmetall.com";
    public const string MainAdminPass = "Adm1n15tr@t0r";
    public const string AdminAuthorizedRoles = AdminRole + "," + MainAdminRole + "," + DevRole;
    public readonly static List<string> WorkingRoles = new List<string> { "User", "Supervisor", "Manager" }; //TODO not in the right place, but move it here first for now...
    public readonly static List<string> DeveloperNames = new List<string> { DevName, SharedDevName };

    //Website requests
    public const string GetManyRequest = "GETMANY";
    public const string GetRequest = "GET";
    public const string PostRequest = "POST";
    public const string PutRequest = "PUT";
    public const string DeleteRequest = "DELETE";

    //Translations
    public readonly static Dictionary<string, string> RequestToActionDict = new Dictionary<string, string> {
      { GetManyRequest, IndexActionName },
      { GetRequest, DetailsActionName },
      { PostRequest, CreateActionName },
      { PutRequest, EditActionName },
      { DeleteRequest, DeleteActionName }
    };

    //Special items
    public readonly static List<string> OnlyAccessCheckingActions = new List<string> {
      IndexActionName,
      "GetLiveDropdownItems", "GetLiveSubcolumns", "Getsubcolumnitems", "UpdateSubcolumnItemsDescription"
    };
  }
}
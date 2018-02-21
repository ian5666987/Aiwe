using System.Collections.Generic;

namespace Aiwe {
  public class DH { //can be extended as wanted
    //User related global items
    public const string SharedDevFullName = "Astrio Developers";
    public const string SharedDevName = "developer";
    public const string SharedDevEmail = "developer@astriotech.com";
    public const string SharedDevPass = "A5tr10T3ch";
    public const string MainAdminName = "admin";
    public const string MainAdminEmail = "admin@companyname.com";
    public const string MainAdminPass = "Adm1n15tr@t0r";
    public const string AdminAuthorizedRoles = Aibe.DH.AdminRole + "," + Aibe.DH.MainAdminRole + "," + Aibe.DH.DevRole;
    public readonly static List<string> WorkingRoles = new List<string> { "User", "Supervisor", "Manager" }; //TODO not in the right place, but move it here first for now...
    public readonly static List<string> DeveloperEmails = new List<string> { Aibe.DH.DevEmail, SharedDevEmail };

    //Website requests
    public const string GetManyRequest = "GETMANY";
    public const string GetRequest = "GET";
    public const string PostRequest = "POST";
    public const string PutRequest = "PUT";
    public const string DeleteRequest = "DELETE";

    //Website related
    public const string JsonType = "application/json";
    public const string WebApi = "Web Api";
    public const string Mvc = "MVC";
    public const string ValuesActionFilterError = "ValuesActionFilterError";
    public const string Request = "request";
    public const string WebApiControllerName = "Values";
    public const string Id = "Id";
    public const string MvcCommonControllerName = "Common";
    public const string MvcMetaControllerName = "MetaItem";
    public const string MvcHomeControllerName = "Home";
    public const string MvcTeamControllerName = "Team";
    public const string MvcUserControllerName = "User";
    public const string MvcRoleControllerName = "Role";
    public const string MvcAdminControllerName = "Admin";
    public const string MvcAccountControllerName = "Account";
    public const string MvcManageControllerName = "Manage";
    public const string UserDisplayName = "DisplayName";
    public const string SuccessActionName = "Success";
    public const string LogInActionName = "Login";
    public const string LogOffActionName = "LogOff";
    public const string ErrorLocalActionName = "ErrorLocal";
    public const string ErrorActionName = "Error";
    public const string ErrorViewName = "Error";
    public const bool UseStrongCheck = true;
    public const bool IsTagChecked = true; //Web must actually check the tags
    public const string IndexDateTimeFormat = Aibe.DH.DefaultDateTimeFormat;
    public const string DetailsDateTimeFormat = Aibe.DH.DefaultDateTimeFormat;
    public const string ScTableDateTimeFormat = Aibe.DH.DefaultDateTimeFormat;
    public const string CsvDateTimeFormat = Aibe.DH.DefaultDateTimeFormat;

    //Website specifics
    public const string UserTableName = "AspNetUsers";
    public const string RoleTableName = "AspNetRoles";
    public const string IdentifierKeyName = "identifierKeys";
    public const string IdentifierValueName = "identifierValues";
    public const string IdentifierColumnName = "identifierColumns";
    public const string NonCreateIdParName = "id";
    public const string CommonDataTableName = "commonDataTableName";
    public readonly static List<string> IdentifierNames = new List<string> {
      IdentifierKeyName, IdentifierValueName, IdentifierColumnName,
    };
    public const string DeleteGroupActualActionName = "DeleteGroupActual";

    //Table
    public const string TableModelClassPrefix = "Aiwe.Models.DB.";

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
      "GetLiveDropDownItems", "GetLiveSubcolumns", "GetSubcolumnItems", "UpdateSubcolumnItemsDescription"
    };
  }
}
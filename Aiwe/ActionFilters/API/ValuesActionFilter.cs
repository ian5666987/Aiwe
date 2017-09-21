using Aibe.Helpers;
using Aibe.Models.DB;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using Aiwe.Models.API;
using Aiwe.Models;
using Aiwe.Extensions;
using Extension.Cryptography;
using Extension.String;
using Aibe.Models;
using Aibe.Models.Core;

namespace Aiwe.ActionFilters {
  public partial class ValuesActionFilterAttribute : ActionFilterAttribute {
    ApplicationDbContext context = new ApplicationDbContext();
    CoreDataModel db = new CoreDataModel();

    //Probably worth to look at
    //http://stackoverflow.com/questions/38661090/token-based-authentication-in-web-api-without-any-user-interface
    public override void OnActionExecuting(HttpActionContext actionContext) {
      if (actionContext.ActionDescriptor.ActionName.EqualsIgnoreCase(Aibe.DH.AuthenticateActionName)) //authenticate action cannot have error here.
        return;

      string errorName = "ValuesActionFilterError";

      if (!actionContext.ModelState.IsValid) { //could probably be used to force the existence of tableName
        actionContext.Request.Properties.Add(new KeyValuePair<string, object>(errorName, ValuesActionFilterResult.ModelStateInvalid));
        return;
      }

      if (!actionContext.ActionArguments.ContainsKey("request")) {
        actionContext.Request.Properties.Add(new KeyValuePair<string, object>(errorName, ValuesActionFilterResult.RequestNotFound));
        return; //error, returning certain HttpResponse would be great
      }

      ClientApiRequest request = (ClientApiRequest)actionContext.ActionArguments["request"];

      if (request == null) {
        actionContext.Request.Properties.Add(new KeyValuePair<string, object>(errorName, ValuesActionFilterResult.NullRequest));
        return;
      }

      if (string.IsNullOrWhiteSpace(request.TableName)) {
        actionContext.Request.Properties.Add(new KeyValuePair<string, object>(errorName, ValuesActionFilterResult.TableNameUnspecified));
        return;
      }

      if (string.IsNullOrWhiteSpace(request.RequestType)) {
        actionContext.Request.Properties.Add(new KeyValuePair<string, object>(errorName, ValuesActionFilterResult.EmptyRequestType));
        return;
      }

      MetaInfo meta = TableHelper.GetMeta(request.TableName);

      if (meta == null) {
        actionContext.Request.Properties.Add(new KeyValuePair<string, object>(errorName, ValuesActionFilterResult.TableDescriptionNotFound));
        return;
      }

      string encryptedPassword = Cryptography.Encrypt(request.Password);

      CoreUserMap userMap = string.IsNullOrWhiteSpace(request.UserName) ? null :
        db.CoreUserMaps.FirstOrDefault(x => x.UserName.ToLower().Trim() == request.UserName.ToLower().Trim() &&
          x.EncryptedPassword == encryptedPassword); //password must be case insensitive and not trimmed, certainly

      //only if userMap is available the user is considered available
      ApplicationUser user = string.IsNullOrWhiteSpace(request.UserName) || userMap == null ? null :
        context.Users.FirstOrDefault(x => x.UserName.ToLower().Trim() == request.UserName.ToLower().Trim());

      if (!UserHelper.UserIsDeveloper(user))
        LogHelper.Action(user.UserName, "Web Api", "Values", request.TableName, request.RequestType, request.CreateLogValue(3000)); //TODO as of now 3000 is hardcoded

      if (!Aiwe.DH.RequestToActionDict.Any(x => x.Key.EqualsIgnoreCase(request.RequestType))) {
        actionContext.Request.Properties.Add(new KeyValuePair<string, object>(errorName, ValuesActionFilterResult.InvalidRequestType));
        return;
      }

      if (UserHelper.UserHasMainAdminRight(user)) //handles null too
        return; //no need to check further for such user

      //Unlike action which is supposed to be case-insensitive, role is NOT relaxed in use of capital/small letters (case sensitive)
      if (meta.AccessExclusions != null) { //there is access exclusion and user role is supposed to be excluded
        if (meta.AccessExclusions.Any(x => x.EqualsIgnoreCase(Aibe.DH.AnonymousRole)) && user == null) { //unauthorized is excluded here and user is not authenticated
          actionContext.Request.Properties.Add(new KeyValuePair<string, object>(errorName, ValuesActionFilterResult.AnonymousUserNotAllowed));
          return;
        }

        if (meta.AccessExclusions.Any(x => x.EqualsIgnoreCase(Aibe.DH.MobileAppRole))) //special user role, mobile app must be present for any user in mobile app to be forbidden
          if (meta.AccessExclusions.Any(x => x.EqualsIgnoreCase(user.WorkingRole) || x.EqualsIgnoreCase(user.AdminRole))) {
            actionContext.Request.Properties.Add(new KeyValuePair<string, object>(errorName, ValuesActionFilterResult.InsufficientAccessRight));
            return;
          }
      }

      string actionName = Aiwe.DH.RequestToActionDict.FirstOrDefault(x => x.Key.EqualsIgnoreCase(request.RequestType)).Value;
      if (actionName.EqualsIgnoreCase(Aibe.DH.IndexActionName)) //Index action only needs as far as access checking
        return;

      if (meta.Actions == null || meta.Actions.Count <= 0) {
        actionContext.Request.Properties.Add(new KeyValuePair<string, object>(errorName, ValuesActionFilterResult.ActionNotFound));
        return;
      }

      if (!meta.Actions.Any(x => x.Name.EqualsIgnoreCase(actionName))) { //action not found in the list
        actionContext.Request.Properties.Add(new KeyValuePair<string, object>(errorName, ValuesActionFilterResult.ActionNotFound));
      } else {
        ActionInfo actionInfo = meta.Actions.FirstOrDefault(x => x.Name.EqualsIgnoreCase(actionName));
        if (!actionInfo.IsAllowed(user, isWebApi: true)) //check if action is not allowed, this is the only part where it uses "isWebApi" option
          actionContext.Request.Properties.Add(new KeyValuePair<string, object>(errorName, ValuesActionFilterResult.InsufficientAccessRightToAction));
      }
    }
  }

  public enum ValuesActionFilterResult {
    OK,
    ModelStateInvalid,
    RequestNotFound,
    NullRequest,
    TableNameUnspecified,
    TableDescriptionNotFound,
    AnonymousUserNotAllowed,
    InsufficientAccessRight,
    EmptyRequestType,
    InvalidRequestType,
    UnauthorizedRequestType,
    InsufficientAccessRightToAction,
    ActionNotFound,
  }
}

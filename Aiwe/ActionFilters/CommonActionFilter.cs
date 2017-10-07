using Extension.String;
using System;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Aibe.Helpers;
using Aibe.Models;
using Aibe.Models.Core;
using Aiwe.Models.DB;
using Aiwe.Extensions;
using Aiwe.Helpers;

namespace Aiwe.ActionFilters {
  public class CommonActionFilterAttribute : ActionFilterAttribute {
    private ActionResult redirectTo(string action, int? errorNo) {
      return new RedirectToRouteResult(
          new RouteValueDictionary(new { controller = Aiwe.DH.MvcHomeControllerName, action = action, errorNo = errorNo })
        );
    }

    CoreDataModel db = new CoreDataModel();
    public override void OnActionExecuting(ActionExecutingContext filterContext) {
      //Table validity checking (checked)
      object value = filterContext.ActionParameters
        .FirstOrDefault(x => x.Key.EqualsIgnoreCaseTrim(Aibe.DH.TableNameParameterName))
        .Value;
      string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
      string actionName = filterContext.ActionDescriptor.ActionName;
      string method = filterContext.HttpContext.Request.HttpMethod;
      object id = null, collections = null;
      if (filterContext.ActionParameters.ContainsKey("id"))
        id = filterContext.ActionParameters["Id"];
      if (filterContext.ActionParameters.ContainsKey("collections"))
        collections = filterContext.ActionParameters["collections"];
      string tableName = value?.ToString();

      IPrincipal user = HttpContext.Current.User;
      string userName = string.IsNullOrWhiteSpace(user.Identity.Name) ? string.Empty : user.Identity.Name;

      if (!AiweUserHelper.UserIsDeveloper(user) && //the developer does not need log
        !Aibe.DH.NonRecordedActions.Any(x => x.EqualsIgnoreCase(actionName)) && //the action must not be excluded for record
        method != null &&
        method.EqualsIgnoreCase(Aiwe.DH.PostRequest) && //only post is recorded
          (string.IsNullOrWhiteSpace(tableName) || 
          !Aibe.DH.CoreTableNames.Contains(tableName))) { //the table name is either null or it is not excluded

        StringBuilder sb = new StringBuilder();

        if (actionName.EqualsIgnoreCase(Aibe.DH.DeleteActionName))
          sb.Append("Id: " + id?.ToString());
        if (collections != null && 
          (actionName.EqualsIgnoreCase(Aibe.DH.CreateActionName) ||
          actionName.EqualsIgnoreCase(Aibe.DH.EditActionName))) {
          FormCollection colls = (FormCollection)collections;
          if (colls != null) {
            int index = 0;
            foreach (var key in colls.AllKeys) {
              if (index > 0)
                sb.Append(Environment.NewLine);
              sb.Append(string.Concat(key, ": ", colls[key]));
              ++index;
            }
          }
        }

        LogHelper.Action(userName, Aiwe.DH.Mvc, controllerName, tableName, actionName, sb.ToString());
      }

      if (value == null) { //table description not found, do nothing right now
        filterContext.Result = redirectTo("TableDescriptionNotFound", -1);
        return;
      }

      if (string.IsNullOrWhiteSpace(tableName)) {
        filterContext.Result = redirectTo("TableDescriptionNotFound", -2);
        return;
      }

      MetaInfo meta = AiweTableHelper.GetMeta(tableName);

      if (meta == null) {
        filterContext.Result = redirectTo("TableDescriptionNotFound", -3);
        return;
      }

      if (AiweUserHelper.UserHasMainAdminRight(user)) //main admins are immune to exclusion
        return;

      //Access checking (checked)
      if (meta.AccessExclusions != null) { //there is access exclusion and user role is supposed to be excluded
        if (meta.AccessExclusions.Contains(Aibe.DH.AnonymousRole) && !user.Identity.IsAuthenticated) { //unauthorized is excluded here and user is not authenticated
          filterContext.Result = redirectTo("InsufficientAccessRightPage", -1);
          return;
        }
        //TODO this User.IsInRole is case insensitive, unfortunately
        if (meta.AccessExclusions.Any(x => user.IsInRole(x))) { //user is in any of the exclusion authorization
          filterContext.Result = redirectTo("InsufficientAccessRightPage", -2);
          return;
        }
      }

      //Action and table action checking
      if (Aiwe.DH.OnlyAccessCheckingActions.Any(x => x.EqualsIgnoreCase(actionName))) //Index action only needs as far as access checking
        return;

      //Action checking (checked)
      if (meta.Actions != null && meta.Actions.Any(x => x.Name.EqualsIgnoreCase(actionName))) {
        ActionInfo action = meta.Actions.FirstOrDefault(x => x.Name.EqualsIgnoreCase(actionName));
        if (action.IsAllowed(user)) //if user is found or is not defined
          return; //then returns
        filterContext.Result = redirectTo("InsufficientAccessRightAction", -1); //action is registered but role is not found
      } //below onwards means actionNotFound

      //Table action checking (checked)
      if (meta.TableActions == null || meta.TableActions.Count <= 0) { //table action is null and action is not found
        filterContext.Result = redirectTo("ActionNotFound", -1);
        return;
      }

      if (!meta.TableActions.Any(x => x.Name.EqualsIgnoreCase(actionName))) { //action not found in the table list either
        filterContext.Result = redirectTo("ActionNotFound", -2);
        return;
      } //below means found in the table action

      ActionInfo tableActionInfo = meta.TableActions.FirstOrDefault(x => x.Name.EqualsIgnoreCase(actionName));
      if (!tableActionInfo.IsAllowed(user))
        filterContext.Result = redirectTo("InsufficientAccessRightAction", -2);
    }
  }
}
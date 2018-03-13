using Aibe.Helpers;
using Aiwe.Helpers;
using Aiwe.Models;
using Aiwe.Models.DB;
using Aiwe.Models.Filters;
using Aiwe.Models.ViewModels;
using Extension.String;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Aiwe.Controllers {
  [Authorize(Roles = Aiwe.DH.AdminAuthorizedRoles)]
  public class UserController : Controller {
    ApplicationDbContext context = new ApplicationDbContext();
    private ApplicationUserManager _userManager;

    public ApplicationUserManager UserManager {
      get {
        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
      }
      private set {
        _userManager = value;
      }
    }

    //EqualsIgnoreCase cannot be used here because it is LINQ to entity
    public ActionResult Index(int? page) {      
      var users = context.Users.Where(x => x.AdminRole != Aibe.DH.DevRole)
        //.ToList() //this may make EqualsIgnoreCase work, but wasting time in a way
        //.OrderByDescending(x => x.AdminRole.EqualsIgnoreCase(Aibe.DH.MainAdminRole))
        .OrderByDescending(x => x.AdminRole.ToLower() == Aibe.DH.MainAdminRole.ToLower())
        .ThenBy(x => x.FullName);

      IEnumerable<ApplicationUserViewModel> userViewModels = AiweViewHelper.PrepareUserViewModels(page, users, ViewBag);

      return userViewModels == null ? View() : View(userViewModels.ToList());
    }

    [HttpPost]
    public ActionResult Index(ApplicationUserFilter filter) {
      var unfiltereds = context.Users.Where(x => x.AdminRole != Aibe.DH.DevRole)
        .OrderByDescending(x => x.AdminRole.ToLower() == Aibe.DH.MainAdminRole.ToLower())
        .ThenBy(x => x.FullName);
      var filtereds = AiweDataFilterHelper.ApplyUserFilter(unfiltereds, filter);
      var unordereds = filtereds
        .OrderByDescending(x => x.AdminRole.ToLower() == Aibe.DH.MainAdminRole.ToLower())
        .ThenBy(x => x.FullName);
      ViewBag.Filter = filter;
      IEnumerable<ApplicationUserViewModel> results = AiweViewHelper.PrepareUserViewModels(filter.Page, unordereds, ViewBag);
      return results == null ? View() : View(results.ToList());
    }

    public ActionResult Create() {
      return View();
    }

    private RedirectToRouteResult redirectToError(string error) {
      return RedirectToAction(Aiwe.DH.ErrorLocalActionName, new { error = error });
    }

    public ActionResult ErrorLocal(string error) {
      ViewBag.Error = error;
      return View(Aiwe.DH.ErrorViewName);
    }

    CoreDataModel db = new CoreDataModel();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(ApplicationUserCreateViewModel model) {
      //try {
      if (string.IsNullOrWhiteSpace(model.AdminRole) && string.IsNullOrWhiteSpace(model.WorkingRole)) {
        ModelState.AddModelError(Aiwe.LCZ.I_WorkingRole, string.Format(Aiwe.LCZ.E_RolesCannotBeBothEmpty, Aiwe.LCZ.W_WorkingRole, Aiwe.LCZ.W_AdminRole));
        ModelState.AddModelError(Aiwe.LCZ.I_AdminRole, string.Format(Aiwe.LCZ.E_RolesCannotBeBothEmpty, Aiwe.LCZ.W_WorkingRole, Aiwe.LCZ.W_AdminRole));
      }

      if (ModelState.IsValid) {

        var now = DateTime.Now;
        var user = new ApplicationUser {
          UserName = model.Email,
          DisplayName = model.DisplayName,
          FullName = model.FullName,
          Email = model.Email,
          Team = model.Team,
          EmailConfirmed = true,
          RegistrationDate = now,
          LastLogin = now,
          AdminRole = model.AdminRole,
          WorkingRole = model.WorkingRole,
        };

        var result = UserManager.Create(user, model.Password);        

        if (result.Succeeded) {
          if (!string.IsNullOrWhiteSpace(model.WorkingRole) &&
            !Aibe.DH.AllowedAdminRoles.Any(x => x.EqualsIgnoreCase(model.WorkingRole))
            ) {
            var addRoleResult = UserManager.AddToRole(user.Id, model.WorkingRole);
            if (!addRoleResult.Succeeded)
              return RedirectToAction(Aiwe.DH.ErrorLocalActionName);
          }
          if (!string.IsNullOrWhiteSpace(model.AdminRole) &&
            Aibe.DH.AllowedAdminRoles.Any(x => x.EqualsIgnoreCase(model.AdminRole))) {
            var addRoleResult = UserManager.AddToRole(user.Id, model.AdminRole);
            if (!addRoleResult.Succeeded)
              return RedirectToAction(Aiwe.DH.ErrorLocalActionName);
          }

          UserHelper.CreateUserMap(model.Email, model.Password);

          return RedirectToAction(Aibe.DH.IndexActionName);
        }
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    public ActionResult Details(string id) {
      ApplicationUser user = UserManager.Users.FirstOrDefault(x => x.Id == id);
      if (user == null)
        return redirectToError(Aibe.LCZ.NFE_UserIdNotFound);
      ApplicationUserViewModel model = new ApplicationUserViewModel(user);
      if (model == null)
        return redirectToError(string.Format(Aibe.LCZ.E_FailToDoActionOnItemIn, Aibe.LCZ.W_Create, Aiwe.LCZ.W_ApplicationUserModel, Aibe.LCZ.W_Details));
      return View(model);
    }

    public ActionResult Delete(string id) {
      ApplicationUser user = UserManager.Users.FirstOrDefault(x => x.Id == id);
      if (user == null) 
        return redirectToError(Aibe.LCZ.NFE_UserIdNotFound);
      ApplicationUserViewModel model = new ApplicationUserViewModel(user);
      if (model == null)
        return redirectToError(string.Format(Aibe.LCZ.E_FailToDoActionOnItemIn, Aibe.LCZ.W_Create, Aiwe.LCZ.W_ApplicationUserModel, Aibe.LCZ.W_Delete));
      return View(model);
    }

    [HttpPost]
    [ActionName(Aibe.DH.DeleteActionName)]
    public ActionResult DeletePost(string id) {
      ApplicationUser user = UserManager.Users.FirstOrDefault(x => x.Id == id);
      if (user == null)
        return redirectToError(Aibe.LCZ.NFE_UserIdNotFound);      
      if (AiweUserHelper.UserHasMainAdminRight(user))
        return redirectToError(string.Format(Aibe.LCZ.E_CannotBeEditedOrDeleted, user.AdminRole));
      UserHelper.DeleteUserMap(user.UserName);
      var result = UserManager.Delete(user);
      if (!result.Succeeded) {
        StringBuilder sb = new StringBuilder(string.Format(Aibe.LCZ.E_FailToDoActionOnItem, Aibe.LCZ.W_Delete, Aibe.LCZ.W_User));
        sb.Append(".<br/>");
        sb.Append(Aibe.LCZ.W_ErrorList);
        sb.Append(":<br/>");
        sb.Append(string.Join("<br/>", result.Errors.ToArray()));
        return redirectToError(sb.ToString());
      }
      return RedirectToAction(Aibe.DH.IndexActionName);
    }

    public ActionResult Edit(string id) {
      ApplicationUser user = UserManager.Users.FirstOrDefault(x => x.Id == id);
      if (user == null)
        return redirectToError(Aibe.LCZ.NFE_UserIdNotFound);
      ApplicationUserEditViewModel model = new ApplicationUserEditViewModel(user);
      if (model == null)
        return redirectToError(string.Format(Aibe.LCZ.E_FailToDoActionOnItemIn, Aibe.LCZ.W_Create, Aiwe.LCZ.W_ApplicationUserModel, Aibe.LCZ.W_Edit));
      return View(model);
    }

    [HttpPost]
    public ActionResult Edit(ApplicationUserEditViewModel model) {
      ApplicationUser user = UserManager.Users.FirstOrDefault(x => x.Id == model.Id);
      if (string.IsNullOrWhiteSpace(model.AdminRole) && string.IsNullOrWhiteSpace(model.WorkingRole)) {
        ModelState.AddModelError(Aiwe.LCZ.I_WorkingRole, string.Format(Aiwe.LCZ.E_RolesCannotBeBothEmpty, Aiwe.LCZ.W_WorkingRole, Aiwe.LCZ.W_AdminRole));
        ModelState.AddModelError(Aiwe.LCZ.I_AdminRole, string.Format(Aiwe.LCZ.E_RolesCannotBeBothEmpty, Aiwe.LCZ.W_WorkingRole, Aiwe.LCZ.W_AdminRole));
      }

      if (!ModelState.IsValid)
        return View(model);
      if (user == null)
        return redirectToError(Aibe.LCZ.NFE_UserIdNotFound);
      if (AiweUserHelper.UserHasMainAdminRight(user))
        return redirectToError(string.Format(Aibe.LCZ.E_CannotBeEditedOrDeleted, user.AdminRole));

      string oldAdminRole = user.AdminRole;
      string oldWorkingRole = user.WorkingRole;
      string oldUserName = user.UserName;

      user.UserName = model.Email;
      user.DisplayName = model.DisplayName;
      user.FullName = model.FullName;
      user.Email = model.Email;
      user.Team = model.Team;
      user.EmailConfirmed = true;
      user.AdminRole = model.AdminRole;
      user.WorkingRole = model.WorkingRole;

      var result = UserManager.Update(user);
      if (!result.Succeeded) {
        StringBuilder sb = new StringBuilder(string.Format(Aibe.LCZ.E_FailToDoActionOnItem, Aibe.LCZ.W_Delete, Aibe.LCZ.W_User));
        sb.Append(".<br/>");
        sb.Append(Aibe.LCZ.W_ErrorList);
        sb.Append(":<br/>");
        sb.Append(string.Join("<br/>", result.Errors.ToArray()));
        return redirectToError(sb.ToString());
      }

      if (oldAdminRole != model.AdminRole) {
        if (!string.IsNullOrWhiteSpace(oldAdminRole)) //if old role is not empty, remove the old role
          UserManager.RemoveFromRole(user.Id, oldAdminRole);

        if (oldAdminRole != model.AdminRole && //old role is different from the current role
          !string.IsNullOrWhiteSpace(model.AdminRole) && //new role is not empty
          Aibe.DH.AllowedAdminRoles.Any(x => x.EqualsIgnoreCase(model.AdminRole))) { //just add new role
          var addRoleResult = UserManager.AddToRole(user.Id, model.AdminRole);
          if (!addRoleResult.Succeeded)
            return RedirectToAction(Aiwe.DH.ErrorLocalActionName);
        }
      }

      if (oldWorkingRole != model.WorkingRole) {
        if (!string.IsNullOrWhiteSpace(oldWorkingRole)) //if old role is not empty, remove the old role
          UserManager.RemoveFromRole(user.Id, oldWorkingRole);

        if (oldWorkingRole != model.WorkingRole && //old role is different from the current role
          !string.IsNullOrWhiteSpace(model.WorkingRole) && //new role is not empty
          !Aibe.DH.AllowedAdminRoles.Any(x => x.EqualsIgnoreCase(model.WorkingRole))
          ) { //just add new role
          var addRoleResult = UserManager.AddToRole(user.Id, model.WorkingRole);
          if (!addRoleResult.Succeeded)
            return RedirectToAction(Aiwe.DH.ErrorLocalActionName);
        }
      }

      UserHelper.EditUserMapName(oldUserName, model.Email);
      return RedirectToAction(Aibe.DH.IndexActionName);
    }
  }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Aiwe.Models;
using Aiwe.Models.ViewModels;
using Aiwe.Models.Filters;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Extension.String;
using Aibe;
using Aibe.Helpers;
using Aibe.Models.DB;

namespace Aiwe.Controllers {
  [Authorize(Roles = DH.AdminAuthorizedRoles)]
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
      var users = context.Users.Where(x => x.AdminRole != DH.DevRole)
        //.ToList() //this may make EqualsIgnoreCase work, but wasting time in a way
        //.OrderByDescending(x => x.AdminRole.EqualsIgnoreCase(DH.MainAdminRole))
        .OrderByDescending(x => x.AdminRole.ToLower() == DH.MainAdminRole.ToLower())
        .ThenBy(x => x.FullName);

      IEnumerable<ApplicationUserViewModel> userViewModels = ViewHelper.PrepareUserViewModels(page, users, ViewBag);

      return userViewModels == null ? View() : View(userViewModels.ToList());
    }

    [HttpPost]
    public ActionResult Index(ApplicationUserFilter filter) {
      var unfiltereds = context.Users.Where(x => x.AdminRole != DH.DevRole)
        .OrderByDescending(x => x.AdminRole.ToLower() == DH.MainAdminRole.ToLower())
        .ThenBy(x => x.FullName);
      var filtereds = DataFilterHelper.ApplyUserFilter(unfiltereds, filter);
      var unordereds = filtereds
        .OrderByDescending(x => x.AdminRole.ToLower() == DH.MainAdminRole.ToLower())
        .ThenBy(x => x.FullName);
      ViewBag.Filter = filter;
      IEnumerable<ApplicationUserViewModel> results = ViewHelper.PrepareUserViewModels(filter.Page, unordereds, ViewBag);
      return results == null ? View() : View(results.ToList());
    }

    public ActionResult Create() {
      return View();
    }

    private RedirectToRouteResult redirectToError(string error) {
      return RedirectToAction("ErrorLocal", new { error = error });
    }

    public ActionResult ErrorLocal(string error) {
      ViewBag.Error = error;
      return View("Error");
    }

    CoreDataModel db = new CoreDataModel();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(ApplicationUserCreateViewModel model) {
      //try {
      if (string.IsNullOrWhiteSpace(model.AdminRole) && string.IsNullOrWhiteSpace(model.WorkingRole)) {
        ModelState.AddModelError("WorkingRole", "[Working Role] and [Admin Role] cannot be both empty");
        ModelState.AddModelError("AdminRole", "[Working Role] and [Admin Role] cannot be both empty");
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
            DH.WorkingRoles.Any(x => x.EqualsIgnoreCase(model.WorkingRole))) {
            var addRoleResult = UserManager.AddToRole(user.Id, model.WorkingRole);
            if (!addRoleResult.Succeeded)
              return RedirectToAction("ErrorLocal");
          }
          if (!string.IsNullOrWhiteSpace(model.AdminRole) &&
            DH.AllowedAdminRoles.Any(x => x.EqualsIgnoreCase(model.AdminRole))) {
            var addRoleResult = UserManager.AddToRole(user.Id, model.AdminRole);
            if (!addRoleResult.Succeeded)
              return RedirectToAction("ErrorLocal");
          }

          UserHelper.CreateUserMap(db, model.Email, model.Password);

          return RedirectToAction("Index");
        }
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    public ActionResult Details(string id) {
      ApplicationUser user = UserManager.Users.FirstOrDefault(x => x.Id == id);
      if (user == null)
        return redirectToError("User Id not found");
      ApplicationUserViewModel model = new ApplicationUserViewModel(user);
      if (model == null)
        return redirectToError("Fail to create application user model");
      return View(model);
    }

    public ActionResult Delete(string id) {
      ApplicationUser user = UserManager.Users.FirstOrDefault(x => x.Id == id);
      if (user == null) 
        return redirectToError("User Id not found");
      ApplicationUserViewModel model = new ApplicationUserViewModel(user);
      if (model == null)
        return redirectToError("Fail to create application user model");
      return View(model);
    }

    [HttpPost]
    [ActionName("Delete")]
    public ActionResult DeletePost(string id) {
      ApplicationUser user = UserManager.Users.FirstOrDefault(x => x.Id == id);
      if (user == null)
        return redirectToError("User Id not found");      
      if (UserHelper.UserHasMainAdminRight(user))
        return redirectToError(user.AdminRole + " User cannot be edited or deleted");
      UserHelper.DeleteUserMap(db, user.UserName);
      var result = UserManager.Delete(user);
      if (!result.Succeeded)
        return redirectToError("User manager fails to delete the user. Errors: " +
          string.Join("<br/>", result.Errors.ToArray()));
      return RedirectToAction("Index");
    }

    public ActionResult Edit(string id) {
      ApplicationUser user = UserManager.Users.FirstOrDefault(x => x.Id == id);
      if (user == null)
        return redirectToError("User Id not found");
      ApplicationUserEditViewModel model = new ApplicationUserEditViewModel(user);
      if (model == null)
        return redirectToError("Fail to create application user model");
      return View(model);
    }

    [HttpPost]
    public ActionResult Edit(ApplicationUserEditViewModel model) {
      ApplicationUser user = UserManager.Users.FirstOrDefault(x => x.Id == model.Id);
      if (!ModelState.IsValid)
        return View(model);
      if (user == null)
        return redirectToError("User Id not found");
      if (UserHelper.UserHasMainAdminRight(user))
        return redirectToError(user.AdminRole + " User cannot be edited or deleted");

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
      if (!result.Succeeded)
        return redirectToError("User manager fails to update the user. Errors: " + 
          string.Join("<br/>", result.Errors.ToArray()));

      if (oldAdminRole != model.AdminRole) {
        if (!string.IsNullOrWhiteSpace(oldAdminRole)) //if old role is not empty, remove the old role
          UserManager.RemoveFromRole(user.Id, oldAdminRole);

        if (oldAdminRole != model.AdminRole && //old role is different from the current role
          !string.IsNullOrWhiteSpace(model.AdminRole) && //new role is not empty
          DH.AllowedAdminRoles.Any(x => x.EqualsIgnoreCase(model.AdminRole))) { //just add new role
          var addRoleResult = UserManager.AddToRole(user.Id, model.AdminRole);
          if (!addRoleResult.Succeeded)
            return RedirectToAction("ErrorLocal");
        }
      }

      if (oldWorkingRole != model.WorkingRole) {
        if (!string.IsNullOrWhiteSpace(oldWorkingRole)) //if old role is not empty, remove the old role
          UserManager.RemoveFromRole(user.Id, oldWorkingRole);

        if (oldWorkingRole != model.WorkingRole && //old role is different from the current role
          !string.IsNullOrWhiteSpace(model.WorkingRole) && //new role is not empty
          DH.WorkingRoles.Any(x => x.EqualsIgnoreCase(model.WorkingRole))) { //just add new role
          var addRoleResult = UserManager.AddToRole(user.Id, model.WorkingRole);
          if (!addRoleResult.Succeeded)
            return RedirectToAction("ErrorLocal");
        }
      }

      UserHelper.EditUserMapName(db, oldUserName, model.Email);
      return RedirectToAction("Index");
    }
  }
}


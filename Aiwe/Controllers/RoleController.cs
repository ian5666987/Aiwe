using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using Aiwe.Models;
using Aiwe.Models.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Extension.String;
using Aibe.Helpers;
using Aibe.Models.Filters;

namespace Aiwe.Controllers {
  [Authorize(Roles = Aibe.DH.DevRole)]
  public class RoleController : Controller {

    ApplicationDbContext context = new ApplicationDbContext();
    public RoleManager<IdentityRole> RoleManager = new RoleManager<IdentityRole>
      (new RoleStore<IdentityRole>(new ApplicationDbContext()));

    public ActionResult Index(int? page) {
      IOrderedQueryable<IdentityRole> roles = context.Roles
        .ToList() //this uses ToList() but in the user it is not. It is because Roles having very few items (tolerable)
        .Where(x => !Aibe.DH.AdminRoles.Any(y => y.EqualsIgnoreCase(x.Name)))
        .AsQueryable()
        .OrderBy(x => x.Name);
      List<RoleViewModel> viewModels = ((IEnumerable<RoleViewModel>)
        ViewHelper.PrepareRoleViewModels(page, roles, ViewBag)).ToList();
      return viewModels == null ? View() : View(viewModels);
    }

    [HttpPost]
    public ActionResult Index(RoleFilter filter) {
      var unfiltereds = context.Roles.Where(x => x.Name != Aibe.DH.DevRole)
        .ToList() //this uses ToList() but in the user it is not. It is because Roles having very few items (tolerable)
        .Where(x => !Aibe.DH.AdminRoles.Any(y => y.EqualsIgnoreCase(x.Name)))
        .AsQueryable()
        .OrderBy(x => x.Name);
      var filtereds = DataFilterHelper.ApplyRoleFilter(unfiltereds, filter);
      var unordereds = filtereds
        .OrderBy(x => x.Name);
      ViewBag.Filter = filter;
      IEnumerable<ApplicationUserViewModel> results = ViewHelper.PrepareRoleViewModels(filter.Page, unordereds, ViewBag);
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(RoleViewModel model) {
      if (!ModelState.IsValid)
        return View(model);
      IdentityRole role = new IdentityRole { Name = model.Name };
      var result = RoleManager.Create(role);
      if (result.Succeeded)
        return RedirectToAction("Index");
      return View(model);
    }

    public ActionResult Details(string id) {      
      IdentityRole role = RoleManager.Roles.FirstOrDefault(x => x.Id == id);
      if (role == null)
        return redirectToError("Role Id not found");
      RoleViewModel model = new RoleViewModel(role);
      if (model == null)
        return redirectToError("Fail to create role view model");
      return View(model);
    }

    public ActionResult Delete(string id) {
      IdentityRole role = RoleManager.Roles.FirstOrDefault(x => x.Id == id);
      if (role == null)
        return redirectToError("Role Id not found");
      RoleViewModel model = new RoleViewModel(role);
      if (model == null)
        return redirectToError("Fail to create role view model");
      return View(model);
    }

    [HttpPost]
    [ActionName("Delete")]
    public ActionResult DeletePost(string id) {
      IdentityRole role = RoleManager.Roles.FirstOrDefault(x => x.Id == id);
      if (role == null)
        return redirectToError("Role Id not found");
      if (Aibe.DH.AdminRoles.Any(x => x.EqualsIgnoreCase(role.Name))) //Admin roles cannot be edited, deleted, or changed
        return redirectToError(role.Name + " Role cannot be edited or deleted");
      var result = RoleManager.Delete(role);
      if (!result.Succeeded)
        return redirectToError("Role manager fails to delete the role. Errors: " +
          string.Join("<br/>", result.Errors.ToArray()));
      return RedirectToAction("Index");
    }

    public ActionResult Edit(string id) {
      IdentityRole role = RoleManager.Roles.FirstOrDefault(x => x.Id == id);

      if (role == null)
        return redirectToError("Role Id not found");
      RoleViewModel model = new RoleViewModel(role);
      if (model == null)
        return redirectToError("Fail to create role view model");
      return View(model);
    }

    [HttpPost]
    public ActionResult Edit(RoleViewModel model) {
      IdentityRole role = RoleManager.Roles.FirstOrDefault(x => x.Id == model.Id);
      if (!ModelState.IsValid)
        return View(model);
      if (role == null)
        return redirectToError("Role Id not found");
      if (Aibe.DH.AdminRoles.Any(x => x.EqualsIgnoreCase(role.Name))) //Admin roles cannot be edited, deleted, or changed
        return redirectToError(role.Name + " Role cannot be edited or deleted");
      role.Name = model.Name;
      var result = RoleManager.Update(role);
      if (!result.Succeeded)
        return redirectToError("Role manager fails to update the role. Errors: " +
          string.Join("<br/>", result.Errors.ToArray()));

      return RedirectToAction("Index");
    }
  }
}


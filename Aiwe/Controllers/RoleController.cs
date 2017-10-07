using Aibe.Models.Filters;
using Aiwe.Helpers;
using Aiwe.Models;
using Aiwe.Models.ViewModels;
using Extension.String;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;

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
        AiweViewHelper.PrepareRoleViewModels(page, roles, ViewBag)).ToList();
      return viewModels == null ? View() : View(viewModels);
    }

    [HttpPost]
    public ActionResult Index(RoleFilter filter) {
      var unfiltereds = context.Roles.Where(x => x.Name != Aibe.DH.DevRole)
        .ToList() //this uses ToList() but in the user it is not. It is because Roles having very few items (tolerable)
        .Where(x => !Aibe.DH.AdminRoles.Any(y => y.EqualsIgnoreCase(x.Name)))
        .AsQueryable()
        .OrderBy(x => x.Name);
      var filtereds = AiweDataFilterHelper.ApplyRoleFilter(unfiltereds, filter);
      var unordereds = filtereds
        .OrderBy(x => x.Name);
      ViewBag.Filter = filter;
      IEnumerable<ApplicationUserViewModel> results = AiweViewHelper.PrepareRoleViewModels(filter.Page, unordereds, ViewBag);
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(RoleViewModel model) {
      if (!ModelState.IsValid)
        return View(model);
      IdentityRole role = new IdentityRole { Name = model.Name };
      var result = RoleManager.Create(role);
      if (result.Succeeded)
        return RedirectToAction(Aibe.DH.IndexActionName);
      return View(model);
    }

    public ActionResult Details(string id) {      
      IdentityRole role = RoleManager.Roles.FirstOrDefault(x => x.Id == id);
      if (role == null)
        return redirectToError(Aibe.LCZ.NFE_RoleIdNotFound);
      RoleViewModel model = new RoleViewModel(role);
      if (model == null)
        return redirectToError(string.Format(Aibe.LCZ.E_FailToDoActionOnItemIn, Aibe.LCZ.W_Create, Aiwe.LCZ.W_RoleViewModel, Aibe.LCZ.W_Details));
      return View(model);
    }

    public ActionResult Delete(string id) {
      IdentityRole role = RoleManager.Roles.FirstOrDefault(x => x.Id == id);
      if (role == null)
        return redirectToError(Aibe.LCZ.NFE_RoleIdNotFound);
      RoleViewModel model = new RoleViewModel(role);
      if (model == null)
        return redirectToError(string.Format(Aibe.LCZ.E_FailToDoActionOnItemIn, Aibe.LCZ.W_Create, Aiwe.LCZ.W_RoleViewModel, Aibe.LCZ.W_Delete));
      return View(model);
    }

    [HttpPost]
    [ActionName(Aibe.DH.DeleteActionName)]
    public ActionResult DeletePost(string id) {
      IdentityRole role = RoleManager.Roles.FirstOrDefault(x => x.Id == id);
      if (role == null)
        return redirectToError(Aibe.LCZ.NFE_RoleIdNotFound);
      if (Aibe.DH.AdminRoles.Any(x => x.EqualsIgnoreCase(role.Name))) //Admin roles cannot be edited, deleted, or changed
        return redirectToError(string.Format(Aibe.LCZ.E_CannotBeEditedOrDeleted, role.Name));
      var result = RoleManager.Delete(role);
      if (!result.Succeeded) {
        StringBuilder sb = new StringBuilder(string.Format(Aibe.LCZ.E_FailToDoActionOnItem, Aibe.LCZ.W_Delete, Aibe.LCZ.W_Role));
        sb.Append(".<br/>");
        sb.Append(Aibe.LCZ.W_ErrorList);
        sb.Append(":<br/>");
        sb.Append(string.Join("<br/>", result.Errors.ToArray()));
        return redirectToError(sb.ToString());
      }
      return RedirectToAction(Aibe.DH.IndexActionName);
    }

    public ActionResult Edit(string id) {
      IdentityRole role = RoleManager.Roles.FirstOrDefault(x => x.Id == id);

      if (role == null)
        return redirectToError(Aibe.LCZ.NFE_RoleIdNotFound);
      RoleViewModel model = new RoleViewModel(role);
      if (model == null)
        return redirectToError(string.Format(Aibe.LCZ.E_FailToDoActionOnItemIn, Aibe.LCZ.W_Create, Aiwe.LCZ.W_RoleViewModel, Aibe.LCZ.W_Edit));
      return View(model);
    }

    [HttpPost]
    public ActionResult Edit(RoleViewModel model) {
      IdentityRole role = RoleManager.Roles.FirstOrDefault(x => x.Id == model.Id);
      if (!ModelState.IsValid)
        return View(model);
      if (role == null)
        return redirectToError(Aibe.LCZ.NFE_RoleIdNotFound);
      if (Aibe.DH.AdminRoles.Any(x => x.EqualsIgnoreCase(role.Name))) //Admin roles cannot be edited, deleted, or changed
        return redirectToError(string.Format(Aibe.LCZ.E_CannotBeEditedOrDeleted, role.Name));
      role.Name = model.Name;
      var result = RoleManager.Update(role);
      if (!result.Succeeded) {
        StringBuilder sb = new StringBuilder(string.Format(Aibe.LCZ.E_FailToDoActionOnItem, Aibe.LCZ.W_Update, Aibe.LCZ.W_Role));
        sb.Append(".<br/>");
        sb.Append(Aibe.LCZ.W_ErrorList);
        sb.Append(":<br/>");
        sb.Append(string.Join("<br/>", result.Errors.ToArray()));
      }
      return RedirectToAction(Aibe.DH.IndexActionName);
    }
  }
}


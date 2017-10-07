using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity.Migrations;
using Aiwe.Helpers;
using Aiwe.Models;
using Aiwe.Models.Filters;
using Aibe.Helpers;
using Aibe.Models;
using Extension.String;

namespace Aiwe.Controllers {
  [Authorize(Roles = Aiwe.DH.AdminAuthorizedRoles)]
  public class TeamController : Controller {

    ApplicationDbContext context = new ApplicationDbContext();

    public ActionResult Index(int? page) {
      var allOrderedMatches = context.Teams
        .OrderBy(x => x.Name.ToLower());
      NavDataModel navDataModel;
      List<Team> results = ViewHelper.PrepareFilteredModels(page, allOrderedMatches, out navDataModel);
      ViewBag.NavData = navDataModel;
      return results == null ? View() : View(results);
    }

    [HttpPost]
    public ActionResult Index(TeamFilter filter) {
      var unfiltereds = context.Teams
        .OrderBy(x => x.Name.ToLower());
      var filtereds = AiweDataFilterHelper.ApplyTeamFilter(unfiltereds, filter);
      var unordereds = filtereds
        .OrderBy(x => x.Name.ToLower());
      ViewBag.Filter = filter;
      NavDataModel navDataModel;
      List<Team> results = ViewHelper.PrepareFilteredModels<Team>(filter.Page, unordereds, out navDataModel);
      ViewBag.NavData = navDataModel;
      return results == null ? View() : View(results);
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
    public ActionResult Create(Team model) {
      if (!ModelState.IsValid)
        return View(model);

      var team = new Team {
        Id = context.Teams != null && context.Teams.Any() ? context.Teams.Max(x => x.Id) + 1 : 1,
        Name = model.Name.Trim(),
      };

      context.Teams.Add(team);
      context.SaveChanges();

      return RedirectToAction(Aibe.DH.IndexActionName);
    }

    public ActionResult Details(int id) {
      Team team = context.Teams.FirstOrDefault(x => x.Id == id);
      if (team == null)
        return redirectToError(Aibe.LCZ.NFE_IdNotFound);
      return View(team);
    }

    public ActionResult Delete(int id) {
      Team team = context.Teams.FirstOrDefault(x => x.Id == id);
      if (team == null)
        return redirectToError(Aibe.LCZ.NFE_IdNotFound);
      return View(team);
    }

    [HttpPost]
    [ActionName(Aibe.DH.DeleteActionName)]
    public ActionResult DeletePost(int id) {
      Team team = context.Teams.FirstOrDefault(x => x.Id == id);
      if (team == null)
        return redirectToError(Aibe.LCZ.NFE_IdNotFound);
      if (team.Name.EqualsIgnoreCase(Aiwe.LCZ.W_HomeTeam))
        return redirectToError(string.Format(Aibe.LCZ.E_CannotBeEditedOrDeleted, team.Name));
      context.Teams.Remove(team);
      context.SaveChanges();
      return RedirectToAction(Aibe.DH.IndexActionName);
    }

    public ActionResult Edit(int id) {
      Team team = context.Teams.FirstOrDefault(x => x.Id == id);
      if (team == null)
        return redirectToError(Aibe.LCZ.NFE_IdNotFound);
      return View(team);
    }

    [HttpPost]
    public ActionResult Edit(Team teamModel) {
      Team team = context.Teams.FirstOrDefault(x => x.Id == teamModel.Id);
      if (team == null)
        return redirectToError(Aibe.LCZ.NFE_IdNotFound);
      if (team.Name.EqualsIgnoreCase(Aiwe.LCZ.W_HomeTeam))
        return redirectToError(string.Format(Aibe.LCZ.E_CannotBeEditedOrDeleted, team.Name));
      context.Teams.AddOrUpdate(teamModel);
      context.SaveChanges();
      return RedirectToAction(Aibe.DH.IndexActionName);
    }
  }
}

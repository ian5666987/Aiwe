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
      return RedirectToAction("ErrorLocal", new { error = error });
    }

    public ActionResult ErrorLocal(string error) {
      ViewBag.Error = error;
      return View("Error");
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

      return RedirectToAction("Index");
    }

    public ActionResult Details(int id) {
      Team team = context.Teams.FirstOrDefault(x => x.Id == id);
      if (team == null)
        return redirectToError("Id not found");
      return View(team);
    }

    public ActionResult Delete(int id) {
      Team team = context.Teams.FirstOrDefault(x => x.Id == id);
      if (team == null)
        return redirectToError("Id not found");
      return View(team);
    }

    [HttpPost]
    [ActionName("Delete")]
    public ActionResult DeletePost(int id) {
      Team team = context.Teams.FirstOrDefault(x => x.Id == id);
      if (team == null)
        return redirectToError("Id not found");
      if (team.Name == "Home")
        return redirectToError(team.Name + " Team cannot be edited or deleted");
      context.Teams.Remove(team);
      context.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Edit(int id) {
      Team team = context.Teams.FirstOrDefault(x => x.Id == id);
      if (team == null)
        return redirectToError("Id not found");
      return View(team);
    }

    [HttpPost]
    public ActionResult Edit(Team teamModel) {
      Team team = context.Teams.FirstOrDefault(x => x.Id == teamModel.Id);
      if (team == null)
        return redirectToError("Id not found");
      if (team.Name == "Home")
        return redirectToError(team.Name + " Team cannot be edited or deleted");
      context.Teams.AddOrUpdate(teamModel);
      context.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}

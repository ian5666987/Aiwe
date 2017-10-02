using Aibe.Helpers;
using Aibe.Models;
using Aibe.Models.Filters;
using Aiwe.Helpers;
using Aiwe.Models.DB;
using Extension.Cryptography;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;

namespace Aiwe.Controllers {
  [Authorize(Roles = Aibe.DH.DevRole)]
  public class MetaController : Controller {
    CoreDataModel db = new CoreDataModel();
#if DEBUG
    public ActionResult Index(int? page) {
      var allOrderedMatches = db.MetaItems
        .OrderBy(x => x.TableName.ToLower());
      NavDataModel navDataModel;
      List<MetaItem> results = ViewHelper.PrepareFilteredModels(page, allOrderedMatches, out navDataModel);
      ViewBag.NavData = navDataModel;
      return results == null ? View() : View(results);
    }

    [HttpPost]
    public ActionResult Index(MetaFilter filter) {
      var unfiltereds = db.MetaItems
        .OrderBy(x => x.TableName.ToLower());
      var filtereds = AiweDataFilterHelper.ApplyMetaFilter(unfiltereds, filter);
      var unordereds = filtereds
        .OrderBy(x => x.TableName.ToLower());
      ViewBag.Filter = filter;
      NavDataModel navDataModel;
      List<MetaItem> results = ViewHelper.PrepareFilteredModels(filter.Page, unordereds, out navDataModel);
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
    [ValidateInput(false)]
    [ValidateAntiForgeryToken]
    public ActionResult Create(MetaItem model) {
      if (!ModelState.IsValid)
        return View(model);

      db.MetaItems.Add(model);
      db.SaveChanges();
      AiweTableHelper.AddMeta(new MetaInfo(model)); //has validity check in the AddMeta method

      return RedirectToAction("Index");
    }

    public ActionResult Details(string id) {
      MetaItem meta = db.MetaItems.FirstOrDefault(x => x.TableName == id);
      if (meta == null)
        return redirectToError("Id not found");
      return View(meta);
    }

    public ActionResult Delete(string id) {
      MetaItem meta = db.MetaItems.FirstOrDefault(x => x.TableName == id);
      if (meta == null)
        return redirectToError("Id not found");
      return View(meta);
    }

    [HttpPost]
    [ActionName("Delete")]
    public ActionResult DeletePost(string id) {
      MetaItem meta = db.MetaItems.FirstOrDefault(x => x.TableName == id);
      if (meta == null)
        return redirectToError("Id not found");
      db.MetaItems.Remove(meta);
      db.SaveChanges();
      AiweTableHelper.DeleteMeta(id);
      return RedirectToAction("Index");
    }

    public ActionResult Edit(string id) {
      MetaItem meta = db.MetaItems.FirstOrDefault(x => x.TableName == id);
      if (meta == null)
        return redirectToError("Id not found");
      return View(meta);
    }

    [HttpPost]
    [ValidateInput(false)]
    public ActionResult Edit(MetaItem model) {
      MetaItem meta = db.MetaItems.FirstOrDefault(x => x.TableName == model.TableName);
      if (!ModelState.IsValid)
        return View(model);

      if (meta == null)
        return redirectToError("Id not found");

      //meta.TableName = model.TableName; //Not needed because it is the key
      meta.DisplayName = model.DisplayName;
      meta.ItemsPerPage = model.ItemsPerPage;
      meta.OrderBy = model.OrderBy;
      meta.ActionList = model.ActionList;
      meta.DefaultActionList = model.DefaultActionList;
      meta.TableActionList = model.TableActionList;
      meta.DefaultTableActionList = model.DefaultTableActionList;
      meta.TextFieldColumns = model.TextFieldColumns;
      meta.PictureColumns = model.PictureColumns;
      meta.IndexShownPictureColumns = model.IndexShownPictureColumns;
      meta.RequiredColumns = model.RequiredColumns;
      meta.NumberLimitColumns = model.NumberLimitColumns;
      meta.RegexCheckedColumns = model.RegexCheckedColumns;
      meta.RegexCheckedColumnExamples = model.RegexCheckedColumnExamples;
      meta.UserRelatedFilters = model.UserRelatedFilters;
      meta.DisableFilter = model.DisableFilter;
      meta.ColumnExclusionList = model.ColumnExclusionList;
      meta.FilterExclusionList = model.FilterExclusionList;
      meta.DetailsExclusionList = model.DetailsExclusionList;
      meta.CreateEditExclusionList = model.CreateEditExclusionList;
      meta.AccessExclusionList = model.AccessExclusionList;
      meta.ColoringList = model.ColoringList;
      meta.FilterDropDownLists = model.FilterDropDownLists;
      meta.CreateEditDropDownLists = model.CreateEditDropDownLists;
      meta.PrefixesOfColumns = model.PrefixesOfColumns;
      meta.PostfixesOfColumns = model.PostfixesOfColumns;
      meta.ListColumns = model.ListColumns;
      meta.TimeStampColumns = model.TimeStampColumns;
      meta.HistoryTable = model.HistoryTable;
      meta.HistoryTrigger = model.HistoryTrigger;
      meta.AutoGeneratedColumns = model.AutoGeneratedColumns;
      meta.ColumnSequence = model.ColumnSequence;
      meta.ColumnAliases = model.ColumnAliases;
      meta.EditShowOnlyColumns = model.EditShowOnlyColumns;
      meta.ScriptConstructorColumns = model.ScriptConstructorColumns;
      meta.ScriptColumns = model.ScriptColumns;

      db.MetaItems.AddOrUpdate(meta);
      db.SaveChanges();

      AiweTableHelper.UpdateMeta(meta);

      return RedirectToAction("Index");
    }

    public ActionResult CryptoSerialize(string id) {
      MetaItem meta = db.MetaItems.FirstOrDefault(x => x.TableName == id);
      if (meta == null)
        return redirectToError("Id not found");
      return View(meta);
    }

    [HttpPost]
    [ActionName("CryptoSerialize")]
    public ActionResult CryptoSerializePost(string id) {
      MetaItem meta = db.MetaItems.FirstOrDefault(x => x.TableName == id);
      if (meta == null)
        return redirectToError("Id not found");
      string folderPath = Server.MapPath("~/Settings");
      Cryptography.CryptoSerialize(meta, folderPath, id);
      return RedirectToAction("Index");
    }

    public ActionResult CryptoSerializeAll() {
      var all = db.MetaItems.ToList();
      var fileNames = all.Select(x => x.TableName).ToList();
      string folderPath = Server.MapPath("~/Settings");
      Cryptography.CryptoSerializeAll(all, folderPath, fileNames);
      return RedirectToAction("Success", new { msg = "You have successfully crypto-serialize all (" + all.Count + ") meta table entries!" });
    }

#endif

    public ActionResult Success(string msg) {
      ViewBag.SuccessMessage = msg;
      return View();
    }

    public ActionResult DecryptoSerializeAll() {
      string folderPath = Server.MapPath("~/Settings");
      int count = AiweTableHelper.DecryptMetaItems(folderPath);
      AiweTableHelper.PrepareMetas();
      return RedirectToAction("Success", new { msg = "You have successfully decrypto-serialize all (" + count + ") meta table ASTRIOCFILE(s)!" });
    }

  }
}


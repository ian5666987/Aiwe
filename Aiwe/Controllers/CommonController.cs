using Aibe.Helpers;
using Aibe.Models;
using Aibe.Models.Core;
using Aiwe.ActionFilters;
using Aiwe.Extensions;
using Aiwe.Models;
using Aiwe.Models.Extras;
using Aiwe.Helpers;
using Extension.Database.SqlServer;
using Extension.Models;
using Extension.String;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Aiwe.Controllers {
  public class CommonController : Controller {
    [CommonActionFilter]
    //Get does not have filter
    public ActionResult Index(string commonDataTableName, int? page) { //Where all common tables are returned as list
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      return View(getFilterIndexModel(meta, commonDataTableName, page, null, loadAllData: false, isGrouping: meta.IsGroupTable));
    }

    [HttpPost]
    [CommonActionFilter]
    public ActionResult Index(string commonDataTableName, int? commonDataFilterPage, FormCollection collections) { //do not change the name commonDataFilterPage to page
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      if (collections.AllKeys.Any(x => x.EqualsIgnoreCase(Aibe.DH.FilterTableActionNameInput))) {
        string tableActionInput = collections[Aibe.DH.FilterTableActionNameInput];
        if (!string.IsNullOrWhiteSpace(tableActionInput)) {
          string prefix = Aibe.DH.DefaultTableActionPrefix + "-";
          if (tableActionInput.EqualsIgnoreCase(prefix + Aibe.DH.ExportToCSVTableActionName))
            return exportToCSV(meta, commonDataTableName, commonDataFilterPage, collections, isGrouping: meta.IsGroupTable);
          else if (tableActionInput.EqualsIgnoreCase(prefix + Aibe.DH.ExportAllToCSVTableActionName))
            return exportAllToCSV(meta, commonDataTableName, commonDataFilterPage, collections, isGrouping: meta.IsGroupTable);          
        }
      }
      return View(getFilterIndexModel(meta, commonDataTableName, commonDataFilterPage, collections, loadAllData: false, isGrouping: meta.IsGroupTable));
    }

    private AiweFilterIndexModel getFilterIndexModel(MetaInfo meta, string commonDataTableName, int? page, FormCollection collections, bool loadAllData, bool isGrouping) {
      TempData.Clear();
      FilterIndexModel model = new FilterIndexModel(meta, page, collections == null ? 
        null : AiweTranslationHelper.FormCollectionToDictionary(collections));

      //Get index info
      QueryHelper.HandleUserRelatedScripting(model.QueryScript, Aiwe.DH.UserTableName, User?.Identity?.Name, AiweUserHelper.UserHasMainAdminRight(User),
        User == null || User.Identity == null ? false : User.Identity.IsAuthenticated, meta.UserRelatedFilters); //TODO not tested yet, but seems to be OK
      model.CompleteModelAndData(isGrouping, loadAllData);
      return new AiweFilterIndexModel(meta, User, model, model.StringDictionary);
    }

    [CommonActionFilter]
    public ActionResult Create(string commonDataTableName, string[] identifierKeys, string[] identifierValues) {
      //Note that the identifiers are only not null if the calling comes from group details to create
      //So the question now here, how the group details get its identifiers in the first place?
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);      
      var identifiers = AiweTranslationHelper.GetIdentifiers(meta, identifierKeys, identifierValues);
      AiweCreateEditModel model = new AiweCreateEditModel(meta, Aibe.DH.CreateActionName, null, identifiers);
      return View(model);
    }

    [ValidateInput(false)]
    [HttpPost]
    [CommonActionFilter]
    public ActionResult Create(string commonDataTableName, string[] identifierKeys, string[] identifierValues, FormCollection collections) {
      DateTime now = DateTime.Now;
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      var identifiers = AiweTranslationHelper.GetIdentifiers(meta, identifierKeys, identifierValues);
      bool isFromGroupTable = identifiers != null && identifiers.Count > 0;
      Dictionary<string, string> dictCollections = AiweTranslationHelper.FormCollectionToDictionary(collections);
      AiweTranslationHelper.AdjustModelState(ModelState, dictCollections); //to remove identifiers from model state
      List<string> checkExclusions = new List<string> { Aibe.DH.TableNameParameterName }; //different per Action, because of additional item in the ModelState

      //Check model state's validity
      Dictionary<string, string> errorDict = new AiweCheckerHelper().CheckModelValidity(Aiwe.DH.TableModelClassPrefix, meta.TableSource, meta.ArrangedDataColumns, 
        dictCollections, ModelState.Keys.ToList(), meta, checkExclusions, AiweUserHelper.UserIsDeveloper(User), now, Aibe.DH.CreateActionName, strongCheck: Aiwe.DH.UseStrongCheck, 
        isTagChecked: Aiwe.DH.IsTagChecked);
      AiweTranslationHelper.FillModelStateWithErrorDictionary(ModelState, errorDict);
      if (!ModelState.IsValid)
        return View(new AiweCreateEditModel(meta, Aibe.DH.CreateActionName, null, identifiers));

      //Only if model state is correct that we could get valid key infos safely
      var completeKeyInfo = KeyInfoHelper.GetCompleteKeyInfo(meta.TableSource, dictCollections, dictCollections.Keys, meta.ArrangedDataColumns, filterStyle: false, meta: meta, actionType: Aibe.DH.CreateActionName);
      if (completeKeyInfo == null || completeKeyInfo.ValidKeys == null || !completeKeyInfo.ValidKeys.Any()) {
        ViewBag.ErrorMessage = string.Format(Aibe.LCZ.E_InvalidOrEmptyParameter, commonDataTableName);
        return View(Aiwe.DH.ErrorViewName);
      }

      var userPars = AiweUserHelper.GetUserParameters(User, Aibe.DH.ParameterUserPrefix);
      meta.HandlePreActionProcedures(Aibe.DH.CreateActionName, -1, null, userPars); //pre action does not have cid or row

      //TODO Beware of duplicate record because the client clicks more than once
      // -> If identical record is found, display other page to ask the client to confirm
      BaseScriptModel scriptModel = LogicHelper.CreateInsertScriptModel(meta.TableSource, completeKeyInfo, dictCollections, now, meta);
      object generatedId = SQLServerHandler.ExecuteScalar(Aibe.DH.DataDBConnectionString, scriptModel.Script, scriptModel.Pars);
      int cid = int.Parse(generatedId.ToString());
      bool saveAttachmentResult = AiweFileHelper.SaveAttachments(Request, 
        Server.MapPath("~/" + Aibe.DH.DefaultAttachmentFolderName + "/" + commonDataTableName + "/" + cid.ToString()));

      meta.HandleEmailEvents(Aibe.DH.CreateActionName, cid, null, userPars); //create has no originalRow
      meta.HandleHistoryEvents(Aibe.DH.CreateActionName, cid, null); //create has no originalRow

      DataRow changedRow = meta.GetFullRowSource(cid);
      meta.HandlePostActionProcedures(Aibe.DH.CreateActionName, cid, null, userPars); //does not have the original row BUT has cid now...
      if (isFromGroupTable) {
        RouteValueDictionary routeValues = AiweTranslationHelper.GetRouteValuesForRedirection(commonDataTableName, identifiers);
        return RedirectToAction(Aibe.DH.GroupDetailsActionName, routeValues);
      } else 
        return RedirectToAction(Aibe.DH.IndexActionName, new { commonDataTableName = commonDataTableName });
    }

    //Likely used by filter and create edit
    [CommonActionFilter]
    public ActionResult Edit(string commonDataTableName, string[] identifierKeys, string[] identifierValues, int id) {
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      var identifiers = AiweTranslationHelper.GetIdentifiers(meta, identifierKeys, identifierValues);
      Dictionary<string, object> objectDictionary = LogicHelper.FillDetailsFromTableToObjectDictionary(meta.TableSource, id);
      AiweCreateEditModel model = new AiweCreateEditModel(meta, Aibe.DH.EditActionName, LogicHelper.ObjectDictionaryToStringDictionary(objectDictionary), identifiers);
      return View(model);
    }

    [ValidateInput(false)]
    [HttpPost]
    [CommonActionFilter]
    public ActionResult Edit(string commonDataTableName, string[] identifierKeys, string[] identifierValues, int cid, FormCollection collections) {
      DateTime now = DateTime.Now;
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      var identifiers = AiweTranslationHelper.GetIdentifiers(meta, identifierKeys, identifierValues);
      bool isFromGroupTable = identifiers != null && identifiers.Count > 0;
      //Again, so that it will replaced with the given collection, using capital "C" -> "Cid"
      ModelState.Remove("cid"); //This is unique because the field parameters here contain "cid". 
      Dictionary<string, string> dictCollections = AiweTranslationHelper.FormCollectionToDictionary(collections);
      AiweTranslationHelper.AdjustModelState(ModelState, dictCollections); //to remove identifiers from model state

      List<string> checkExclusions = new List<string> { Aibe.DH.TableNameParameterName }; //different per Action, because of additional item in the ModelState

      //Check model state's validity
      Dictionary<string, string> errorDict = new AiweCheckerHelper().CheckModelValidity(Aiwe.DH.TableModelClassPrefix, meta.TableSource, meta.ArrangedDataColumns, 
        dictCollections, ModelState.Keys.ToList(), meta, checkExclusions, AiweUserHelper.UserIsDeveloper(User), now, Aibe.DH.EditActionName, strongCheck: Aiwe.DH.UseStrongCheck,
        isTagChecked: Aiwe.DH.IsTagChecked);
      AiweTranslationHelper.FillModelStateWithErrorDictionary(ModelState, errorDict);
      if (!ModelState.IsValid) {
        AiweCreateEditModel model = new AiweCreateEditModel(meta, Aibe.DH.EditActionName, dictCollections, identifiers);
        return View(model);
      }

      var filteredKeys = dictCollections.Keys.Where(x => !x.EqualsIgnoreCase(Aibe.DH.Cid)); //everything filled but the Cid
      var completeKeyInfo = KeyInfoHelper.GetCompleteKeyInfo(meta.TableSource, dictCollections, filteredKeys, meta.ArrangedDataColumns, filterStyle: false, meta: meta, actionType: Aibe.DH.EditActionName);
      if (completeKeyInfo == null || completeKeyInfo.ValidKeys == null || !completeKeyInfo.ValidKeys.Any()) {
        ViewBag.ErrorMessage = string.Format(Aibe.LCZ.E_InvalidOrEmptyParameter, commonDataTableName);
        return View(Aiwe.DH.ErrorViewName);
      }

      var userPars = AiweUserHelper.GetUserParameters(User, Aibe.DH.ParameterUserPrefix);
      DataRow originalRow = meta.GetFullRowSource(cid);
      meta.HandlePreActionProcedures(Aibe.DH.EditActionName, cid, originalRow, userPars);

      BaseScriptModel scriptModel = LogicHelper.CreateUpdateScriptModel(meta.TableSource, cid, completeKeyInfo, dictCollections, now);
      SQLServerHandler.ExecuteScript(Aibe.DH.DataDBConnectionString, scriptModel.Script, scriptModel.Pars);
      bool saveAttachmentResult = AiweFileHelper.SaveAttachments(Request,
        Server.MapPath("~/" + Aibe.DH.DefaultAttachmentFolderName + "/" + commonDataTableName + "/" + cid)); //there is no need for checking this too, because all errors are returned

      meta.HandleEmailEvents(Aibe.DH.EditActionName, cid, originalRow, userPars); //handle email events and history events are still using the originalRow, this is correct
      meta.HandleHistoryEvents(Aibe.DH.EditActionName, cid, originalRow);
      meta.HandlePostActionProcedures(Aibe.DH.EditActionName, cid, originalRow, userPars); //passing the originalRow, not the changed one

      if (isFromGroupTable) {
        RouteValueDictionary routeValues = AiweTranslationHelper.GetRouteValuesForRedirection(commonDataTableName, identifiers);
        return RedirectToAction(Aibe.DH.GroupDetailsActionName, routeValues);
      } else
        return RedirectToAction(Aibe.DH.IndexActionName, new { commonDataTableName = commonDataTableName });
    }

    [CommonActionFilter]
    public ActionResult Delete(string commonDataTableName, string[] identifierKeys, string[] identifierValues, int id) { //Where all common tables details are returned and can be deleted
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      var identifiers = AiweTranslationHelper.GetIdentifiers(meta, identifierKeys, identifierValues);
      Dictionary<string, object> objectDictionary = LogicHelper.FillDetailsFromTableToObjectDictionary(meta.TableSource, id);
      AiweDetailsModel model = new AiweDetailsModel(meta, id, LogicHelper.ObjectDictionaryToStringDictionary(objectDictionary), identifiers);
      return View(model);
    }

    [HttpPost]
    [CommonActionFilter]
    [ActionName(Aibe.DH.DeleteActionName)]
    public ActionResult DeletePost(string commonDataTableName, string[] identifierKeys, string[] identifierValues, int id) { //Where all common tables deletes are returned and can be deleted
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      var identifiers = AiweTranslationHelper.GetIdentifiers(meta, identifierKeys, identifierValues);
      bool isFromGroupTable = identifiers != null && identifiers.Count > 0;

      var userPars = AiweUserHelper.GetUserParameters(User, Aibe.DH.ParameterUserPrefix);
      DataRow originalRow = meta.GetFullRowSource(id);
      meta.HandlePreActionProcedures(Aibe.DH.DeleteActionName, id, originalRow, userPars);
      meta.HandleEmailEvents(Aibe.DH.DeleteActionName, id, null, userPars); //delete must not have original row //email and history events must be handled before the deletion
      meta.HandleHistoryEvents(Aibe.DH.DeleteActionName, id, null); //delete must not have original row
      LogicHelper.DeleteItem(meta.TableSource, id); //Currently do not return any error

      meta.HandlePostActionProcedures(Aibe.DH.DeleteActionName, -1, null, userPars); //delete action has neither id nor row
      if (isFromGroupTable) {
        RouteValueDictionary routeValues = AiweTranslationHelper.GetRouteValuesForRedirection(commonDataTableName, identifiers);
        return RedirectToAction(Aibe.DH.GroupDetailsActionName, routeValues);
      } else
        return RedirectToAction(Aibe.DH.IndexActionName, new { commonDataTableName = commonDataTableName });
    }

    //Later add filter to check if a user has right to see this table
    [CommonActionFilter]
    public ActionResult Details(string commonDataTableName, string[] identifierKeys, string[] identifierValues, int id) { //there must be a number named cid (common Id)
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      var identifiers = AiweTranslationHelper.GetIdentifiers(meta, identifierKeys, identifierValues);
      Dictionary<string, object> objectDictionary = LogicHelper.FillDetailsFromTableToObjectDictionary(meta.TableSource, id);
      AiweDetailsModel model = new AiweDetailsModel(meta, id, LogicHelper.ObjectDictionaryToStringDictionary(objectDictionary), identifiers);
      return View(model);
    }

    //Simply returning a view which consists of list of attachments. The view should have some mechanism to download
    [CommonActionFilter]
    public ActionResult DownloadAttachments(string commonDataTableName, string[] identifierKeys, string[] identifierValues, int id) { //there must be a number named cid (common Id)
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      var identifiers = AiweTranslationHelper.GetIdentifiers(meta, identifierKeys, identifierValues);
      Dictionary<string, object> objectDictionary = LogicHelper.FillDetailsFromTableToObjectDictionary(meta.TableSource, id);
      AiweDetailsModel model = new AiweDetailsModel(meta, id, LogicHelper.ObjectDictionaryToStringDictionary(objectDictionary), identifiers);
      return View(model);
    }

    [CommonActionFilter]
    public ActionResult CreateGroup(string commonDataTableName, string[] identifierColumns) {
      DateTime now = DateTime.Now;
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      AiweCreateEditGroupModel model = new AiweCreateEditGroupModel(meta,
        Aibe.DH.CreateGroupActionName, null, identifierColumns.ToList());
      if (!model.Meta.IsGroupByFullyAutomatic())
        return View(model);
      Dictionary<string, string> dictCollections = new Dictionary<string, string>();
      foreach (var identifierColumn in identifierColumns)
        dictCollections.Add(identifierColumn, null);
      return commonCreateEditGroup(commonDataTableName, Aibe.DH.CreateGroupActionName, identifierColumns, null, dictCollections); //if it is fully automatic, then consider CreateGroup POST has happened...
    }

    [ValidateInput(false)]
    [HttpPost]
    [CommonActionFilter]
    public ActionResult CreateGroup(string commonDataTableName, string[] identifierColumns, FormCollection collections) {
      Dictionary<string, string> dictCollections = AiweTranslationHelper.FormCollectionToDictionary(collections);
      return commonCreateEditGroup(commonDataTableName, Aibe.DH.CreateGroupActionName, identifierColumns, null, dictCollections);
    }

    [CommonActionFilter]
    public ActionResult EditGroup(string commonDataTableName, string[] identifierKeys, string[] identifierValues) {
      DateTime now = DateTime.Now;
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      List<KeyValuePair<string, object>> identifiers = AiweTranslationHelper.GetIdentifiers(meta, identifierKeys, identifierValues);
      Dictionary<string, string> stringDict = new Dictionary<string, string>();
      foreach (var identifier in identifiers)
        stringDict.Add(identifier.Key, identifier.Value.ToString());
      AiweCreateEditGroupModel model = new AiweCreateEditGroupModel(meta,
        Aibe.DH.EditGroupActionName, stringDict, identifiers.Select(x => x.Key).ToList(), identifiers);
      return View(model);
    }

    [ValidateInput(false)]
    [HttpPost]
    [CommonActionFilter]
    public ActionResult EditGroup(string commonDataTableName, string[] identifierKeys, string[] identifierValues, FormCollection collections) {
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      List<KeyValuePair<string, object>> identifierInputs = AiweTranslationHelper.GetIdentifiers(meta, identifierKeys, identifierValues);
      Dictionary<string, string> dictCollections = AiweTranslationHelper.FormCollectionToDictionary(collections);
      return commonCreateEditGroup(commonDataTableName, Aibe.DH.EditGroupActionName, identifierKeys, identifierInputs, dictCollections);
    }

    private ActionResult commonCreateEditGroup(string commonDataTableName, string actionName, string[] identifierColumns, 
      List<KeyValuePair<string, object>> identifierInputs, Dictionary<string, string> dictCollections) {
      DateTime now = DateTime.Now;
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      AiweTranslationHelper.AdjustModelState(ModelState, dictCollections); //to remove identifiers from model state

      List<string> checkExclusions = new List<string> { Aibe.DH.TableNameParameterName }; //different per Action, because of additional item in the ModelState

      //Check model state's validity
      Dictionary<string, string> errorDict = new AiweCheckerHelper().CheckModelValidity(Aiwe.DH.TableModelClassPrefix, meta.TableSource, meta.ArrangedDataColumns,
        dictCollections, ModelState.Keys.ToList(), meta, checkExclusions, AiweUserHelper.UserIsDeveloper(User), now, actionName, strongCheck: Aiwe.DH.UseStrongCheck,
        isTagChecked: Aiwe.DH.IsTagChecked);
      AiweTranslationHelper.FillModelStateWithErrorDictionary(ModelState, errorDict);
      if (!ModelState.IsValid)
        return View(new AiweCreateEditGroupModel(meta, actionName, dictCollections, identifierColumns.ToList(), identifierInputs));

      //Only if model state is correct that we could get valid key infos safely
      var completeKeyInfo = KeyInfoHelper.GetCompleteKeyInfo(meta.TableSource, dictCollections, dictCollections.Keys,
        meta.ArrangedDataColumns, filterStyle: false, meta: meta, actionType: actionName);
      if (completeKeyInfo == null || completeKeyInfo.ValidKeys == null || !completeKeyInfo.ValidKeys.Any()) {
        ViewBag.ErrorMessage = string.Format(Aibe.LCZ.E_InvalidOrEmptyParameter, commonDataTableName);
        return View(Aiwe.DH.ErrorViewName);
      }

      List<KeyValuePair<string, object>> identifiers = LogicHelper.GetAllAutoGeneratedPairs(meta.TableSource, true, completeKeyInfo, dictCollections, now, meta);
      if (actionName.EqualsIgnoreCase(Aibe.DH.CreateGroupActionName)) { //CreateGroup case
        AiweFilterGroupDetailsModel filterGroupDetailsModel = getFilterGroupDetailsModel(commonDataTableName, identifiers);
        return View(Aibe.DH.GroupDetailsActionName, filterGroupDetailsModel);
      } else { //EditGroup case
        int result = meta.ApplyEditGroup(identifierInputs, identifiers); //currently, the result is unchecked
        return RedirectToAction(Aibe.DH.IndexActionName, new { commonDataTableName = commonDataTableName });
      }
    }

    private AiweFilterGroupDetailsModel getFilterGroupDetailsModel(string commonDataTableName, List<KeyValuePair<string, object>> identifiers,
      int? page = 1, Dictionary<string, string> collections = null, bool loadAllData = false, bool isGroupDeletion = false) {
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      FilterGroupDetailsModel model = new FilterGroupDetailsModel(meta, page, identifiers, collections);
      QueryHelper.HandleUserRelatedScripting(model.QueryScript, Aiwe.DH.UserTableName, User?.Identity?.Name,
        AiweUserHelper.UserHasMainAdminRight(User),
        User?.Identity != null && !string.IsNullOrWhiteSpace(User.Identity.Name), meta.UserRelatedFilters);
      model.CompleteModelAndData(isGrouping: false, loadAllData: loadAllData);
      return new AiweFilterGroupDetailsModel(meta, User, model, isGroupDeletion, model.StringDictionary);
    }

    [CommonActionFilter]
    public ActionResult GroupDetails(string commonDataTableName, string[] identifierKeys, string[] identifierValues, int? page = 1) {
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      var identifiers = AiweTranslationHelper.GetIdentifiers(meta, identifierKeys, identifierValues);
      return View(getFilterGroupDetailsModel(commonDataTableName, identifiers, page, null, loadAllData: false, isGroupDeletion: false));
    }

    [HttpPost]
    [CommonActionFilter]
    public ActionResult GroupDetails(string commonDataTableName, string[] identifierKeys, string[] identifierValues, int? commonDataFilterPage, FormCollection collections) {
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      var identifiers = AiweTranslationHelper.GetIdentifiers(meta, identifierKeys, identifierValues);
      Dictionary<string, string> dictCollections = AiweTranslationHelper.FormCollectionToDictionary(collections);
      return View(getFilterGroupDetailsModel(commonDataTableName, identifiers, commonDataFilterPage, dictCollections, loadAllData: false, isGroupDeletion: false));
    }

    [CommonActionFilter]
    public ActionResult DeleteGroup(string commonDataTableName, string[] identifierKeys, string[] identifierValues, int? page = 1) {
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      var identifiers = AiweTranslationHelper.GetIdentifiers(meta, identifierKeys, identifierValues);
      return View(getFilterGroupDetailsModel(commonDataTableName, identifiers, page, null, loadAllData: false, isGroupDeletion: true));
    }

    [HttpPost]
    [CommonActionFilter]
    public ActionResult DeleteGroup(string commonDataTableName, string[] identifierKeys, string[] identifierValues, int? commonDataFilterPage, FormCollection collections) {
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      var identifiers = AiweTranslationHelper.GetIdentifiers(meta, identifierKeys, identifierValues);
      Dictionary<string, string> dictCollections = AiweTranslationHelper.FormCollectionToDictionary(collections);
      return View(getFilterGroupDetailsModel(commonDataTableName, identifiers, commonDataFilterPage, dictCollections, loadAllData: false, isGroupDeletion: true));
    }

    //This is where the actual delete occurs
    [HttpPost]
    [CommonActionFilter]
    [ActionName(Aiwe.DH.DeleteGroupActualActionName)]
    public ActionResult DeleteGroupActual(string commonDataTableName, string[] identifierKeys, string[] identifierValues) {
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      var identifiers = AiweTranslationHelper.GetIdentifiers(meta, identifierKeys, identifierValues);
      LogicHelper.DeleteGroup(meta.TableSource, identifiers);
      return RedirectToAction(Aibe.DH.IndexActionName, new { commonDataTableName = commonDataTableName });
    }

    private ActionResult commonExportToCSV(string tableName, string fileDownloadName, AiweBaseFilterIndexModel model) {
      List<string> excludedColumns = model.GetExcludedColumnsInCsv(model.Meta.RawDataColumnNames, User);
      string csvString = model.BaseModel.GenerateCSVString(excludedColumns, Aiwe.DH.CsvDateTimeFormat);
      byte[] buff = Encoding.ASCII.GetBytes(csvString);
      string mimeType = "text/csv";
      return File(buff, mimeType, fileDownloadName);
    }

    private ActionResult exportToCSV (MetaInfo meta, string commonDataTableName, int? commonDataFilterPage, FormCollection collections, bool isGrouping) {
      try {
        return commonExportToCSV(commonDataTableName, commonDataTableName + ".csv",
          getFilterIndexModel(meta, commonDataTableName, commonDataFilterPage, collections, loadAllData: false, isGrouping: isGrouping));
      } catch (Exception ex) {
        string exStr = ex.ToString();
        LogHelper.Error(User.Identity.Name, null, Aiwe.DH.Mvc, Aiwe.DH.MvcCommonControllerName,
          commonDataTableName, Aibe.DH.ExportToCSVTableActionName, null, exStr);
#if DEBUG
        ViewBag.ErrorMessage = exStr;
#endif
        return View(Aiwe.DH.ErrorViewName);
      }
    }

    private ActionResult exportAllToCSV(MetaInfo meta, string commonDataTableName, int? commonDataFilterPage, FormCollection collections, bool isGrouping) {
      try { 
      return commonExportToCSV(commonDataTableName, commonDataTableName + "_All.csv",
        getFilterIndexModel(meta, commonDataTableName, commonDataFilterPage, collections, loadAllData: true, isGrouping: isGrouping));
      } catch (Exception ex) {
        string exStr = ex.ToString();
        LogHelper.Error(User.Identity.Name, null, Aiwe.DH.Mvc, Aiwe.DH.MvcCommonControllerName,
          commonDataTableName, Aibe.DH.ExportAllToCSVTableActionName, null, exStr);
#if DEBUG
        ViewBag.ErrorMessage = exStr;
#endif
        return View(Aiwe.DH.ErrorViewName);
      }
    }

    #region live javascript use
    //Called when live dropdown is lifted up
    [CommonActionFilter]
    public JsonResult GetLiveDropDownItems(string commonDataTableName, string changedColumnName, 
      string[] originalColumnValues, string[] liveddColumnNames, string[] liveddDataTypes, string[] liveddItems) {
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      List<LiveDropDownResult> results = meta.GetLiveDropDownResults(
        changedColumnName, originalColumnValues, liveddColumnNames, liveddDataTypes, liveddItems);
      foreach (var result in results)
        result.ViewString = result.GetHTML();
      return Json(results, JsonRequestBehavior.AllowGet);
    }

    //Called when live dropdown is lifted up
    [CommonActionFilter]
    public JsonResult GetLiveSubcolumns(string commonDataTableName, string changedColumnName, string changedColumnValue) {
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      List<ListColumnResult> results = meta.GetLiveListColumnResults(changedColumnName, changedColumnValue);
      foreach (var result in results)
        result.ViewString = result.UsedListColumnInfo.GetHTML(result.DataValue);
      return Json(results, JsonRequestBehavior.AllowGet);
    }

    //Called when live dropdown is lifted up
    [CommonActionFilter]
    public JsonResult GetForeignInfo(string commonDataTableName, string changedColumnName, string changedColumnValue) {
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      if (!meta.IsForeignInfoColumn(changedColumnName))
        return Json(null, JsonRequestBehavior.AllowGet);
      ForeignInfoColumnInfo info = meta.GetForeignInfoColumn(changedColumnName);
      if (info == null)
        return Json(null, JsonRequestBehavior.AllowGet);
      List<ForeignInfoResult> results = new List<ForeignInfoResult>();
      if (string.IsNullOrWhiteSpace(changedColumnValue)) {
        List<DataColumn> columns = info.GetAffectedColumns();
        foreach (var column in columns)
          results.Add(new ForeignInfoResult(new KeyValuePair<string, object>(column.ColumnName, string.Empty)));
      } else {
        List<KeyValuePair<string, object>> tempResults = info.GetForeignDataDictionary(changedColumnName, changedColumnValue);
        foreach (var tempResult in tempResults)
          results.Add(new ForeignInfoResult(tempResult));
      }
      return Json(results, JsonRequestBehavior.AllowGet);
    }

    //Called for add or delete, when a list-column button (add or delete) is pressed
    [CommonActionFilter]
    public JsonResult GetSubcolumnItems(string commonDataTableName, string columnName, string dataValue, string lcType, int itemNo, string addString, bool isCopy) {
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      ListColumnResult result = new ListColumnResult(null, dataValue); //no need to have columnName here, only dataValue is needed
      if(!result.AddCopyOrDeleteDataValue(itemNo, meta, columnName, addString, lcType, isCopy))
        return Json(result, JsonRequestBehavior.AllowGet); //if not successful, no need to take time and built HTML string      
      ListColumnInfo info = meta.GetListColumnInfo(columnName); //Need to get the info from the columnName
      result.ViewString = info.GetHTML(result.DataValue); //if successful, do not forget to recreate the HTML string before return
      return Json(result, JsonRequestBehavior.AllowGet);
    }

    //Called when list-column input is focused out (change the ListColumnItem description)
    //IMPORTANT: commonDataTableName parameter, though not used in the function, is necessary to have because of the CommonActionFilter. Do not remove
    [CommonActionFilter]
    public JsonResult UpdateSubcolumnItemsDescription(string commonDataTableName, string columnName, int rowNo, int columnNo, string dataValue, string inputValue, string lcType) {
      ListColumnResult result = new ListColumnResult(null, dataValue);
      MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
      ListColumnInfo info = meta.GetListColumnInfo(columnName);
      result.UpdateDataValue(info, inputValue, rowNo, columnNo, lcType); //the result does not matter here, just return it anyway      
      return Json(result, JsonRequestBehavior.AllowGet); //Return the result, successful or not
    }
    #endregion
  }
}

//using System.IO;
//using System.Runtime.InteropServices;
//using Excel = Microsoft.Office.Interop.Excel;
//
//    [CommonActionFilter]
//    //Base code by: ZYS
//    //Edited by: Ian
//    public ActionResult ExportToExcel(string commonDataTableName) {
//      try {
//        DataTable tbl = (DataTable)TempData["DataTableForExcel"];
//        string tempFolderPath = Server.MapPath("~/temp");
//        Directory.CreateDirectory(tempFolderPath); //the checking if the directory exists is already inside.
//        string excelFilePath = Path.Combine(tempFolderPath, commonDataTableName + "_Temp.xls");//temp file
//        string mimeType = "application/vnd.ms-excel";
//        byte[] buff = null;
//        buff = exportToExcelFile(tbl, excelFilePath);
//        if (System.IO.File.Exists(excelFilePath))
//          System.IO.File.Delete(excelFilePath);
//        TempData["DataTableForExcel"] = tbl; //to ensure multiple calls do not generate error
//        return File(buff, mimeType);
//      } catch (Exception ex) {
//        string exStr = ex.ToString();
//        LogHelper.Error(User.Identity.Name, null, Aiwe.DH.Mvc, Aiwe.DH.MvcCommonControllerName,
//          commonDataTableName, "ExportToExcel", null, exStr);
//#if DEBUG
//        ViewBag.ErrorMessage = exStr;
//#endif
//        return View(Aiwe.DH.ErrorViewName);
//      }
//    }

////Base code by: ZYS
////Edited by: Ian
//private byte[] exportToExcelFile(DataTable tbl, string excelFilePath) {
//  if (tbl == null) {
//    return null;
//  }
//  if (System.IO.File.Exists(excelFilePath))
//    System.IO.File.Delete(excelFilePath);
//  try {
//    // load excel, and create a new workbook
//    var excelApp = new Excel.Application();
//    excelApp.Workbooks.Add();

//    // single worksheet
//    Excel._Worksheet workSheet = excelApp.ActiveSheet;
//    workSheet.Name = "ExportData";

//    // column headings
//    for (var i = 0; i < tbl.Columns.Count; i++)
//      workSheet.Cells[1, i + 1] = tbl.Columns[i].ColumnName;

//    // rows
//    for (var i = 0; i < tbl.Rows.Count; i++)
//      // to do: format datetime values before printing
//      for (var j = 0; j < tbl.Columns.Count; j++)
//        workSheet.Cells[i + 2, j + 1] = tbl.Rows[i][j];

//    workSheet.SaveAs(excelFilePath);
//    excelApp.Quit();
//    Marshal.ReleaseComObject(workSheet); //Both the worksheet and the excelApp
//    Marshal.ReleaseComObject(excelApp); //must be released, otherwise the excel application accummulates

//    System.Threading.Thread.Sleep(new TimeSpan(0, 0, 1));//must sleep for a while

//    byte[] buff = null;
//    FileStream fs = new FileStream(excelFilePath,
//                                   FileMode.Open,
//                                   FileAccess.Read);
//    BinaryReader br = new BinaryReader(fs);
//    long numBytes = new FileInfo(excelFilePath).Length;
//    buff = br.ReadBytes((int)numBytes);
//    br.Close();
//    br.Dispose();

//    return buff;
//  } catch { //error log is handled by the action
//    throw;
//  }
//}

//[ValidateInput(false)]
//[HttpPost]
//[CommonActionFilter]
//public ActionResult GroupDetails(string commonDataTableName, string[] identifierKeys, string[] identifierValues, FormCollection collections, int? page = 1, bool isGroupDeletion = false) {
//  Dictionary<string, string> dictCollections = AiweTranslationHelper.FormCollectionToDictionary(collections);
//  MetaInfo meta = AiweTableHelper.GetMeta(commonDataTableName);
//  var identifiers = AiweTranslationHelper.GetIdentifiers(meta, identifierKeys, identifierValues);
//  AiweFilterGroupDetailsModel model = getFilterGroupDetailsModel(commonDataTableName, identifiers, page, dictCollections, loadAllData: false, isGroupDeletion: isGroupDeletion);
//  return null;
//}
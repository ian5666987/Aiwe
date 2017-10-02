using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using Extension.Database.SqlServer;
using Extension.String;
using Aibe.Helpers;
using Aibe.Models;
using Aibe.Models.Core;
using Aiwe.ActionFilters;
using Aiwe.Extensions;
using Aiwe.Models;
using Aiwe.Helpers;
using Excel = Microsoft.Office.Interop.Excel;

namespace Aiwe.Controllers { //TODO check if this is already correct
  public class CommonController : Controller {
    [CommonActionFilter]
    //Get does not have filter
    public ActionResult Index(string tableName, int? page) { //Where all common tables are returned as list
      return View(getFilterIndexModel(tableName, page, null));
    }

    [HttpPost]
    [CommonActionFilter]
    public ActionResult Index(string tableName, int? commonDataFilterPage, FormCollection collections) { //do not change the name commonDataFilterPage to page
      return View(getFilterIndexModel(tableName, commonDataFilterPage, collections));
    }

    private AiweFilterIndexModel getFilterIndexModel(string tableName, int? page, FormCollection collections) {
      TempData.Clear();
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      FilterIndexModel model = new FilterIndexModel(meta, page, collections == null ? 
        null : AiweTranslationHelper.FormCollectionToDictionary(collections));

      //Get index info
      AiweQueryHelper.HandleUserRelatedScripting(model.QueryScript, User, meta.UserRelatedFilters);
      model.CompleteModelAndData();
      return new AiweFilterIndexModel(meta, User, model);
    }

    [CommonActionFilter]
    public ActionResult Create(string tableName) {
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      AiweCreateEditModel model = new AiweCreateEditModel(meta, Aibe.DH.CreateActionName, null);
      return View(model);
    }

    [HttpPost]
    [CommonActionFilter]
    public ActionResult Create(string tableName, FormCollection collections) {
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      DateTime now = DateTime.Now;
      Dictionary<string, string> dictCollections = AiweTranslationHelper.FormCollectionToDictionary(collections);

      foreach (var item in dictCollections)
        ModelState.Add(item.Key, new ModelState());

      List<string> checkExclusions = new List<string> { Aibe.DH.TableNameParameterName }; //different per Action, because of additional item in the ModelState

      //Check model state's validity
      Dictionary<string, string> errorDict = new AiweCheckerHelper().CheckModelValidity(Aiwe.DH.TableModelClassPrefix, tableName, meta.ArrangedDataColumns, 
        dictCollections, ModelState.Keys.ToList(), meta, checkExclusions, AiweUserHelper.UserIsDeveloper(User), now, Aibe.DH.CreateActionName);
      AiweTranslationHelper.FillModelStateWithErrorDictionary(ModelState, errorDict);
      if (!ModelState.IsValid)
        return View(new AiweCreateEditModel(meta, Aibe.DH.CreateActionName, null));

      //Only if model state is correct that we could get valid key infos safely
      var completeKeyInfo = KeyInfoHelper.GetCompleteKeyInfo(tableName, dictCollections, dictCollections.Keys, meta.ArrangedDataColumns, filterStyle: false, meta: meta, actionType: Aibe.DH.CreateActionName);
      if (completeKeyInfo == null || completeKeyInfo.ValidKeys == null || !completeKeyInfo.ValidKeys.Any()) {
        ViewBag.ErrorMessage = string.Concat("Invalid/Empty parameters for [", tableName, "]");
        return View("Error");
      }

      //TODO Beware of duplicate record because the client clicks more than once
      // -> If identical record is found, display other page to ask the client to confirm
      //TODO Beware of incomplete input, because the client does not fill everything
      // -> likely done in the meta table

      BaseScriptModel scriptModel = LogicHelper.CreateInsertScriptModel(tableName, completeKeyInfo, dictCollections, now, meta);
      object generatedId = SQLServerHandler.ExecuteScalar(scriptModel.Script, Aibe.DH.DataDBConnectionString, scriptModel.Pars);
      AiweFileHelper.SaveAttachments(Request, Server.MapPath("~/Images/" + tableName + "/" + generatedId?.ToString()));
      return RedirectToAction("Index", new { tableName = tableName });
    }

    //Likely used by filter and create edit
    [CommonActionFilter]
    public ActionResult Edit(string tableName, int id) {
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      Dictionary<string, object> objectDictionary = LogicHelper.FillDetailsFromTableToObjectDictionary(tableName, id);
      AiweCreateEditModel model = new AiweCreateEditModel(meta, Aibe.DH.EditActionName, LogicHelper.ObjectDictionaryToStringDictionary(objectDictionary));
      return View(model);
    }

    [HttpPost]
    [CommonActionFilter]
    public ActionResult Edit(string tableName, int cid, FormCollection collections) {
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      ModelState.Remove("cid"); //Again, so that it will replaced with the given collection, using capital "C" -> "Cid"
      DateTime now = DateTime.Now;
      Dictionary<string, string> dictCollections = AiweTranslationHelper.FormCollectionToDictionary(collections);

      foreach (var item in dictCollections)
        ModelState.Add(item.Key, new ModelState());

      List<string> checkExclusions = new List<string> { Aibe.DH.TableNameParameterName }; //different per Action, because of additional item in the ModelState

      //Check model state's validity
      Dictionary<string, string> errorDict = new AiweCheckerHelper().CheckModelValidity(Aiwe.DH.TableModelClassPrefix, tableName, meta.ArrangedDataColumns, 
        dictCollections, ModelState.Keys.ToList(), meta, checkExclusions, AiweUserHelper.UserIsDeveloper(User), now, Aibe.DH.EditActionName);
      AiweTranslationHelper.FillModelStateWithErrorDictionary(ModelState, errorDict);
      if (!ModelState.IsValid) {
        AiweCreateEditModel model = new AiweCreateEditModel(meta, Aibe.DH.EditActionName, dictCollections);
        return View(model);
      }

      var filteredKeys = dictCollections.Keys.Where(x => !x.EqualsIgnoreCase("Cid")); //everything filled but the Cid
      var completeKeyInfo = KeyInfoHelper.GetCompleteKeyInfo(tableName, dictCollections, filteredKeys, meta.ArrangedDataColumns, filterStyle: false, meta: meta, actionType: Aibe.DH.EditActionName);
      if (completeKeyInfo == null || completeKeyInfo.ValidKeys == null || !completeKeyInfo.ValidKeys.Any()) {
        ViewBag.ErrorMessage = string.Concat("Invalid/Empty parameters for [", tableName, "]");
        return View("Error");
      }

      BaseScriptModel scriptModel = LogicHelper.CreateUpdateScriptModel(tableName, cid, completeKeyInfo, dictCollections, now);
      SQLServerHandler.ExecuteScript(scriptModel.Script, Aibe.DH.DataDBConnectionString, scriptModel.Pars);
      AiweFileHelper.SaveAttachments(Request, Server.MapPath("~/Images/" + tableName + "/" + cid));
      return RedirectToAction("Index", new { tableName = tableName });
    }

    [CommonActionFilter]
    public ActionResult Delete(string tableName, int id) { //Where all common tables details are returned and can be deleted
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      Dictionary<string, object> objectDictionary = LogicHelper.FillDetailsFromTableToObjectDictionary(tableName, id);
      AiweDetailsModel model = new AiweDetailsModel(meta, id, LogicHelper.ObjectDictionaryToStringDictionary(objectDictionary));
      return View(model);
    }

    [HttpPost]
    [CommonActionFilter]
    [ActionName("Delete")]
    public ActionResult DeletePost(string tableName, int id) { //Where all common tables deletes are returned and can be deleted
      LogicHelper.DeleteItem(tableName, id); //Currently do not return any error
      return RedirectToAction("Index", new { tableName = tableName });
    }

    //Later add filter to check if a user has right to see this table
    [CommonActionFilter]
    public ActionResult Details(string tableName, int id) { //there must be a number named cid (common Id)
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      Dictionary<string, object> objectDictionary = LogicHelper.FillDetailsFromTableToObjectDictionary(tableName, id);
      AiweDetailsModel model = new AiweDetailsModel(meta, id, LogicHelper.ObjectDictionaryToStringDictionary(objectDictionary));
      return View(model);
    }

    [CommonActionFilter]
    //Base code by: ZYS
    //Edited by: Ian
    public ActionResult ExportToExcel(string tableName) {
      try {
        DataTable tbl = (DataTable)TempData["DataTableForExcel"];
        string tempFolderPath = Server.MapPath("~/temp");
        Directory.CreateDirectory(tempFolderPath); //the checking if the directory exists is already inside.          
        string excelFilePath = Path.Combine(tempFolderPath, tableName + "_Temp.xls");//temp file
        string mimeType = "application/vnd.ms-excel";
        byte[] buff = null;
        buff = exportToExcelFile(tbl, excelFilePath);
        if (System.IO.File.Exists(excelFilePath))
          System.IO.File.Delete(excelFilePath);
        TempData["DataTableForExcel"] = tbl; //to ensure multiple calls do not generate error
        return File(buff, mimeType);
      } catch (Exception ex) {
        string exStr = ex.ToString();
        LogHelper.Error(User.Identity.Name, null, "MVC", "Common",
          tableName, "ExportToExcel", null, exStr);
#if DEBUG
        ViewBag.ErrorMessage = exStr;
#endif
        return View("Error");
      }
    }

    #region live javascript use
    //Called when live dropdown is lifted up
    [CommonActionFilter]
    public JsonResult GetLiveDropDownItems(string tableName, string changedColumnName, string[] originalColumnValues,
      string[] liveddColumnNames, string[] liveddDataTypes, string[] liveddItems) {
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      List<LiveDropDownResult> results = meta.GetLiveDropDownResults(
        changedColumnName, originalColumnValues, liveddColumnNames, liveddDataTypes, liveddItems);
      foreach (var result in results)
        result.ViewString = result.BuildDropdownString();
      return Json(results, JsonRequestBehavior.AllowGet);
    }

    //Called when live dropdown is lifted up
    [CommonActionFilter]
    public JsonResult GetLiveSubcolumns(string tableName, string changedColumnName, string changedColumnValue) {
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      List<ListColumnResult> results = meta.GetLiveListColumnResults(changedColumnName, changedColumnValue);
      foreach (var result in results)
        result.ViewString = result.UsedListColumnInfo.GetHTML(result.DataValue);
      return Json(results, JsonRequestBehavior.AllowGet);
    }

    //Called for add or delete, when a list-column button (add or delete) is pressed
    [CommonActionFilter]
    public JsonResult GetSubcolumnItems(string tableName, string columnName, string dataValue, string lcType, int deleteNo, string addString) {
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      ListColumnResult result = new ListColumnResult(null, dataValue); //no need to have columnName here, only dataValue is needed
      if(!result.AddOrDeleteDataValue(deleteNo, meta, columnName, addString, lcType))
        return Json(result, JsonRequestBehavior.AllowGet); //if not successful, no need to take time and built HTML string      
      ListColumnInfo info = meta.GetListColumnInfo(columnName); //Need to get the info from the columnName
      result.ViewString = info.GetHTML(result.DataValue); //if successful, do not forget to recreate the HTML string before return
      return Json(result, JsonRequestBehavior.AllowGet);
    }

    //Called when list-column input is focused out (change the ListColumnItem description)
    //IMPORTANT: tableName parameter, though not used in the function, is necessary to have because of the CommonActionFilter. Do not remove
    [CommonActionFilter]
    public JsonResult UpdateSubcolumnItemsDescription(string tableName, int rowNo, int columnNo, string dataValue, string inputValue, string lcType) {
      ListColumnResult result = new ListColumnResult(null, dataValue);
      result.UpdateDataValue(inputValue, rowNo, columnNo, lcType); //the result does not matter here, just return it anyway      
      return Json(result, JsonRequestBehavior.AllowGet); //Return the result, successful or not
    }
    #endregion

    #region private methods
    //Base code by: ZYS
    //Edited by: Ian
    private byte[] exportToExcelFile(DataTable tbl, string excelFilePath) {
      if (tbl == null) {
        return null;
      }
      if (System.IO.File.Exists(excelFilePath))
        System.IO.File.Delete(excelFilePath);
      try {
        // load excel, and create a new workbook
        var excelApp = new Excel.Application();
        excelApp.Workbooks.Add();

        // single worksheet
        Excel._Worksheet workSheet = excelApp.ActiveSheet;
        workSheet.Name = "ExportData";

        // column headings
        for (var i = 0; i < tbl.Columns.Count; i++)
          workSheet.Cells[1, i + 1] = tbl.Columns[i].ColumnName;

        // rows
        for (var i = 0; i < tbl.Rows.Count; i++)
          // to do: format datetime values before printing
          for (var j = 0; j < tbl.Columns.Count; j++)
            workSheet.Cells[i + 2, j + 1] = tbl.Rows[i][j];

        workSheet.SaveAs(excelFilePath);
        excelApp.Quit();
        Marshal.ReleaseComObject(workSheet); //Both the worksheet and the excelApp
        Marshal.ReleaseComObject(excelApp); //must be released, otherwise the excel application accummulates
                        
        System.Threading.Thread.Sleep(new TimeSpan(0, 0, 1));//must sleep for a while

        byte[] buff = null;
        FileStream fs = new FileStream(excelFilePath,
                                       FileMode.Open,
                                       FileAccess.Read);
        BinaryReader br = new BinaryReader(fs);
        long numBytes = new FileInfo(excelFilePath).Length;
        buff = br.ReadBytes((int)numBytes);
        br.Close();
        br.Dispose();

        return buff;
      } catch { //error log is handled by the action
        throw;
      }
    }
    #endregion
  }
}
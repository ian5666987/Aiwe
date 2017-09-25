using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Text;
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
      TempData.Clear();
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      FilterIndexModel model = new FilterIndexModel(meta, page, null);

      //Get index info
      AiweQueryHelper.HandleUserRelatedScripting(model.QueryScript, User, meta.UserRelatedFilters);
      model.CompleteModelAndData();
      FilterIndexInfo info = new FilterIndexInfo(meta, User, model);
      return View(info);
    }

    [HttpPost]
    [CommonActionFilter]
    public ActionResult Index(string tableName, int? commonDataFilterPage, FormCollection collections) { //do not change the name commonDataFilterPage to page
      TempData.Clear();
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      Dictionary<string, string> dictCollections = AiweTranslationHelper.FormCollectionToDictionary(collections);
      FilterIndexModel model = new FilterIndexModel(meta, commonDataFilterPage, dictCollections);

      //Filter related stuffs
      if (model.TempDataDict != null && model.TempDataDict.Count > 0)
        AiweTranslationHelper.FillTempDataFromDictionary(TempData, model.TempDataDict);
      ViewBag.FilterNo = model.FilterNo;
      ViewBag.FilterMsg = getFilterMessage(model.TempDataDict, model.FilterNo > 0);

      //Get index info
      AiweQueryHelper.HandleUserRelatedScripting(model.QueryScript, User, meta.UserRelatedFilters);
      model.CompleteModelAndData();
      FilterIndexInfo info = new FilterIndexInfo(meta, User, model);
      return View(info);
    }

    [CommonActionFilter]
    public ActionResult Create(string tableName) {
      TempData.Clear();
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      CreateEditInfo model = new CreateEditInfo(meta, Aibe.DH.CreateActionName);
      return View(model);
    }

    [HttpPost]
    [CommonActionFilter]
    public ActionResult Create(string tableName, FormCollection collections) {
      TempData.Clear();
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      DateTime now = DateTime.Now;
      Dictionary<string, string> dictCollections = AiweTranslationHelper.FormCollectionToDictionary(collections);

      foreach (var item in dictCollections)
        ModelState.Add(item.Key, new ModelState());

      List<string> checkExclusions = new List<string> { Aibe.DH.TableNameParameterName }; //different per Action, because of additional item in the ModelState

      //Check model state's validity
      Dictionary<string, string> errorDict = AiweCheckerHelper.CheckModelValidity(Aiwe.DH.TableModelClassPrefix, tableName, meta.ArrangedDataColumns, 
        dictCollections, ModelState.Keys.ToList(), meta, checkExclusions, AiweUserHelper.UserIsDeveloper(User), now, Aibe.DH.CreateActionName);
      AiweTranslationHelper.FillModelStateWithErrorDictionary(ModelState, errorDict);
      if (!ModelState.IsValid) {
        CreateEditInfo ceInfo = new CreateEditInfo(meta, Aibe.DH.CreateActionName);
        AiweTranslationHelper.FillTempDataFromCollections(TempData, dictCollections, checkExclusions);
        return View(ceInfo);
      }

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
      AiweTranslationHelper.FillTempDataFromObjectDictionary(TempData, objectDictionary);
      CreateEditInfo model = new CreateEditInfo(meta, Aibe.DH.EditActionName);
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
      Dictionary<string, string> errorDict = AiweCheckerHelper.CheckModelValidity(Aiwe.DH.TableModelClassPrefix, tableName, meta.ArrangedDataColumns, 
        dictCollections, ModelState.Keys.ToList(), meta, checkExclusions, AiweUserHelper.UserIsDeveloper(User), now, Aibe.DH.EditActionName);
      AiweTranslationHelper.FillModelStateWithErrorDictionary(ModelState, errorDict);
      if (!ModelState.IsValid) {
        CreateEditInfo model = new CreateEditInfo(meta, Aibe.DH.EditActionName);
        AiweTranslationHelper.FillTempDataFromCollections(TempData, dictCollections, checkExclusions);
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
      TempData.Clear();
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      DetailsInfo model = new DetailsInfo(meta, id);
      Dictionary<string, object> objectDictionary = LogicHelper.FillDetailsFromTableToObjectDictionary(tableName, id);
      AiweTranslationHelper.FillTempDataFromObjectDictionary(TempData, objectDictionary); //TempData is prepared inside!
      return View(model);
    }

    [HttpPost]
    [CommonActionFilter]
    [ActionName("Delete")]
    public ActionResult DeletePost(string tableName, int id) { //Where all common tables deletes are returned and can be deleted
      TempData.Clear();
      LogicHelper.DeleteItem(tableName, id); //Currently do not return any error
      return RedirectToAction("Index", new { tableName = tableName });
    }

    //Later add filter to check if a user has right to see this table
    [CommonActionFilter]
    public ActionResult Details(string tableName, int id) { //there must be a number named cid (common Id)
      TempData.Clear();
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      DetailsInfo model = new DetailsInfo(meta, id);
      Dictionary<string, object> objectDictionary = LogicHelper.FillDetailsFromTableToObjectDictionary(tableName, id);
      AiweTranslationHelper.FillTempDataFromObjectDictionary(TempData, objectDictionary); //TempData is prepared inside!
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
    public JsonResult GetLiveDropdownItems(string tableName, string changedColumnName, string[] originalColumnValues,
      string[] liveddColumnNames, string[] liveddDataTypes, string[] liveddItems) {

      MetaInfo meta = AiweTableHelper.GetMeta(tableName);

      Dictionary<string, List<string>> dropdowns = new Dictionary<string, List<string>>();
      int count = 0;
      Dictionary<string, DropdownPassedArguments> passedColumnsAndValues = 
        new Dictionary<string, DropdownPassedArguments>();
      foreach (var col in liveddColumnNames) {
        DropdownPassedArguments arg = new DropdownPassedArguments {
          Value = liveddItems.Length > count ? liveddItems[count] : null,
          OriginalValue = originalColumnValues[count],
          DataType = liveddDataTypes[count]          
        };
        passedColumnsAndValues.Add(col, arg);
        count++;
      }
      count = 0;
      foreach (var col in liveddColumnNames) {
        if (col == changedColumnName) { //skipped
          count++;
          continue;
        }

        //check if affected
        DropDownInfo info = meta.GetCreateEditDropDownColumnInfo(col);
        if (info == null || !info.IsValid || !meta.IsCreateEditDropDownColumnAffectedBy(col, changedColumnName)) {
          count++;
          continue; //if not affected, then leave it be...
        }

        List<string> dropdown = AiweDropDownHelper.CreateLiveCreateEditDropDownFor(
          tableName, col, originalColumnValues[count], liveddDataTypes[count], passedColumnsAndValues);
        dropdowns.Add(col, dropdown);
        count++;
      }

      List<LiveDropDownResult> resultPairs = new List<LiveDropDownResult>();
      foreach (var dd in dropdowns) {
        DropdownPassedArguments arg = passedColumnsAndValues.ContainsKey(dd.Key) ?
          passedColumnsAndValues[dd.Key] : null;
        string ddStr = buildDropdownString(dd.Key, dd.Value, arg?.OriginalValue?.ToString(), arg);
        LiveDropDownResult res = new LiveDropDownResult {
          ColumnName = dd.Key,
          HTMLString = ddStr,
        };
        resultPairs.Add(res);
      }

      return Json(resultPairs, JsonRequestBehavior.AllowGet);
    }

    //Called when live dropdown is lifted up
    [CommonActionFilter]
    public JsonResult GetLiveSubcolumns(string tableName, string changedColumnName, string changedColumnValue) {
      //for the given table and column, first find in the
      List<ListColumnResult> results = new List<ListColumnResult>();
      if (string.IsNullOrWhiteSpace(changedColumnValue))
        return Json(results, JsonRequestBehavior.AllowGet);

      MetaInfo meta = AiweTableHelper.GetMeta(tableName);

      var affectedColumnNames = meta.ArrangedDataColumns
        .Where(x => meta.IsListColumn(x.ColumnName) && meta.IsListColumnAffectedBy(x.ColumnName, changedColumnName))
        .Select(x => x.ColumnName)
        .ToList();

      foreach(var affectedColumnName in affectedColumnNames) {
        ListColumnResult result = new ListColumnResult() {
          IsSuccessful = false,
          Name = affectedColumnName,
        };

        //I need to get the info from the affected columnName
        ListColumnInfo info = meta.GetListColumnInfo(affectedColumnName);
        if (info == null || !info.IsValid)
          continue;

        //From the given info, perform searches to get the wanted data value!
        string newDataValue = string.Empty;
        bool extractResult = info.GetRefDataValue(changedColumnName, changedColumnValue, out newDataValue);
        if (!extractResult)
          continue;
        result.DataValue = newDataValue;

        //And from the dataValue, creates the HTML
        result.HTMLString = info.GetHTML(newDataValue);

        //Add the successful result
        result.IsSuccessful = true;
        results.Add(result); //only return data that is successful
      }

      return Json(results, JsonRequestBehavior.AllowGet);
    }

    //Called for add or delete, when a list-column button (add or delete) is pressed
    [CommonActionFilter]
    public JsonResult GetSubcolumnItems(string tableName, string columnName, string dataValue, string lcType, int deleteNo, string addString) {
      ListColumnResult result = new ListColumnResult() {
        IsSuccessful = false,
        DataValue = dataValue,
      };
      MetaInfo meta = AiweTableHelper.GetMeta(tableName);
      string newDataValue = dataValue;

      if (deleteNo == 0) { //add        
        //do something for checking add
        //if this column is supposed to be checkList, then one check type, else, another check type
        if (!meta.IsListColumn(columnName) || string.IsNullOrWhiteSpace(addString))
          return Json(result, JsonRequestBehavior.AllowGet);

        var addParts = addString.Split(';').Select(x => x.Trim()).ToList();
        List<ListColumnItem> items = new List<ListColumnItem>();
        
        foreach (var addPart in addParts) {
          ListColumnItem item = new ListColumnItem(addPart, lcType);
          if (item.IsValid)
            items.Add(item);
        }

        if (items.Count <= 0)
          return Json(result, JsonRequestBehavior.AllowGet);

        //Add this point, only valid add strings are used
        var validAddString = string.Join(";", items.Select(x => x.CurrentDesc));

        //do something to really add
        newDataValue = string.Concat(string.IsNullOrWhiteSpace(dataValue) ? string.Empty : dataValue + ";", validAddString);
      } else { //delete
        int deleteIndex = deleteNo - 1;
        var dataParts = dataValue.Split(';').Select(x => x.Trim()).ToList();
        dataParts.RemoveAt(deleteIndex);
        newDataValue = string.Join(";", dataParts);        
      }

      //I need to get the info from the columnName
      ListColumnInfo info = meta.GetListColumnInfo(columnName);

      result.DataValue = newDataValue;
      result.HTMLString = info.GetHTML(newDataValue);
      result.IsSuccessful = true;

      return Json(result, JsonRequestBehavior.AllowGet);
    }

    //Called when list-column input is focused out (change the ListColumnItem description)
    [CommonActionFilter]
    public JsonResult UpdateSubcolumnItemsDescription(string tableName, string columnName, string inputPart, 
      string dataValue, bool isText, int inputNo, string inputValue, string lcType) {
      ListColumnResult result = new ListColumnResult() {
        IsSuccessful = false,
        DataValue = dataValue,
      };
      string newDataValue = dataValue;

      if (inputNo < 1)
        return Json(result, JsonRequestBehavior.AllowGet);

      int inputIndex = inputNo - 1;

      var dataValueParts = dataValue.Split(';').Select(x => new ListColumnItem(x.Trim(), lcType))
        .Where(x => x.IsValid).ToList();      

      if (inputIndex >= dataValueParts.Count) //no such parts
        return Json(result, JsonRequestBehavior.AllowGet);

      var changedItem = dataValueParts[inputIndex]; //able to get the wanted changed item 

      //now it is to determine which part is actually changed
      switch (inputPart) {
        case "text":
        case "check":
        case "dropdown":
          changedItem.Value = inputValue;
          break;
        case "remarks":
          changedItem.Remarks = inputValue;
          break;
        case "unit":
          changedItem.Ending = inputValue;
          break;          
      }

      newDataValue = string.Join(";", dataValueParts.Select(x => x.CurrentDesc));        
      result.DataValue = newDataValue;
      result.IsSuccessful = true;

      return Json(result, JsonRequestBehavior.AllowGet);
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

    private string buildDropdownString(string columnName, List<string> values, string originalValue, DropdownPassedArguments arg) {
      StringBuilder sb = new StringBuilder();
      sb.Append("<select class=\"form-control form-control-common-plus common-column-dropdown\" id=\"");
      sb.Append("common-column-dropdown-" + columnName);
      sb.Append("\" name=\"");
      sb.Append(columnName);
      string prevValue = arg == null || arg.Value == null || string.IsNullOrWhiteSpace(arg.Value.ToString()) ?
        string.Empty : arg.Value.ToString();
      if (string.IsNullOrWhiteSpace(prevValue))
        sb.Append("\"><option selected=\"selected\"></option>\n");
      else
        sb.Append("\"><option value=\"\"></option>\n");
      if (values != null) { //if the null is put for the second time, then you cannot choose as freely TODO probably should return to everything?
        if (!string.IsNullOrWhiteSpace(originalValue) && !values.Contains(originalValue))
          values.Insert(0, originalValue);
        foreach (var val in values) {
          sb.Append("<option value=\"");
          sb.Append(val);
          if (!string.IsNullOrWhiteSpace(prevValue) && val == prevValue)
            sb.Append("\" selected=\"selected");
          sb.Append("\">");
          sb.Append(val);
          sb.Append("</option>\n");
        }
      }
      sb.Append("</select>");
      return sb.ToString();
    }


    private string getFilterMessage(Dictionary<string, string> tempDataDict, bool hasFilter) {
      //KeyValuePair<int, string> kvp = new KeyValuePair<int, string>();
      string msg = string.Empty;
      if (hasFilter) {
        int count = 0;
        foreach (var filterMsg in tempDataDict) {
          if (count > 0)
            msg += Environment.NewLine;
          msg += filterMsg.Key + ": " + filterMsg.Value;
          count++;
        }
      }
      return msg;
    }
    #endregion
  }
}

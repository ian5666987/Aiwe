using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;
using Extension.Database;
using Extension.String;
using Aibe;
using Aibe.Helpers;
using Aibe.Models;
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
      MetaInfo meta = TableHelper.GetMeta(tableName);
      List<UserRelatedFilterInfo> userRelatedInfo = meta.UserRelatedFilters;
      FilterIndexInfo model;

      int usedPage = page.HasValue && page.Value > 0 ? page.Value : 1;
      using (SqlConnection conn = new SqlConnection(DH.DataDBConnectionString)) {
        conn.Open();

        //Script initialization
        string selectScript = "SELECT";
        StringBuilder queryScript = new StringBuilder(string.Concat("FROM [", meta.TableName, "]"));

        //Finalize
        model = finalizeAndExecuteScript(selectScript, queryScript, conn, pars: null, copiesPars: null, meta: meta, usedPage: usedPage, userRelatedInfos: userRelatedInfo);

        conn.Close();
      }
      
      return View(model);
    }

    [HttpPost]
    [CommonActionFilter]
    public ActionResult Index(string tableName, int? commonDataFilterPage, FormCollection collections) {
      TempData.Clear();
      MetaInfo meta = TableHelper.GetMeta(tableName);
      List<UserRelatedFilterInfo> userRelatedInfo = meta.UserRelatedFilters;
      FilterIndexInfo model;
      DateTime now = DateTime.Now;

      int usedPage = commonDataFilterPage.HasValue && commonDataFilterPage.Value > 0 ? commonDataFilterPage.Value : 1;
      using (SqlConnection conn = new SqlConnection(DH.DataDBConnectionString)) {
        conn.Open();

        //Script initialization
        string selectScript = "SELECT";
        StringBuilder queryScript = new StringBuilder(string.Concat("FROM [", meta.TableName, "]"));

        //Filters
        List<SqlParameter> pars = new List<SqlParameter>();
        List<SqlParameter> copiesPars = new List<SqlParameter>(); //WARNING! if not copied, the complete script cannot be run as the Sql Pars have been used by this countScript
        addFiltersOnScript(queryScript, meta, conn, collections, pars, copiesPars, now);

        //Finalize
        model = finalizeAndExecuteScript(selectScript, queryScript, conn, pars, copiesPars, meta, usedPage, userRelatedInfo);

        conn.Close();
      }

      return View(model);
    }

    //public ActionResult Error() {
    //  return View();
    //}

    [CommonActionFilter]
    public ActionResult Create(string tableName) {
      TempData.Clear();
      MetaInfo meta = TableHelper.GetMeta(tableName);
      CreateEditInfo model = new CreateEditInfo(meta, DH.CreateActionName);
      return View(model);
    }

    [HttpPost]
    [CommonActionFilter]
    public ActionResult Create(string tableName, FormCollection collections) {
      TempData.Clear();
      MetaInfo meta = TableHelper.GetMeta(tableName);
      DateTime now = DateTime.Now;

      foreach (var item in collections)
        ModelState.Add(new KeyValuePair<string, ModelState>(
          item.ToString(), new ModelState()));

      List<string> checkExclusions = new List<string> { DH.TableNameParameterName }; //different per Action, because of additional item in the ModelState

      //Check model state's validity
      checkModelValidity(tableName, meta.DataColumns, collections, meta, checkExclusions, now, DH.CreateActionName);
      if (!ModelState.IsValid) {
        CreateEditInfo ceInfo = new CreateEditInfo(meta, DH.CreateActionName);
        fillDetailsFromCollectionsToTempData(collections, checkExclusions);        
        return View(ceInfo);
      }

      //Only if model state is correct that we could get valid key infos safely
      var completeKeyInfo = getCompleteKeyInfo(tableName, collections, collections.AllKeys, meta.DataColumns, filterStyle: false, meta: meta, actionType: DH.CreateActionName);

      if (completeKeyInfo == null || completeKeyInfo.ValidKeys == null || !completeKeyInfo.ValidKeys.Any()) {
        ViewBag.ErrorMessage = string.Concat("Invalid/Empty parameters for [", tableName, "]");
        return View("Error");
      }

      //TODO Beware of duplicate record because the client clicks more than once
      // -> If identical record is found, display other page to ask the client to confirm
      //TODO Beware of incomplete input, because the client does not fill everything
      // -> likely done in the meta table

      object generatedId = (int)0;
      using (SqlConnection conn = new SqlConnection(DH.DataDBConnectionString)) {
        conn.Open();
        StringBuilder openingScript = new StringBuilder(string.Concat("INSERT INTO [", tableName, "] ("));
        StringBuilder insertParNamesScript = new StringBuilder();

        int count = 0;
        List<SqlParameter> pars = new List<SqlParameter>();        
        foreach (var validKeyInfo in completeKeyInfo.ValidKeys)
          addInsertParameter(openingScript, insertParNamesScript, validKeyInfo, pars, collections, now, ref count, meta);

        foreach (var nullifiedKeyInfo in completeKeyInfo.NullifiedKeys) { //This is needed in case any of the nullified key info is actually among the timestamp columns
          if (nullifiedKeyInfo.IsTimeStamp) //for create, if it is timestamp, then creates it no matter what
            addInsertParameter(openingScript, insertParNamesScript, nullifiedKeyInfo, pars, collections, now, ref count, meta); //inside, the time stamp is taken cared of
          if (nullifiedKeyInfo.IsAutoGenerated) //for create, if it is timestamp, then creates it no matter what
            addInsertParameter(openingScript, insertParNamesScript, nullifiedKeyInfo, pars, collections, now, ref count, meta);
          //TODO not needed for now, but just in case
        }

        using (SqlCommand command = new SqlCommand(string.Concat(
          openingScript, ") VALUES(", insertParNamesScript.ToString(), "); SELECT SCOPE_IDENTITY()"), conn)) {
          command.Parameters.AddRange(pars.ToArray());
          //command.ExecuteNonQuery();
          generatedId = command.ExecuteScalar();
        }

        conn.Close();
      }

      FileHelper.SaveAttachments(Request, Server.MapPath("~/Images/" + tableName + "/" + generatedId?.ToString()));
      return RedirectToAction("Index", new { tableName = tableName });
    }

    //Likely used by filter and create edit
    [CommonActionFilter]
    public ActionResult Edit(string tableName, int id) {
      MetaInfo meta = TableHelper.GetMeta(tableName);
      fillDetailsFromTableToTempData(tableName, id); //TempData is prepared inside!
      CreateEditInfo model = new CreateEditInfo(meta, DH.EditActionName);
      return View(model);
    }

    [HttpPost]
    [CommonActionFilter]
    public ActionResult Edit(string tableName, int cid, FormCollection collections) {
      MetaInfo meta = TableHelper.GetMeta(tableName);
      ModelState.Remove("cid"); //Again, so that it will replaced with the given collection, using capital "C" -> "Cid"
      DateTime now = DateTime.Now;

      foreach (var item in collections)
        ModelState.Add(new KeyValuePair<string, ModelState>(
          item.ToString(), new ModelState()));

      List<string> checkExclusions = new List<string> { DH.TableNameParameterName }; //different per Action, because of additional item in the ModelState

      //Check model state's validity
      checkModelValidity(tableName, meta.DataColumns, collections, meta, checkExclusions, now, DH.EditActionName);
      if (!ModelState.IsValid) {
        CreateEditInfo model = new CreateEditInfo(meta, DH.EditActionName);
        fillDetailsFromCollectionsToTempData(collections, checkExclusions);        
        return View(model);
      }

      var filteredKeys = collections.AllKeys.Where(x => !x.EqualsIgnoreCase("Cid")); //everything filled but the Cid
      var completeKeyInfo = getCompleteKeyInfo(tableName, collections, filteredKeys, meta.DataColumns, filterStyle: false, meta: meta, actionType: DH.EditActionName);

      if (completeKeyInfo == null || completeKeyInfo.ValidKeys == null || !completeKeyInfo.ValidKeys.Any()) {
        ViewBag.ErrorMessage = string.Concat("Invalid/Empty parameters for [", tableName, "]");
        return View("Error");
      }

      StringBuilder updateScript = new StringBuilder(string.Concat("UPDATE [", tableName, "] SET "));
      string whereScript = string.Concat(" WHERE [Cid] = ", cid);

      int count = 0;
      List<SqlParameter> pars = new List<SqlParameter>();
      foreach (var validKeyInfo in completeKeyInfo.ValidKeys)
        if (!validKeyInfo.IsAutoGenerated)
          addUpdateParameter(updateScript, validKeyInfo, pars, collections, now, ref count);

      foreach (var nullifiedKeyInfo in completeKeyInfo.NullifiedKeys.Where(x => !x.PureKeyName.EqualsIgnoreCase("Cid")))
        if (nullifiedKeyInfo.IsTimeStamp) //If it is a TimeStamp, update it
          addUpdateParameter(updateScript, nullifiedKeyInfo, pars, collections, now, ref count);
        else if (nullifiedKeyInfo.IsAutoGenerated) {
          //do nothing! cannot be changed with edit
        } else
          addNullUpdateParameter(updateScript, nullifiedKeyInfo, ref count);

      using (SqlConnection conn = new SqlConnection(DH.DataDBConnectionString)) {
        conn.Open();
        using (SqlCommand command = new SqlCommand(string.Concat(
          updateScript, whereScript), conn)) {
          command.Parameters.AddRange(pars.ToArray());
          command.ExecuteNonQuery();
        }

        conn.Close();
      }

      FileHelper.SaveAttachments(Request, Server.MapPath("~/Images/" + tableName + "/" + cid));
      return RedirectToAction("Index", new { tableName = tableName });
    }

    [CommonActionFilter]
    public ActionResult Delete(string tableName, int id) { //Where all common tables details are returned and can be deleted
      TempData.Clear();
      MetaInfo meta = TableHelper.GetMeta(tableName);
      DetailsInfo model = new DetailsInfo(meta, id);
      fillDetailsFromTableToTempData(tableName, id);
      return View(model);
    }

    [HttpPost]
    [CommonActionFilter]
    [ActionName("Delete")]
    public ActionResult DeletePost(string tableName, int id) { //Where all common tables deletes are returned and can be deleted
      TempData.Clear();
      MetaInfo meta = TableHelper.GetMeta(tableName);
      deleteItem(tableName, id); //Currently do not return any error
      return RedirectToAction("Index", new { tableName = tableName });
    }

    //Later add filter to check if a user has right to see this table
    [CommonActionFilter]
    public ActionResult Details(string tableName, int id) { //there must be a number named cid (common Id)
      TempData.Clear();
      MetaInfo meta = TableHelper.GetMeta(tableName);
      DetailsInfo model = new DetailsInfo(meta, id);
      fillDetailsFromTableToTempData(tableName, id);
      return View(model);
    }

    [CommonActionFilter]
    //Base code by: ZYS
    //Edited by: Ian
    public ActionResult ExportToExcel(string tableName) {
      //return View("ActionNotReady");
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

      MetaInfo meta = TableHelper.GetMeta(tableName);

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

        List<string> dropdown = DropDownHelper.CreateLiveCreateEditDropDownFor(
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

      MetaInfo meta = TableHelper.GetMeta(tableName);

      var affectedColumnNames = meta.DataColumns
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
      MetaInfo meta = TableHelper.GetMeta(tableName);
      //if (meta == null)
      //  return Json(result, JsonRequestBehavior.AllowGet);
      //CreateEditInfo model = new CreateEditInfo(meta, SQLServerHandler.GetColumns(DataHolder.DataDBConnectionString, tableName), null);
      //if (model == null)
      //  return Json(result, JsonRequestBehavior.AllowGet);

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
    public byte[] exportToExcelFile(DataTable tbl, string excelFilePath) {
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

    private List<string> getUsedKeys(FormCollection collections, IEnumerable<string> filteredKeys) {
      var usedKeys = new List<string>();

      //Columns must be known! check against the table's columns!
      foreach (var key in filteredKeys) { //only collect the filter number, do nothing at this moment          
        var value = collections[key];
        if (string.IsNullOrWhiteSpace(value))
          continue;
        usedKeys.Add(key);
      }

      return usedKeys;
    }

    private CompleteKeyInfo getCompleteKeyInfo(string tableName, FormCollection collections,
      IEnumerable<string> filteredKeys, List<DataColumn> columns,
      bool filterStyle = false, MetaInfo meta = null, string actionType = null) {
      var usedKeys = getUsedKeys(collections, filteredKeys);
      List<string> columnNames = columns.Select(x => x.ColumnName).ToList();
      var keyInfos = usedKeys.Select(x => new KeyInfo(tableName, x)).Where(x => columnNames.Any(y => y.EqualsIgnoreCase(x.PureKeyName))).ToList();
      var nullifiedKeyInfos = columnNames.Except(usedKeys).Select(x => new KeyInfo(tableName, x) { IsNullified = true }).ToList();
      if (!filterStyle) {
        keyInfos = keyInfos.Where(x => !x.AddKeyName.EqualsIgnoreCase(DH.CreateEditTimeAppendixName) &&
        !x.AddKeyName.EqualsIgnoreCase(DH.CreateEditPictureLinkAppendixName)).ToList();
        foreach (var keyInfo in keyInfos) {
          DataColumn column = columns.FirstOrDefault(x => x.ColumnName.EqualsIgnoreCase(keyInfo.PureKeyName));
          keyInfo.DataType = column.DataType.ToString().Substring(DH.SharedPrefixDataType.Length);
          keyInfo.UpdateTimeStampAndAutoGenerated(meta, column.ColumnName, actionType);
        }
        foreach (var nullifiedKeyInfo in nullifiedKeyInfos) {
          DataColumn column = columns.FirstOrDefault(x => x.ColumnName.EqualsIgnoreCase(nullifiedKeyInfo.PureKeyName));
          nullifiedKeyInfo.DataType = column.DataType.ToString().Substring(DH.SharedPrefixDataType.Length);
          nullifiedKeyInfo.UpdateTimeStampAndAutoGenerated(meta, column.ColumnName, actionType);
        }
      }
      CompleteKeyInfo completeKeyInfo = new CompleteKeyInfo { ValidKeys = keyInfos, NullifiedKeys = nullifiedKeyInfos };
      return completeKeyInfo;
    }

    private CompleteKeyInfo getCompleteKeyInfo(string tableName, FormCollection collections,
      IEnumerable<string> filteredKeys,
      bool filterStyle = false) {
      return getCompleteKeyInfo(tableName, collections, filteredKeys, SQLServerHandler.GetColumns(DH.DataDBConnectionString, tableName), filterStyle);
    }

    private void addFiltersOnScript(StringBuilder queryScript, MetaInfo meta,
      SqlConnection conn, FormCollection collections, List<SqlParameter> pars, List<SqlParameter> copiesPars,
      DateTime refDtNow) {
      //Filters
      var filteredKeys = collections.AllKeys.Except(DH.ExemptedFilterFormCollection);
      int filterNo = 0;
      var completeKeyInfo = getCompleteKeyInfo(meta.TableName, collections, filteredKeys, filterStyle: true);

      foreach (var validKeyInfo in completeKeyInfo.ValidKeys) {
        object value = validKeyInfo.ExtractValueAsObject(collections, refDtNow: refDtNow, filterStyle: true);
        if (value != null) {
          object valueObject = validKeyInfo.CreateQueryValueAsObject(value);
          string querySubstring = validKeyInfo.CreateQuerySubstring(filterNo);
          addQueryParameter(queryScript, querySubstring,
            valueObject, pars, copiesPars, validKeyInfo.Key, value.ToString(), ref filterNo);
        }
      }

      foreach (var nullifiedKeyInfo in completeKeyInfo.NullifiedKeys) {
        //TODO as of now, no need to to anything, but might probably be needed for data type like boolean
      }
      //Fill filter message and filter no
      fillFilterMessageAndNo(filterNo);
    }

    private void addQueryParameter(StringBuilder queryScript, string querySubstring, object val,
      List<SqlParameter> pars, List<SqlParameter> copiesPars, string key, string valueStr, ref int filterNo) {
      queryScript.Append(filterNo == 0 ? " WHERE " : " AND ");
      queryScript.Append(querySubstring);
      string parName = string.Concat("@par" + filterNo);
      SqlParameter par = new SqlParameter(parName, val);
      SqlParameter copyPar = new SqlParameter(parName, val);
      pars.Add(par);
      copiesPars.Add(copyPar);
      TempData.Add(key, valueStr);
      filterNo++;
    }

    private void fillFilterMessageAndNo(int filterNo) {
      if (filterNo > 0) {
        string msg = string.Empty;
        int count = 0;
        foreach (var filterMsg in TempData) {
          if (count > 0)
            msg += Environment.NewLine;
          msg += filterMsg.Key + ": " + filterMsg.Value;
          count++;
        }
        ViewBag.FilterMsg = msg;
      }
      ViewBag.FilterNo = filterNo;
    }

    private FilterIndexInfo finalizeAndExecuteScript(string selectScript, StringBuilder queryScript, SqlConnection conn,
      List<SqlParameter> pars, List<SqlParameter> copiesPars, MetaInfo meta, int usedPage,
      List<UserRelatedFilterInfo> userRelatedInfos) {

      bool whereExisted = queryScript.ToString().ToLower().Contains(" where ");
      StringBuilder userRelatedQueryScript = new StringBuilder();

      if (!UserHelper.UserHasMainAdminRight(User) && //User is not main admin (main admin is free from user related filters)
        User.Identity.IsAuthenticated && //User is authenticated
        userRelatedInfos != null && userRelatedInfos.Count > 0) { //there is a user related info
        //Get user info here!
        List<DataColumn> userColumns = new List<DataColumn>();
        string userQueryScript = "SELECT TOP 1 * FROM [" + DH.UserTableName + "] WHERE [" + DH.UserNameColumnName + "] = " + 
          StringHelper.ProcessAsSqlStringValue(User.Identity.Name);
        DataTable userDataTable = SQLServerHandler.GetDataTable(DH.UserDBConnectionString, userQueryScript);

        foreach (DataColumn column in userDataTable.Columns)
          userColumns.Add(column);

        DataRow userRow = userDataTable.Rows[0];
        int userRelatedInfoCount = 0;
        for (int i = 0; i < userRelatedInfos.Count; ++i) {
          if (userRelatedInfoCount > 0)
            userRelatedQueryScript.Append(" AND ");
          UserRelatedFilterInfo userRelatedInfo = userRelatedInfos[i];
          object userVal = userRow[userRelatedInfo.UserInfoColumnName];
          if (userRelatedInfo.HasUserInfoColumnFreeCandidate)
            if (userRelatedInfo.UserInfoColumnFreeCandidates.Contains(userVal.ToString())) //this user does not have the filter applied
              continue;
          userRelatedQueryScript.Append("([");
          userRelatedQueryScript.Append(userRelatedInfo.ThisColumnName);
          userRelatedQueryScript.Append("]=");
          userRelatedQueryScript.Append("'"); //as of now, assuming string
          userRelatedQueryScript.Append(userVal);
          userRelatedQueryScript.Append("'"); //as of now, assuming string
          if (userRelatedInfo.HasColumnFreeCandidate) {
            foreach (var cand in userRelatedInfo.ThisColumnFreeCandidates) {
              userRelatedQueryScript.Append(" OR [");
              userRelatedQueryScript.Append(userRelatedInfo.ThisColumnName);
              userRelatedQueryScript.Append("]=");
              userRelatedQueryScript.Append("'"); //as of now, assuming string
              userRelatedQueryScript.Append(cand);
              userRelatedQueryScript.Append("'"); //as of now, assuming string
            }
            //Means "Any" user will do
          }
          userRelatedQueryScript.Append(")");
          userRelatedInfoCount++;
        }
      }

      if (!string.IsNullOrWhiteSpace(userRelatedQueryScript.ToString())) {
        queryScript.Append(whereExisted ? " AND " : " WHERE ");
        queryScript.Append(userRelatedQueryScript.ToString());
      }

      //Counting filtered/non-filtered results
      string countScript = string.Concat(selectScript, " count(*) ", queryScript);
      DataTable countDataTable = SQLServerHandler.GetDataTable(conn, countScript);
      int queryCount = countDataTable == null || countDataTable.Rows == null || countDataTable.Rows.Count <= 0 ? 
        0 : (int)countDataTable.Rows[0].ItemArray.FirstOrDefault();

      //Order by      
      bool hasOrderBy = false;
      bool hasOrderByCid = false;
      if (meta.OrderBys != null && meta.OrderBys.Count > 0) {
        int orderCount = 0;
        foreach (var orderBy in meta.OrderBys) {
          queryScript.Append(string.Concat(orderCount == 0 ? " ORDER BY " : ", ", "[", orderBy.Name, "] ", 
            orderBy.GetOrderDirection())); //only the first one requires [order by] phrase
          hasOrderBy = true;
          hasOrderByCid = orderBy.Name.EqualsIgnoreCaseTrim("cid");
          orderCount++;
        }
      }

      if (!hasOrderBy) //there must be at least one
        queryScript.Append(" ORDER BY [Cid] ASC ");
      else if (!hasOrderByCid) { //If it has order by but not having Cid, add the order by [Cid] at the very last
        queryScript.Append(", [Cid] ASC ");
      }

      //Paging
      //Preparing page navigation model
      NavDataModel navData = new NavDataModel(usedPage, meta.ItemsPerPage, queryCount);
      navData.ParentUri = Request.Url.AbsoluteUri.Split('?')[0]; //take everything before the first '?'

      //int firstItemNo = (page.Value - 1) * itemsPerPage + 1;
      int skippedItemNo = (navData.CurrentPage - 1) * meta.ItemsPerPage; //current page in the Nav Data is controlled within the possible range
      if (skippedItemNo == 0) {
        selectScript = string.Concat(selectScript, " TOP " + meta.ItemsPerPage);
      } else {
        queryScript.Append(string.Concat(" OFFSET (", skippedItemNo, ") ROWS FETCH NEXT (", meta.ItemsPerPage, ") ROWS ONLY"));
      }

      //Executing query script
      string completeScript = string.Concat(selectScript, " * ", queryScript.ToString()); //form the complete script here
      DataTable dataTable = SQLServerHandler.GetDataTable(conn, completeScript, pars); //new DataTable();

      FilterIndexInfo filterIndexInfo = new FilterIndexInfo(meta, User, dataTable, navData);
      return filterIndexInfo;
    }

    private void checkModelValidity(string tableName,
      //CreateEditInfo ceInfo,
      List<DataColumn> columns,
      FormCollection collections,
      MetaInfo meta,
      List<string> checkExclusions, 
      DateTime refDtNow,
      string actionType,
      bool filterStyle = false) {
      List<string> modelStateKeys = ModelState.Keys.ToList();

      ////Regex columns
      //Dictionary<string, string> regexes = meta.RegexCheckedColumns?.GetXMLTaggedDictionary("reg");
      ////Regex examples column
      //Dictionary<string, string> regexExamples = meta.RegexCheckedColumnExamples?.GetXMLTaggedDictionary("ex");

      //Prepare columnNames for limitation later...
      //format example: ColumnName1=min:23.555,max:75.112;ColumnName2=max:36.991
      //Dictionary<string, string> limits = new Dictionary<string, string>();
      //if (!string.IsNullOrWhiteSpace(meta.NumberLimits)) {
      //  var numberLimitStrings = meta.NumberLimits.Split(';');
      //  foreach (var numberLimitString in numberLimitStrings) {
      //    var parts = numberLimitString?.Split('=')?.Select(x => x.Trim())?.ToList();
      //    if (parts != null && parts.Count > 1)
      //      limits.Add(parts[0], parts[1]);
      //  }
      //}

      foreach (var key in modelStateKeys) {
        if (checkExclusions.Contains(key))
          continue; //no need to check if explicitly excluded

        bool isExplicitlyRequired = false;
        if (!filterStyle) { //Only on creation and edit
          //List<string> required = meta.RequiredColumns?.Split(';')?.ToList();
          isExplicitlyRequired = meta.RequiredColumns != null && 
            meta.RequiredColumns.Any(x => !string.IsNullOrWhiteSpace(x) && x.EqualsIgnoreCase(key));
        }

        //Now, get the value
        string val = collections[key];
        KeyInfo keyInfo = new KeyInfo(tableName, key);
        DataColumn column = columns.Where(x => x.ColumnName.EqualsIgnoreCase(keyInfo.PureKeyName)).FirstOrDefault();

        //If not filter style, then update the data type from the column given
        //filter style would already have correct data type
        if (!filterStyle)
          keyInfo.DataType = column.DataType.ToString().Substring(DH.SharedPrefixDataType.Length);

        //If column names is not found, assumes it is an unknown injection
        if (column == null) {
          ModelState.Add("UnknownColumn", new ModelState());
          ModelState.AddModelError("UnknownColumn", string.Concat(
            "The column name [", keyInfo.PureKeyName.ToCamelBrokenString(), "] is unknown"));
          continue;
        }

        //If things are not required, and not excepted and is null, no need for further check
        if (string.IsNullOrWhiteSpace(val)) {
          //if (isRequired || !column.AllowDBNull) //column.AllowDBNull, again, does NOT do what it ought to!
          if (!filterStyle) { //only non-filtered style has concept of "required"
            bool isNullable = columnIsNullableByClass(meta.TableName, column.ColumnName);
            bool isImplicitlyRequired = columnIsRequiredByAttribute(meta.TableName, column.ColumnName);
            if (isExplicitlyRequired || isImplicitlyRequired || !isNullable) //if things are required but null, there must be something wrong, required cannot be null
              ModelState.AddModelError(key, string.Concat(
                "field [", keyInfo.PureKeyName.ToCamelBrokenString(), "] is required"));
          }
          continue; //if things are not required or is filterStyle, immediately continue
        }

        //These must be checked AFTER the check for required/not-required, because if they are required, they cannot be skipped
        //Check if the items are implicitly excluded, filter or not, these keys don't matter to be further checked
        if (key.EndsWith(DH.CreateEditTimeAppendixName) ||
            DH.FilterTimeAppendixNames.Any(x => key.EndsWith(x)) ||
            DH.FilterDateAppendixNames.Any(x => key.EndsWith(x)) ||
            key.EndsWith(DH.CreateEditPictureLinkAppendixName)) //picture name is also unchecked from here onwards
          continue; //some null/non-null items like datetime need not to be further checked

        //From this point onwards, the item is not null or empty and is ont excluded explicitly or implicitly (that is, not datetime)
        //At this point, filter style need not further check
        if (filterStyle)
          continue; //TODO this was previously put as return. Check if it is error or if it is intended

        keyInfo.UpdateTimeStampAndAutoGenerated(meta, keyInfo.PureKeyName, actionType);
        //From this point onwards, data is already to be checked
        object value = keyInfo.ExtractValueAsObject(collections, refDtNow, filterStyle);
        if (value == null) //there must be something wrong with the format, otherwise the extracting cannot be null
          ModelState.AddModelError(keyInfo.Key, string.Concat(
            "The input for field [", keyInfo.PureKeyName.ToCamelBrokenString(),
            "] is in a non-acceptable data format. The data type is [",
            keyInfo.DataType, "] but the value given is [", collections[keyInfo.Key],
            "]. Please correct your input data format"));

        //Now, check if string length is violated against the length specified
        if (keyInfo.DataType.EqualsIgnoreCase(DH.StringDataType) || keyInfo.DataType.EqualsIgnoreCase(DH.CharDataType)) {
          //Painfully gets length by reflection, column.MaxLength does NOT show desired string length's limit!!
          int length = getStringLengthFor(meta.TableName, keyInfo.PureKeyName);
          string strVal = value.ToString();
          string camelBrokenPureKeyName = keyInfo.PureKeyName.ToCamelBrokenString();
          if (strVal.Length > length) {
            ModelState.AddModelError(keyInfo.Key, string.Concat(
              "The input string [", value,
              "] for [", camelBrokenPureKeyName,
              "] is too long. The maximum length for [", camelBrokenPureKeyName,
              "] is ", length, " character(s)"
              ));
          }

          RegexCheckedColumnInfo regexInfo = meta.GetRegexCheckedColumn(keyInfo.PureKeyName);
          if (regexInfo != null) { //regex checked items
            Regex regex = new Regex(regexInfo.Content);
            Match match = regex.Match(value.ToString());
            if (!match.Success) {
              StringBuilder regexError = new StringBuilder(string.Concat(
                "The input string [", value,
                "] for [", camelBrokenPureKeyName, "] ",
                "does not match with the required pattern"));

              if (UserHelper.UserIsDeveloper(User)) {
                regexError.Append(" [");
                regexError.Append(regex.ToString());
                regexError.Append("]");
              }

              RegexCheckedColumnExampleInfo exampleInfo = meta.GetRegexCheckedColumnExample(keyInfo.PureKeyName);
              if (exampleInfo != null) {
                regexError.Append(". Example(s) of correct pattern: ");
                regexError.Append(exampleInfo.Content);
              }

              ModelState.AddModelError(keyInfo.Key, regexError.ToString());
            }
          }
        }

        //lastly, check number limits if data type is number types
        //ColumnName1=min:23.555|max:75.112;ColumnName2=max:36.991
        NumberLimitColumnInfo numberLimitInfo = meta.GetNumberLimitColumn(keyInfo.PureKeyName);
        if (DH.NumberDataTypes.Contains(keyInfo.DataType) && //if it is indeed number data types
          numberLimitInfo != null) { //And the limit keys contain pure key name
          double columnValue;
          bool result = double.TryParse(value.ToString(), out columnValue);
          if (result) {
            if (numberLimitInfo.Min > columnValue) //min is violated
              ModelState.AddModelError(keyInfo.Key, string.Concat(
                "The input value [", columnValue, "] for field [",
                keyInfo.PureKeyName.ToCamelBrokenString(),
                "] is smaller than the minimum limit [", numberLimitInfo.Min, "]"
              ));
            if (numberLimitInfo.Max < columnValue) { //max is violated
              ModelState.AddModelError(keyInfo.Key, string.Concat(
                "The input value [", columnValue, "] for field [",
                keyInfo.PureKeyName.ToCamelBrokenString(),
                "] is greater than the maximum limit [", numberLimitInfo.Max, "]"
              ));
            }
          }
          //var limitValueStrings = limits[keyInfo.PureKeyName].Split('|').Select(x => x.Trim()).ToList();
          //foreach (var limitValueString in limitValueStrings) { //each limit value is like min:23.555 or max:75.112
          //  var limitValues = limitValueString.Split(':').Select(x => x.Trim()).ToList();
          //  if (limitValues == null || limitValues.Count < 2)
          //    continue;
          //  decimal limitValue, columnValue;
          //  bool result = decimal.TryParse(limitValues[1], out limitValue);
          //  bool result2 = decimal.TryParse(value.ToString(), out columnValue);
          //  if (!result || !result2) //if any result cannot be parsed by decimal, forget it
          //    continue;

          //  if (limitValues[0] == "max" && columnValue > limitValue) //maximum limitation violated
          //}
        }
      }
    }

    private Type getTableType(string tableName) {
      //  Assembly executingAssembly = Assembly.GetExecutingAssembly();
      //  string type = string.Concat(executingAssembly.GetName().ToString().Split(',')[0], //to get only the assembly name
      //    ".Models.DB.", tableName);
      //return Type.GetType(type);
      return Type.GetType(string.Concat("Aibe.Models.DB.", tableName)); //it is always in the Aibe.Models
    }

    private PropertyInfo getColumnPropertyInfo(string tableName, string columnName) {
      Type tempClass = getTableType(tableName);
      object tableObject = Activator.CreateInstance(tempClass);
      return tempClass.GetProperty(columnName);
    }

    private bool columnIsRequiredByAttribute(string tableName, string columnName) {
      PropertyInfo propertyInfo = getColumnPropertyInfo(tableName, columnName);
      CustomAttributeData cad = propertyInfo.CustomAttributes
        .FirstOrDefault(x => x.AttributeType.ToString().EndsWith("RequiredAttribute"));
      return cad != null;
    }

    private bool columnIsNullableByClass(string tableName, string columnName) {
      PropertyInfo propertyInfo = getColumnPropertyInfo(tableName, columnName);
      string propertyType = propertyInfo.PropertyType.ToString();
      return DH.NullableIndicators.Any(x => propertyType.StartsWith(DH.SharedPrefixDataType + x));
    }

    private int getStringLengthFor(string tableName, string columnName) {
      PropertyInfo propertyInfo = getColumnPropertyInfo(tableName, columnName);
      CustomAttributeData cad = propertyInfo.CustomAttributes
        .FirstOrDefault(x => x.AttributeType.ToString().EndsWith("StringLengthAttribute"));
      return (int)cad.ConstructorArguments[0].Value; //this should be Int32 type actually
    }

    private void addInsertParameter(StringBuilder openingScript, StringBuilder insertParNamesScript,
      KeyInfo keyInfo, List<SqlParameter> pars, FormCollection collections, DateTime refDtNow, ref int parNo, MetaInfo meta) {
      if (parNo > 0) {
        openingScript.Append(",");
        insertParNamesScript.Append(",");
      };

      List<KeyValuePair<string, string>> tableColumnNamePairs = KeyInfo.GetTableColumnPairs(meta, keyInfo.PureKeyName);
      var value = keyInfo.ExtractValueAsObject(collections, refDtNow, filterStyle: false, tableColumnNamePairs: tableColumnNamePairs);
      openingScript.Append(string.Concat("[", keyInfo.Key, "]"));
      string parName = string.Concat("@par", parNo);
      insertParNamesScript.Append(parName);
      SqlParameter par = new SqlParameter(parName, value);
      pars.Add(par);
      parNo++;
    }

    private void addNullUpdateParameter(StringBuilder updateScript, KeyInfo nullifiedKeyInfo, ref int parNo) {
      if (parNo > 0)
        updateScript.Append(",");
      updateScript.Append(string.Concat("[", nullifiedKeyInfo.PureKeyName, "] = NULL"));
    }

    private void addUpdateParameter(StringBuilder updateScript, KeyInfo validKeyInfo,
      List<SqlParameter> pars, FormCollection collections, DateTime refDtNow, ref int parNo) {
      if (parNo > 0)
        updateScript.Append(",");
      object value = validKeyInfo.ExtractValueAsObject(collections, refDtNow, filterStyle: false);
      string parName = string.Concat("@par", parNo);
      updateScript.Append(string.Concat("[", validKeyInfo.PureKeyName, "] = ", parName));
      SqlParameter par = new SqlParameter(parName, value);
      pars.Add(par);
      parNo++;
    }

    private void fillDetailsFromCollectionsToTempData(FormCollection collections, List<string> exclusionList) {
      TempData.Clear();
      foreach (var key in collections.AllKeys)
        if (exclusionList == null || !exclusionList.Contains(key))
          TempData.Add(key, collections[key]);
    }

    private void fillDetailsFromTableToTempData(string tableName, int cid) {
      TempData.Clear();

      //Script making
      StringBuilder queryScript = new StringBuilder(string.Concat("SELECT * FROM [", tableName, "] WHERE Cid = ", cid));
      DataTable dataTable = SQLServerHandler.GetDataTable(DH.DataDBConnectionString, queryScript.ToString()); //new DataTable();

      if (dataTable != null && dataTable.Rows.Count > 0) //only if there is some result
        foreach (DataColumn column in dataTable.Columns)
          TempData.Add(column.ColumnName, dataTable.Rows[0][column]);
    }

    private void deleteItem(string tableName, int cid) {
      StringBuilder queryScript = new StringBuilder(string.Concat("DELETE FROM [", tableName, "] WHERE Cid = ", cid));
      SQLServerHandler.ExecuteScript(queryScript.ToString(), DH.DataDBConnectionString);
    }

    #endregion
  }
}

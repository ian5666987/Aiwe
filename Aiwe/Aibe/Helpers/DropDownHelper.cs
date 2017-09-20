using System;
using System.Collections.Generic;
using System.Linq;
using Aibe.Models;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using Extension.Database;
using Extension.String;

namespace Aibe.Helpers {
  public partial class DropDownHelper {
    static List<string> removedValuedChangedStrings = new List<string> { "@", "[", "]" };
    public static Regex VarToValRegex = new Regex(@"(@|\[@)\w+(\s|$|\)|\])", RegexOptions.Multiline);
    public static string GetCheckedMatch(object match) {
      string checkedMatch = match.ToString().Trim().ToLower();
      foreach (var str in removedValuedChangedStrings)
        checkedMatch = checkedMatch.Replace(str, string.Empty);
      return checkedMatch;
    }

    private static List<string> subcommonGetDropDownFor(
      DropDownInfo dropdown,
      string originalValue, string dataType = "number",
      bool filterApplied = false, 
      Dictionary<string, DropdownPassedArguments> passedColumnsAndValues = null) {
      List<string> dropdownItems = new List<string>();

      //Enumerate here
      foreach (var dropdownItem in dropdown.Items) {
        if (dropdownItem.IsItem) { //If it is item, just put it immediately
          if (!string.IsNullOrWhiteSpace(dropdownItem.Item))
            dropdownItems.Add(dropdownItem.Item);
          continue;
        }

        if (dropdownItem.RefInfo == null) //if it is not item, and not having table value reference, just skip it
          continue;

        //The dropdown parts will be only in terms of a single column name in the table that is referred to already
        TableValueRefInfo rInfo = dropdownItem.RefInfo;
        DropdownPassedArguments passedValue = null;
        if (filterApplied && passedColumnsAndValues != null &&
          !string.IsNullOrWhiteSpace(rInfo.CrossTableCondColumn) &&
          !string.IsNullOrWhiteSpace(rInfo.CondColumn) &&
          passedColumnsAndValues.ContainsKey(rInfo.CrossTableCondColumn)) { //if the passed values contain the tableColumnName, then gets it
          passedValue = passedColumnsAndValues[rInfo.CrossTableCondColumn];
        }

        //Enumerate from the given table and column
        try {
          //Script initialization
          StringBuilder selectScript = new StringBuilder(string.Concat(
            "SELECT DISTINCT [", rInfo.Column, "] FROM [", rInfo.RefTableName, "] WHERE [",
            rInfo.Column, "] IS NOT NULL"));

          List<SqlParameter> pars = new List<SqlParameter>();
          if (passedValue != null &&
            !string.IsNullOrWhiteSpace(passedValue.ToString())) {//other reference has to be made
            SqlParameter par2 = new SqlParameter("@par2", passedValue.DataType.EqualsIgnoreCase(DH.StringDataType) &&
              passedValue.Value != null ? passedValue.Value.ToString() : passedValue.Value);
            pars.Add(par2);
            selectScript.Append(string.Concat(" AND [", rInfo.CondColumn, "] = @par2"));
          }

          if (!string.IsNullOrWhiteSpace(rInfo.AdditionalWhereClause)) {
            var matches = VarToValRegex.Matches(rInfo.AdditionalWhereClause);
            bool isApplied = true; //assume applied till proven otherwise
            string appliedScript = rInfo.AdditionalWhereClause; //to change whenever necessary
            if (passedColumnsAndValues != null) { //only if there is something passed in the first place
              foreach (var match in matches) {
                string checkedMatch = GetCheckedMatch(match);
                DropdownPassedArguments passedItem = passedColumnsAndValues
                  .Where(x => x.Key.EqualsIgnoreCase(checkedMatch))
                  .Select(x => x.Value)
                  .FirstOrDefault();
                if (passedItem == null || string.IsNullOrWhiteSpace(passedItem.Value.ToString())) { //failed case
                  isApplied = false;
                  break;
                }

                string checkString = passedItem.Value.ToString();
                if (SQLServerHandler.ContainsDangerousElement(checkString, passedItem.DataType)) {
                  isApplied = false;
                  LogHelper.Error("Auto", "SQL Injection", "Auto", "Auto", "Auto", "Dropdown Update", rInfo.AdditionalWhereClause,
                    string.Concat("Column: ", match.ToString(), Environment.NewLine, "DataType: ", passedItem.DataType, Environment.NewLine, "Value: ", checkString));
                  break; //WARNING! SQL injection detected!
                }

                //then change the applied item
                appliedScript = appliedScript.Replace(match.ToString(), passedItem.GetFilterStringValue()); //this may be bad since the data type is unkown
              }

              if (isApplied) {
                selectScript.Append(" AND (");
                selectScript.Append(appliedScript);
                selectScript.Append(")");
              }
            } else {
              if (matches.Count <= 0) { //Only apply the script if there is no parameters
                selectScript.Append(" AND (");
                selectScript.Append(appliedScript);
                selectScript.Append(")");
              }
            }
          }

          DataTable dataTable = SQLServerHandler.GetDataTable(DH.DataDBConnectionString, selectScript.ToString(), pars);
          if (dataTable == null)
            return null;

          foreach (DataRow row in dataTable.Rows)
            dropdownItems.Add(row.ItemArray[0].ToString());
        } catch { //TODO put something if necessary, just leave it like this for now
          return null;
        }
      }

      if (dropdownItems != null && !string.IsNullOrWhiteSpace(originalValue) && !dropdownItems.Contains(originalValue))
        dropdownItems.Insert(0, originalValue);

      if (dropdownItems == null || dropdownItems.Count <= 0)
        return null;

      if (!string.IsNullOrWhiteSpace(dropdown.OrderByDirective)) { //If there is order-by directive
        if (dropdown.OrderByDirective.EqualsIgnoreCase("DESC"))
          if (dataType != null && dataType.EqualsIgnoreCase("number")) {
            dropdownItems = dropdownItems.OrderByDescending(x => double.Parse(x)).ToList();
          } else
            dropdownItems = dropdownItems.OrderByDescending(x => x).ToList();
        else if (dropdown.OrderByDirective.EqualsIgnoreCase("ASC"))
          if (dataType != null && dataType.EqualsIgnoreCase("number")) {
            dropdownItems = dropdownItems.OrderBy(x => double.Parse(x)).ToList();
          } else
            dropdownItems = dropdownItems.OrderBy(x => x).ToList();
      }

      return dropdownItems;
    }

    public static List<string> CreateLiveCreateEditDropDownFor(string tableName, string tableColumn, object originalValue,
      string dataType = "number", Dictionary<string, DropdownPassedArguments> passedColumnsAndValues = null) {
      //MetaItem meta = TableHelper.GetMeta(tableName);
      MetaInfo meta = TableHelper.GetMeta(tableName);

      if (meta == null || meta.CreateEditDropDowns == null || !meta.CreateEditDropDowns.Any())
        return null; //fails to enumerate, please handle without dropdown

      //This is to get "Info1" string
      DropDownInfo dropDownInfo = meta.GetCreateEditDropDownColumnInfo(tableColumn);
      if (dropDownInfo == null || dropDownInfo.Items == null || !dropDownInfo.Items.Any())
        return null;

      //This is to process 1,2,3,[RInfo1],[RInfo2],... to distinguish between "Item" and "TableValued"
      if (dropDownInfo.Items.Any(x => !x.IsItem)) //table-valued
        return subcommonGetDropDownFor(dropDownInfo, originalValue?.ToString(), dataType, filterApplied: true, passedColumnsAndValues: passedColumnsAndValues);
      return subcommonGetDropDownFor(dropDownInfo, originalValue?.ToString(), dataType);
    }

    public static List<string> GetStaticCreateEditDropDownFor(string tableName, string tableColumn, string originalValue, string dataType = "number") {
      MetaInfo meta = TableHelper.GetMeta(tableName);

      if (meta == null || meta.CreateEditDropDowns == null || !meta.CreateEditDropDowns.Any())
        return null; //fails to enumerate, please handle without dropdown

      DropDownInfo dropDownInfo = meta.GetCreateEditDropDownColumnInfo(tableColumn);
      if (dropDownInfo == null || dropDownInfo.Items == null || !dropDownInfo.Items.Any())
        return null;

      return subcommonGetDropDownFor(dropDownInfo, originalValue, dataType, filterApplied: false, passedColumnsAndValues: null);
    }

    public static List<string> GetStaticFilterDropDownFor(string tableName, string tableColumn, string dataType = "number") {
      MetaInfo meta = TableHelper.GetMeta(tableName);

      if (meta == null || meta.FilterDropDowns == null || !meta.FilterDropDowns.Any())
        return null; //fails to enumerate, please handle without dropdown

      DropDownInfo dropDownInfo = meta.GetFilterDropDownColumnInfo(tableColumn);
      if (dropDownInfo == null || dropDownInfo.Items == null || !dropDownInfo.Items.Any())
        return null;

      return subcommonGetDropDownFor(dropDownInfo, null, dataType, filterApplied: false, passedColumnsAndValues: null);
    }
  }

  public class DropdownPassedArguments {
    public object OriginalValue { get; set; }
    public object Value { get; set; }
    public string DataType { get; set; }

    public string GetFilterStringOriginalValue() {
      if (DataType.EqualsIgnoreCase(DH.StringDataType) || DataType.EqualsIgnoreCase(DH.DateTimeDataType))
        return string.Concat("'", OriginalValue.ToString().Replace("'", "''"), "'");
      return OriginalValue.ToString();
    }

    public string GetFilterStringValue() {
      if (DataType.EqualsIgnoreCase(DH.StringDataType) || DataType.EqualsIgnoreCase(DH.DateTimeDataType))
        return string.Concat("'", Value.ToString().Replace("'", "''"), "'");
      return Value.ToString();
    }
  }
}

//public static List<string> CreateLiveCreateEditDropDownFor(string tableName, string tableColumn, object originalValue,
//  string dataType = "number", Dictionary<string, DropdownPassedArguments> passedColumnsAndValues = null) {
//  //MetaItem meta = TableHelper.GetMeta(tableName);
//  MetaInfo meta = TableHelper.GetMeta(tableName);

//  if (meta == null || meta.CreateEditDropDowns == null || !meta.CreateEditDropDowns.Any())
//    return null; //fails to enumerate, please handle without dropdown

//  ////For different dropdown columns: Info1;Info2;...;InfoN
//  ////Thus, symbol ";" cannot be in the where clause
//  //List<string> dropdownLists = meta.CreateEditDropDownLists.Split(';')?.Select(x => x.Trim()).ToList();
//  //if (dropdownLists == null || dropdownLists.Count <= 0)
//  //  return null; //fails to enumerate, please handle without dropdown

//  ////Each Info should be like Info1=1,2,3,[RInfo1],[RInfo2],...
//  ////This is to get "Info1" string
//  //string dropdownString = dropdownLists //actually checking table column
//  //  .FirstOrDefault(x => x.Split('=')[0].Trim() == tableColumn);
//  DropDownInfo dropDownInfo = meta.GetCreateEditDropDownColumnInfo(tableColumn);
//  if (dropDownInfo == null || dropDownInfo.Items == null || !dropDownInfo.Items.Any())
//    return null;

//  //if (string.IsNullOrWhiteSpace(dropdownString))
//  //  return null; //fails to enumerate, please handle without dropdown

//  ////Since it uses index of, does not matter if the item contains "="
//  ////But the item cannot contains ","
//  ////This is to process 1,2,3,[RInfo1],[RInfo2],...
//  //var dropdownParts = dropdownString.Substring(dropdownString.IndexOf('=') + 1)
//  //  .Split(',').Select(x => x.Trim()).ToList();

//  //if (dropdownParts.Count < 1)
//  //  return null; //fails to enumerate, please handle without dropdown

//  //List<string> filteredDropdownParts = dropdownParts.Where(x => x.StartsWith("[") && x.EndsWith("]") && x.Contains('=') && x.Split(':').Length >= 3).ToList();
//  if (dropDownInfo.Items.Any(x => !x.IsItem)) //table-valued
//    return subcommonGetDropDownFor(dropDownInfo, originalValue?.ToString(), dataType, filterApplied: true, passedColumnsAndValues: passedColumnsAndValues);
//  return subcommonGetDropDownFor(dropDownInfo, originalValue?.ToString(), dataType);
//}

//public static List<string> GetStaticCreateEditDropDownFor(string tableName, string tableColumn, string originalValue, string dataType = "number") {
//  MetaInfo meta = TableHelper.GetMeta(tableName);

//  if (meta == null || meta.CreateEditDropDowns == null || !meta.CreateEditDropDowns.Any())
//    return null; //fails to enumerate, please handle without dropdown

//  DropDownInfo dropDownInfo = meta.GetCreateEditDropDownColumnInfo(tableColumn);
//  if (dropDownInfo == null || dropDownInfo.Items == null || !dropDownInfo.Items.Any())
//    return null;

//  //List<string> dropdownLists = meta.CreateEditDropDownLists.Split(';')?.Select(x => x.Trim()).ToList();
//  //if (dropdownLists == null || dropdownLists.Count <= 0)
//  //  return null; //fails to enumerate, please handle without dropdown

//  return subcommonGetDropDownFor(dropDownInfo, originalValue, dataType, filterApplied: false, passedColumnsAndValues: null);
//  //return commonStaticFilteredGetDropdownFor(meta.CreateEditDropDowns, tableColumn, originalValue, dataType);
//}

//public static List<string> GetStaticFilterDropDownFor(string tableName, string tableColumn, string dataType = "number") {
//  MetaInfo meta = TableHelper.GetMeta(tableName);

//  if (meta == null || meta.FilterDropDowns == null || !meta.FilterDropDowns.Any())
//    return null; //fails to enumerate, please handle without dropdown

//  DropDownInfo dropDownInfo = meta.GetFilterDropDownColumnInfo(tableColumn);
//  if (dropDownInfo == null || dropDownInfo.Items == null || !dropDownInfo.Items.Any())
//    return null;

//  //List<string> dropdownLists = meta.FilterDropDownLists.Split(';')?.Select(x => x.Trim()).ToList();
//  //if (dropdownLists == null || dropdownLists.Count <= 0)
//  //  return null; //fails to enumerate, please handle without dropdown

//  return subcommonGetDropDownFor(dropDownInfo, null, dataType, filterApplied: false, passedColumnsAndValues: null);
//  //return commonStaticFilteredGetDropdownFor(meta.FilterDropDowns, tableColumn, null, dataType);
//}

//List<string> dropdownParts, 
//List<DropDownInfo> dropdowns,
//List<DropDownItemInfo> dropdownParts,
//string refTableOrderStyle = string.Empty;

//private static List<string> commonStaticFilteredGetDropdownFor(
//  List<DropDownInfo> dropdowns, string tableColumn, string originalValue, string dataType = "number") {
//  //[0] NullableString=[TestTableCommonA:OfficerName:ThisTableColumn=OtherConn.OtherTable.OtherColumn],{DESC}
//  //[1] NullableInt=[TestTableCommonB:JobSite],{ASC};
//  if (dropdowns == null || !dropdowns.Any())
//    return null; //fails to enumerate, please handle without dropdown
//  //List<DropDownInfo> filteredDropDowns = dropdowns.Where(x => x.Name.EqualsIgnoreCase(tableColumn)).ToList(); //get only dropdowns which has the column names
//  //                                                                                                            //.FirstOrDefault(x => x.Split('=')[0].Trim() == tableColumn);
//  //if (dropdowns == null || !dropdowns.Any())
//  //  return null; //fails to enumerate, please handle without dropdown

//  ////Only the first equal part, so it is OK
//  ////NullableString=[TestTableCommonA:OfficerName:OtherColumn=ThisTableColumn],{DESC}
//  //var dropdownParts = dropdownString.Substring(dropdownString.IndexOf('=') + 1)
//  //  .Split(',').Select(x => x.Trim()).ToList();

//  ////24,[...],35,{DESC}
//  //if (dropdownParts.Count < 1)
//  //  return null; //fails to enumerate, please handle without dropdown

//  return subcommonGetDropdownFor(filteredDropDowns, originalValue, dataType, filterApplied: false, passedColumnsAndValues: null);
//}
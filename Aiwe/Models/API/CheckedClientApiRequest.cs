using Extension.Database.SqlServer;
using Extension.String;
using Aibe.Helpers;
using Aibe.Models;
using Aiwe.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Aiwe.Models.API {
  public class CheckedClientApiRequest {
    public string UserName { get; private set; }
    public string Password { get; private set; }
    public ApiRequestType RequestType { get; private set; }
    public string TableName { get; private set; }
    public string TableSource { get; private set; }
    public string ColumnNamesDesc { get; private set; }
    private List<string> validColumnNames = new List<string>();
    public string FilterDesc { get; private set; }
    public string OrderByDesc { get; private set; }
    public int? ItemsLoaded { get; private set; }
    public int? ItemsSkipped { get; private set; }
    public int? Id { get; private set; }
    public string Value { get; private set; }
    public ClientApiRequestAttachment[] Attachments { get; private set; }
    public bool IsSuccess { get; private set; }
    public string ErrorMessage { get; private set; }  
    public MetaInfo Meta { get; private set; }

    //Additional properties to help
    private List<DataColumn> columns = new List<DataColumn>();
    private List<string> columnNames = new List<string>();

    public static Dictionary<string, ApiRequestType> RequestDict = new Dictionary<string, ApiRequestType> {
      { Aiwe.DH.GetManyRequest, ApiRequestType.SelectMany },
      { Aiwe.DH.GetRequest, ApiRequestType.Select },
      { Aiwe.DH.PostRequest, ApiRequestType.Create },
      { Aiwe.DH.PutRequest, ApiRequestType.Update },
      { Aiwe.DH.DeleteRequest, ApiRequestType.Delete },
    };

    //TODO to be checked especially the using or ToLower() and Substring(1, l-2). They are supposed to be replaced with correct StringExtension counterparts
    public CheckedClientApiRequest(ClientApiRequest clientRequest) { 
      UserName = clientRequest.UserName;
      Password = clientRequest.Password;

      if (RequestDict.ContainsKey(clientRequest.RequestType))
        RequestType = RequestDict[clientRequest.RequestType];

      TableName = clientRequest.TableName; //table name must exists since it will already be filtered before this is called
      Meta = AiweTableHelper.GetMeta(TableName);
      TableSource = Meta.TableSource;

      ColumnNamesDesc = clientRequest.ColumnNames;
      columns = SQLServerHandler.GetColumns(Aibe.DH.DataDBConnectionString, TableSource);
      columnNames = columns.Select(x => x.ColumnName).ToList();

      if (!string.IsNullOrWhiteSpace(ColumnNamesDesc)) {
        List<string> usedColumnNames = ColumnNamesDesc.Split(',').Select(x => x.ToLower().Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        validColumnNames = columnNames.Where(x => usedColumnNames.Contains(x.ToLower().Trim())).ToList();
      }

      Attachments = clientRequest.Attachments;

      string errorMsg;
      bool result;
      if (!string.IsNullOrWhiteSpace(clientRequest.FilterDesc)) {        
        result = checkFilterDesc(clientRequest.FilterDesc, out errorMsg);
        if (result)
          FilterDesc = clientRequest.FilterDesc;
        else { //should fail here!
          ErrorMessage = errorMsg;
          return;
        }
      }

      if (!string.IsNullOrWhiteSpace(clientRequest.OrderByDesc)) {
        result = checkOrderByDesc(clientRequest.OrderByDesc, out errorMsg);
        if (result)
          OrderByDesc = clientRequest.OrderByDesc;
        else {
          ErrorMessage = errorMsg;
          return;
        }
      }

      ItemsLoaded = clientRequest.ItemsLoaded;
      ItemsSkipped = clientRequest.ItemsSkipped;
      Id = clientRequest.Id;
      Value = clientRequest.Value; //TODO as of now, make it simple first...
      IsSuccess = true;
    }

    public string CreateLogValue(int lengthLimit = 3000) {
      StringBuilder sb = new StringBuilder();
      if (!string.IsNullOrWhiteSpace(ColumnNamesDesc)) {
        sb.Append("ColumnNames: ");
        sb.Append(ColumnNamesDesc);
        sb.Append(System.Environment.NewLine);
      }
      if (Id.HasValue) {
        sb.Append("Id: ");
        sb.Append(Id.Value);
        sb.Append(System.Environment.NewLine);
      }
      if (!string.IsNullOrWhiteSpace(FilterDesc)) {
        sb.Append("FilterDesc: ");
        sb.Append(FilterDesc);
        sb.Append(System.Environment.NewLine);
      }
      if (!string.IsNullOrWhiteSpace(OrderByDesc)) {
        sb.Append("OrderByDesc: ");
        sb.Append(OrderByDesc);
        sb.Append(System.Environment.NewLine);
      }
      if (ItemsLoaded.HasValue) {
        sb.Append("ItemsLoaded: ");
        sb.Append(ItemsLoaded.Value);
        sb.Append(System.Environment.NewLine);
      }
      if (ItemsSkipped.HasValue) {
        sb.Append("ItemsSkipped: ");
        sb.Append(ItemsSkipped.Value);
        sb.Append(System.Environment.NewLine);
      }
      if (!string.IsNullOrWhiteSpace(Value)) {
        sb.Append("Values: ");
        sb.Append(Value);
      }
      string testStr = sb.ToString();
      if (testStr != null && testStr.Length > lengthLimit) //for now just hard code this as 3000
        testStr = testStr.Substring(0, lengthLimit);
      return testStr;
    }


    //Where clause allowed? yes!
    //Unchecked? yes! because it is very hard to check the where clause
    //[Column Name] ASC, NextColumnName DESC, NextColumnName
    private List<string> componentSplitter(string desc) {
      List<string> components = new List<string>();
      StringBuilder sb = new StringBuilder();
      bool openSquareFound = false;
      int openBracketFound = 0; //can do this, except that this is only applied for Where clause //this has depth
      bool openAposthropeFound = false;
      string str = string.Empty;
      char[] chArr = desc.ToArray();
      char nextChar;

      for (int i = 0; i < chArr.Length; ++i) {
        char ch = chArr[i];

        if (openAposthropeFound) { //open aposthrope means must be completed till the end, surpasing bracket and square bracket
          if (ch == '\'') { //two possibilities, closing apostrophe or double apostrhope
            if (i == chArr.Length - 1) { //the closing apostrophe for sure
              sb.Append(ch);
              components.Add(sb.ToString());
              sb = new StringBuilder();
              openAposthropeFound = false;
            } else {
              nextChar = chArr[i + 1];
              if (nextChar == '\'') { //double aposthrope
                sb.Append("''"); //put double aposthropes to the current component
                i++; //skip the next check
              } else { //closing aposthrope
                sb.Append(ch);
                components.Add(sb.ToString());
                sb = new StringBuilder();
                openAposthropeFound = false;
              }
            }
          } else
            sb.Append(ch);
          continue;
        }

        if (openBracketFound > 0) { //passing the openSquare!
          sb.Append(ch);
          if (ch == '(')
            openBracketFound++; //increasing the depth          
          if (ch == ')') {
            if (openBracketFound == 1) {
              components.Add(sb.ToString());
              sb = new StringBuilder();              
            }
            openBracketFound--; //the depth is minus by 1
          }
          continue;
        }

        if (openSquareFound) { //in this condition insert all character till it is no long inside square bracket
          sb.Append(ch);
          if (ch == ']') {
            components.Add(sb.ToString());
            sb = new StringBuilder();
            openSquareFound = false;
          }
          continue;
        }

        //from this point onwards it is not on openSquareFound
        switch (ch) {
          case ' ': //space means whatever before is taken
            str = sb.ToString();
            if (!string.IsNullOrWhiteSpace(str))
              components.Add(sb.ToString());
            sb = new StringBuilder();
            break;
          case ';': //WARNING! SQL INJECTION!!
            return null; //stops immediately! 
          case '>':
          case '<':
          case '!': //it is ok to inject now to be rejected latter
            if (i == chArr.Length - 1) { //last char
              str = sb.ToString();
              if (!string.IsNullOrWhiteSpace(str))
                components.Add(sb.ToString());
              sb = new StringBuilder();
              components.Add(ch.ToString());
              break;
            }
            nextChar = chArr[i + 1]; //not last char, must be followed by "="
            str = sb.ToString();
            if (!string.IsNullOrWhiteSpace(str))
              components.Add(sb.ToString());
            sb = new StringBuilder();
            if (nextChar == '=') { //if followed by the subsquent '=' then just pick both
              components.Add(ch.ToString() + nextChar.ToString()); //add the current and the next character
              i++; //skip the next check
            } else { //next char is not equal to?
              components.Add(ch.ToString());
            }
            break;
          case '-': //this may mean simple minus, but maybe injection! must be checked! double -- is not allowed in the base level, only allowed if enclosed by aposthropes
            str = sb.ToString();
            if (!string.IsNullOrWhiteSpace(str))
              components.Add(sb.ToString());
            sb = new StringBuilder();
            if (i == chArr.Length - 1) { //last char
              components.Add(ch.ToString());
              break;
            }
            nextChar = chArr[i + 1]; //not last char, must NOT be followed by "-"
            if (nextChar == '-') //WARNING! SQL INJECTION!!
              return null; //stops immediately!
            else
              components.Add(ch.ToString());
            break;
          case '+':
          case '*': //this may mean two things, but both are ok
          case '/':
          case '=':
          case ',': //single character is allowed
            str = sb.ToString();
            if (!string.IsNullOrWhiteSpace(str))
              components.Add(sb.ToString());
            sb = new StringBuilder();
            components.Add(ch.ToString());
            break;
          case '\'': //open aposthrope only
            openAposthropeFound = true;
            sb.Append(ch);
            break;
          case '[': //open square bracket
            openSquareFound = true;
            sb.Append(ch);
            break;
          case '(': //open bracket
            openBracketFound++;
            sb.Append(ch);
            break;
          //close square/- bracket, not possible to be found if there is no open square bracket earlier
          default: //any other cases, just add it
            sb.Append(ch);
            break;
        }
      }
      str = sb.ToString();
      if (!string.IsNullOrWhiteSpace(str))
        components.Add(str);
      return components;
    }

    static List<string> allowedOrderByNonColumnNameComponents = new List<string> {
      ",", "(", ")", //base things are allowed
      "+", "-", "*", "/", "%", //basic maths are allowed
      Aibe.DH.AscOrderWord, Aibe.DH.DescOrderWord
    };

    private bool checkOrderByDesc(string desc, out string errorMsg) {
      errorMsg = string.Empty;
      try {
        List<string> components = componentSplitter(desc);
        if (components == null)
          return false;
        foreach (var component in components) {
          string componentContent = component.GetNonEmptyTrimmedInBetween("[", "]");
          if (componentContent == null) { //not something in between []
            if (!columnNames.Any(x => x.EqualsIgnoreCase(component))) //cannot find it in the columnNames, not a column
              if (!allowedOrderByNonColumnNameComponents.Any(x => x.EqualsIgnoreCase(component))) { //cannot find it in the allowed non column name components, not order directive either
                errorMsg = string.Concat(Aibe.LCZ.W_Desc, ": ", desc, "\n", Aibe.LCZ.W_Action, ": ",
                  Aiwe.LCZ.W_OrderCheck, "\n", Aibe.LCZ.W_Error, ": ", Aibe.LCZ.NFE_InvalidKeywordOrComponent, "\n", Aibe.LCZ.W_Item, ": ", component);
                return false;
              }
          } else { //not something in between []
            if (!columnNames.Any(x => x.EqualsIgnoreCase(componentContent))) { //does not correspond to any valid column name
              errorMsg = string.Concat(Aibe.LCZ.W_Desc, ": ", desc, "\n", Aibe.LCZ.W_Action, ": ",
                Aiwe.LCZ.W_OrderCheck, "\n", Aibe.LCZ.W_Error, ": ", Aibe.LCZ.NFE_InvalidColumnName, "\n", Aibe.LCZ.W_Item, ": ", component);
              return false;
            }
          }
        }
        return true;
      } catch {
        return false;
      }
    }

    private bool checkNameValidity(string name) {
      return !name.Any(ch => !char.IsLetterOrDigit(ch) && !char.IsWhiteSpace(ch) && ch != '_'); //not letter or decimal digit, not white space nor underscore, false!
    }

    private bool checkComparisonContainsTautologyInjection(string prev, string next) {
      List<char> acceptableChars = new List<char> { };
      string str = string.Empty;
      str = new string(str.Where(x => acceptableChars.Contains(x)).ToArray());
      //TODO this is very hard to make after all!
      //4. [CN1]/[CN2] = [CN1]*[CN1]/([CN1]*[CN2])
      //Unless it is simplified, we cannot check the expression final result as the same or not, we cannot do this
      return false;
    }

    static List<string> comparatorComponents = new List<string> { "=", ">", "<", ">=", "<=", "!=", "<>" };
    static List<string> allowedFilterNonTableNameNonColumnNameComponents = new List<string> {
      ",", "and", "or", //base things are allowed
      "=", ">", "<", ">=" , "<=", "!=", "<>", //comparators are allowed
      "+", "-", "*", "/", "%", //basic maths are allowed
      "is", "not", "like", "select", "from", "where", "order", "by", "asc", "desc", //some keywords are allowed
      "null", //null is allowed
    };

    //numbers, tableNames, or columnNames are allowed. Other than that, rejects!
    //tableNames or columnNames must be checked differently. Check if it is in the correct format
    private bool checkFilterDesc(string desc, out string errorMsg) {
      errorMsg = string.Empty;
      try {
        List<string> components = componentSplitter(desc);
        if (components == null)
          return false; //immediately fails if there is null in any level
        int index = 0;
        foreach (var component in components) {
          if (component.StartsWith("(") && component.EndsWith(")") && component.Length > 2) { //bracket case
            if (!checkFilterDesc(component.Substring(1, component.Length - 2).Trim(), out errorMsg)) //checks one level deeper
              return false;
          } else if (component.StartsWith("'") && component.EndsWith("'") && component.Length > 2) { //case in between the aposthrope, leaves it
            //Currently, do nothing, since it is considered valid if having ''
          } else if (component.StartsWith("[") && component.EndsWith("]") && component.Length > 2) { //components with tableName or columnName
            if (!checkNameValidity(component.Substring(1, component.Length - 2).ToLower().Trim())) { //check if the name is invalid
              errorMsg = string.Concat(Aibe.LCZ.W_Desc, ": ", desc, "\n", Aibe.LCZ.W_Action, ": ",
                Aiwe.LCZ.W_FilterCheck, "\n", Aibe.LCZ.W_Error, ": ", Aibe.LCZ.NFE_InvalidName, "\n", Aibe.LCZ.W_Item, ": ", component);
              return false; //returns false
            }
          } else if (comparatorComponents.Contains(component) && index < components.Count - 1 && index > 0 && components.Count > 2){ //check for tautology, not the beginning or the end
            string prevComponent = components[index - 1];
            string nextComponent = components[index + 1];
            if (checkComparisonContainsTautologyInjection(prevComponent, nextComponent)) {
              errorMsg = string.Concat(Aibe.LCZ.W_Desc, ": ", desc, "\n", Aibe.LCZ.W_Action, ": ",
                Aiwe.LCZ.W_FilterCheck, "\n", Aibe.LCZ.W_Error, ": ", Aibe.LCZ.NFE_BadComparison, "\n", Aibe.LCZ.W_Item, ": ",
                prevComponent, component, nextComponent);
              return false; //must not be allowed items, otherwise harmful!
            }
          } else { //all other classes
            var parts = component.Split(',');
            foreach (var part in parts) { //it must be numbers 
              decimal dummyDec;
              bool result = decimal.TryParse(part, out dummyDec);
              if (result) //if it can be parsed by decimal, it is not harmful at all
                continue;
              //If it is not number, then it must only be table names or columns (checked in the square bracket above)               
              if (!allowedFilterNonTableNameNonColumnNameComponents.Any(x => x == part.ToLower())) { //all others must be registered
                errorMsg = string.Concat(Aibe.LCZ.W_Desc, ": ", desc, "\n", Aibe.LCZ.W_Action, ": ",
                  Aiwe.LCZ.W_FilterCheck, "\n", Aibe.LCZ.W_Error, ": ", Aibe.LCZ.NFE_InvalidKeywordOrComponent, "\n", Aibe.LCZ.W_Item, ": ", component);
                return false; //must be not-allowed items, otherwise harmful!
              }
            }
          }
          ++index;
        }

      } catch (Exception ex) {
        errorMsg = ex.ToString();
        return false;
      }
      return true;
    }

    public string CreateDeleteString() {
      if (string.IsNullOrWhiteSpace(TableSource) || RequestType != ApiRequestType.Delete || Id == null)
        return null;

      StringBuilder sb = new StringBuilder("DELETE FROM ");
      sb.Append(string.Concat("[", TableSource, "] WHERE [", Aibe.DH.Cid , "] = ", Id.Value));

      return sb.ToString();
    }

    //TODO this is actually pretty bad for the TimeStamp and for the AutoGenerated 
    //Leave it for now
    private List<SqlParameter> createParsFromValues(MetaInfo meta, bool isInsert, out List<string> usedColumnNames) {
      usedColumnNames = new List<string>();
      try {
        List<SqlParameter> pars = new List<SqlParameter>();
        List<string> items = Value == null ? new List<string>() : Value.ParseComponents(','); //Value.Split(',').Select(x => x.Trim()).ToList(); //this is a wrong way to do it since SQL string may contain comma!
        int index = 0; //take from the item one by one
        List<string> columnNamesExceptCid = columnNames.Where(x => !x.EqualsIgnoreCase(Aibe.DH.Cid)).ToList(); //take the columnName except Cid
        if (items.Count > columnNamesExceptCid.Count) //not possible to do this be it insert or update
          return null;
        DateTime refDtNow = DateTime.Now;
        //Actually, if the string is used to create new item and some columns consists of autogenerated item, then this item should not take
        //The value, but using autogenerated. But then, in this case, it must send dummy item.
        foreach (var item in items) {
          SqlParameter par = new SqlParameter();
          string columnName = columnNamesExceptCid[index]; //the initial value is fit for insert, not for update
          string valueStr = item; //take the item in the split as the value until proven otherwise
          if (!isInsert) { //if it is update style
            int equalIndex = item.IndexOf('=');
            if (equalIndex < 0 || equalIndex >= item.Length - 1) //if equal index is not found or is the last one (or more)
              return null; //fails
            //string[] parts = item.Split('=');
            //if (parts == null || parts.Length < 2)
            //  return null; //must be wrong things
            columnName = item.Substring(0, equalIndex).Trim();
            valueStr = item.Substring(equalIndex + 1).Trim();
          }
          par.ParameterName = "@par" + index;
          usedColumnNames.Add(columnName);

          DataColumn column = columns.FirstOrDefault(x => x.ColumnName.EqualsIgnoreCase(columnName));
          if (column == null || columnName.EqualsIgnoreCase(Aibe.DH.Cid)) //if the column information is not around or it is Cid
            return null; //then fails it

          //get the Value
          string dataType = column.DataType.ToString().Substring(Aibe.DH.SharedPrefixDataType.Length);

          //additional info to extract auto-generated object
          string actionName = isInsert ? Aibe.DH.CreateActionName : Aibe.DH.EditActionName;
          bool isTimeStamp = meta.IsTimeStampAppliedFor(columnName, actionName);
          bool isTimeStampFixed = meta.IsTimeStampFixedFor(columnName, actionName);
          int timeStampShift = meta.GetTimeStampShiftFor(columnName, actionName);
          bool isAutoGenerated = meta.IsAutoGenerated(columnName);

          List<KeyValuePair<string, string>> tableColumnNamePairs = KeyInfo.GetTableColumnPairs(meta, columnName);
          if (!string.IsNullOrWhiteSpace(valueStr) && valueStr.EqualsIgnoreCaseTrim(Aibe.DH.NULL))
            par.Value = DBNull.Value;
          else {
            object value = null;
            value = KeyInfoHelper.ExtractValueAsObjectForWebApi(dataType, valueStr, refDtNow, 
              TableSource, columnName, isTimeStamp, isTimeStampFixed, timeStampShift, isAutoGenerated, tableColumnNamePairs);
            if (value == null) //fails to get the object as it should, return null as a whole
              return null;
            par.Value = value; //get the object, assign the object
          }
          
          pars.Add(par);
          ++index;
        }

        //The prefilled columns are used here, only for insert!
        if (isInsert && meta.TableName != meta.TableSource && meta.PrefilledColumns != null) {
          foreach(var pc in meta.PrefilledColumns) {
            SqlParameter par = new SqlParameter();
            par.ParameterName = "@par" + index;
            par.Value = pc.RightSide;
            usedColumnNames.Add(pc.Name);
            ++index;
          }
        }

        return pars;
      } catch {
        return null;
      }
    }

    public string CreateUpdateString(out List<SqlParameter> pars) {
      pars = null;
      if (string.IsNullOrWhiteSpace(TableSource) || RequestType != ApiRequestType.Update || Id == null)
        return null;

      StringBuilder sb = new StringBuilder("UPDATE ");
      sb.Append(string.Concat("[", TableSource, "] SET "));

      List<string> usedColumnNames = new List<string>();
      pars = createParsFromValues(Meta, false, out usedColumnNames);
      if (pars == null)
        return null;

      int index = 0;
      foreach (var par in pars) {
        if (index > 0)
          sb.Append(", ");
        string columnName = usedColumnNames[index];
        sb.Append(string.Concat("[", columnName, "] = @par", index));
        ++index;
      }

      sb.Append(string.Concat(" WHERE [", Aibe.DH.Cid, "] = ", Id.Value));

      return sb.ToString();
    }

    //Actually, this is for POST
    //Note the for TimeStamp and for Automatically generated Id, there must be a way to produce that
    public string CreateInsertIntoString(out List<SqlParameter> pars) { //Yet the method's name is "insert into" following SQL command convention --bad naming, bad connection
      pars = null;
      if (string.IsNullOrWhiteSpace(TableSource) || RequestType != ApiRequestType.Create) //and the request type is CREATE somemore
        return null;

      StringBuilder sb = new StringBuilder("INSERT INTO ");
      StringBuilder backSb = new StringBuilder(" VALUES (");
      sb.Append(string.Concat("[", TableSource, "] "));

      List<string> usedColumnNames = new List<string>();
      pars = createParsFromValues(Meta, true, out usedColumnNames);
      if (pars == null)
        return null;
      bool hasColumn = usedColumnNames != null && usedColumnNames.Any();

      int index = 0;
      foreach (var par in pars) {
        if (index > 0) {
          if (hasColumn)
            sb.Append(", ");
          backSb.Append(", ");
        }
        if (hasColumn) {
          string columnName = usedColumnNames[index];
          if (index == 0)
            sb.Append("(");
          sb.Append(string.Concat("[", columnName, "]"));
        }
        backSb.Append(string.Concat("@par", index));
        ++index;
      }

      if (hasColumn)
        sb.Append(")");
      backSb.Append(")");

      return string.Concat(sb.ToString(), backSb.ToString());
    }

    public string CreateQueryStringSingle() {
      if (string.IsNullOrWhiteSpace(TableSource) || RequestType != ApiRequestType.Select || Id == null)
        return null;

      StringBuilder sb = new StringBuilder("SELECT ");
      if (string.IsNullOrWhiteSpace(ColumnNamesDesc)) {
        sb.Append("*");
      } else {
        if (validColumnNames == null || validColumnNames.Count <= 0)
          return null;
        int index = 0;
        foreach (var validColumnName in validColumnNames) {
          if (index > 0)
            sb.Append(",");
          sb.Append(string.Concat("[", validColumnName, "]"));
          index++;
        }
      }
      sb.Append(" FROM ");
      sb.Append(string.Concat("[", TableSource, "]"));
      sb.Append(" WHERE [" + Aibe.DH.Cid + "] = ");
      sb.Append(Id.Value);

      return sb.ToString();
    }

    public string CreateQueryString() {
      if (string.IsNullOrWhiteSpace(TableSource) || RequestType != ApiRequestType.SelectMany)
        return null;

      StringBuilder sb = new StringBuilder("SELECT ");

      if (ItemsLoaded != null && ItemsLoaded > 0)
        if (ItemsSkipped == null || ItemsSkipped == 0) {
          sb.Append(string.Concat(" TOP ", ItemsLoaded, " "));
        } else {
          sb.Append(string.Concat(" OFFSET (", ItemsSkipped, ") ROWS FETCH NEXT (", ItemsLoaded, ") ROWS ONLY "));
        }

      if (string.IsNullOrWhiteSpace(ColumnNamesDesc)) {
        sb.Append("*");
      } else {
        if (validColumnNames == null || validColumnNames.Count <= 0)
          return null;
        int index = 0;
        foreach (var validColumnName in validColumnNames) {
          if (index > 0)
            sb.Append(",");
          sb.Append(string.Concat("[", validColumnName, "]"));
          index++;
        }
      }

      sb.Append(" FROM ");
      sb.Append(string.Concat("[", TableSource, "]"));

      if (!string.IsNullOrWhiteSpace(FilterDesc))
        sb.Append(string.Concat(" WHERE ", FilterDesc));

      if (!string.IsNullOrWhiteSpace(OrderByDesc))
        sb.Append(string.Concat(" ORDER BY ", OrderByDesc));
      
      return sb.ToString();
    }
  }

  public enum ApiRequestType {
    SelectMany,
    Select,
    Create,
    Update,
    Delete,
    //Insert, //Currently unused
  }
}

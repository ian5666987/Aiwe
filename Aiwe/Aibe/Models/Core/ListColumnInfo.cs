using System.Collections.Generic;
using System.Linq;
using Extension.String;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Extension.Database.SqlServer;

namespace Aibe.Models.Core {
  //ColumnName1|HeaderName11,HeaderName12,…,HeaderName1N=ListType|RefTableName:RefColumnName:RefAnotherColumnName=ThisOtherColumnName:AddWhereAndClause;
  public partial class ListColumnInfo : CommonBaseInfo {
    private static List<string> defaultListTypes = new List<string> { "default", "check", "list", "remarks", "dropdown", "table" }; //TODO table type is yet to be made
    private static List<string> defaultHeaderNames = new List<string> { "Name", "Value", "Ending" };
    public new string Name { get; private set; } //this is hiding the original name, but make use of it
    public List<string> HeaderNames { get; private set; }
    public string ListType { get; private set; } = "default"; 
    public TableValueRefInfo RefInfo { get; private set; }
    public ListColumnInfo(string desc) : base(desc) {
      if (!IsValid)
        return;

      List<string> baseNameParts = base.Name.GetTrimmedNonEmptyParts('|');
      if(baseNameParts.Count <= 0) { //it cannot be without name
        IsValid = false;
        return;
      }
      Name = baseNameParts[0]; //The name exists
      if (baseNameParts.Count >= 2) //have header names
        HeaderNames = baseNameParts[1].GetTrimmedNonEmptyParts(',');              

      if (!HasRightSide) //it is ok not to have the right side for ListColumnInfo
        return;

      //Actually, where clause also cannot have "|"
      var rightParts = RightSide.GetTrimmedNonEmptyParts('|');

      if (rightParts.Count < 1)
        return;

      if (defaultListTypes.Any(x => x.EqualsIgnoreCaseTrim(rightParts[0]))) //If it is a valid type
        ListType = defaultListTypes.FirstOrDefault(x => x.EqualsIgnoreCaseTrim(rightParts[0])); //apply the new type

      if (rightParts.Count < 2)
        return;

      RefInfo = new TableValueRefInfo(rightParts[1]);
      if (!RefInfo.IsValid) { //if it has reference, it must be valid. Otherwise revoke the validity
        IsValid = false;
        return;
      }
    }

    public bool GetRefDataValue(string changedColumnName, string changedColumnValue, out string dataValue) {
      dataValue = string.Empty;
      if (RefInfo == null ||
        string.IsNullOrWhiteSpace(RefInfo.RefTableName) ||
        string.IsNullOrWhiteSpace(RefInfo.Column) ||
        string.IsNullOrWhiteSpace(RefInfo.CondColumn) ||
        (string.IsNullOrWhiteSpace(RefInfo.StaticCrossTableCondColumn) &&  //if static cross-cond-column is empty
           (string.IsNullOrWhiteSpace(RefInfo.CrossTableCondColumn) || //the dynamic cannot be empty
            string.IsNullOrWhiteSpace(changedColumnValue) || //the changed column value cannot be empty 
            string.IsNullOrWhiteSpace(changedColumnName) || //the changed column name cannot be empty
            RefInfo.CrossTableCondColumn.EqualsIgnoreCase(changedColumnName))))
        return false;

      try {
        //Script making
        StringBuilder selectScript = new StringBuilder(string.Concat(
          "SELECT DISTINCT [", RefInfo.Column, "] FROM [", RefInfo.RefTableName, "] WHERE [",
          RefInfo.Column, "] IS NOT NULL AND [", RefInfo.CondColumn, "] = @par")
         );

        if (!string.IsNullOrWhiteSpace(RefInfo.AdditionalWhereClause)) {
          selectScript.Append(" AND (");
          selectScript.Append(RefInfo.AdditionalWhereClause);
          selectScript.Append(")");
        }

        string appliedValue = string.IsNullOrWhiteSpace(RefInfo.CrossTableCondColumn) &&
          !string.IsNullOrWhiteSpace(RefInfo.StaticCrossTableCondColumn) ?
          RefInfo.StaticCrossTableCondColumn : changedColumnValue;
        SqlParameter par = new SqlParameter("@par", appliedValue);
        DataTable dataTable = SQLServerHandler.GetDataTable(DH.DataDBConnectionString, selectScript.ToString(), par);

        if (dataTable == null || dataTable.Rows == null || dataTable.Rows.Count <= 0 ||
          dataTable.Rows[0].ItemArray == null || dataTable.Rows[0].ItemArray.Length <= 0 ||
          dataTable.Rows[0].ItemArray[0] == null)
          return false;

        dataValue = dataTable.Rows[0].ItemArray[0].ToString();
        return true;
      } catch {
        return false;
      }
    }
  }
}
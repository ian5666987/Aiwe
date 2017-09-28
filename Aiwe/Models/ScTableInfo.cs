using System.Data;
using System.Linq;
using System.Collections.Generic;
using Aibe.Models.Core;
using Extension.String;

namespace Aiwe.Models {
  public class ScTableInfo {
    public ScriptColumnInfo ScInfo { get; private set; }
    public bool IsValid { get; private set; }
    public DataTable DataTable { get; private set; }
    public List<DataColumn> Columns { get; private set; } = new List<DataColumn>();
    public List<DataRow> Rows { get; private set; } = new List<DataRow>();
    public bool HasPassedData { get; private set; }
    public bool HasRow { get { return Rows != null && Rows.Count > 0; } }
    public bool HasCidColumn { get; private set; }
    public ScTableInfo (ScriptColumnInfo scColumn, Dictionary<string, string> stringDictionary) {
      if (scColumn == null || !scColumn.IsValid)
        return;
      if (stringDictionary == null || stringDictionary.Count <= 0) { //used for "create" case
        ScInfo = scColumn;
        IsValid = true;
      }
      //Below is "edit" case, that is, having initial data
      DataTable = scColumn.GetDataTable(stringDictionary);
      if (DataTable == null)
        return;
      HasPassedData = true;
      ScInfo = scColumn;
      IsValid = true;
      foreach (DataColumn column in DataTable.Columns)
        Columns.Add(column);
      HasCidColumn = Columns.Any(x => x.ColumnName.EqualsIgnoreCase("Cid"));
      foreach (DataRow row in DataTable.Rows)
        Rows.Add(row);
    }

    public List<DataColumn> GetAvailableDataColumns() { //use constructor DataColumn for create, otherwise use real scTable.Columns result
      return Columns == null || Columns.Count <= 0 ? ScInfo.Constructor.DataColumns : Columns;
    }
  }
}
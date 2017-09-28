using System.Data;
using System.Collections.Generic;
using Aibe.Models.Core;

namespace Aiwe.Models {
  public class ColumnInfo {
    public string Name { get; set; }
    public string DataType { get; private set; }
    public string DisplayName { get; set; }
    public bool IsIndexIncluded { get; set; }
    public bool IsFilterIncluded { get; set; }
    public bool IsIndexShowImage { get; set; }
    public bool IsListColumn { get; set; }
    public bool IsSciptColumn { get; set; }
    public int ImageWidth { get; set; } = 100; //default value
    public ScTableInfo ScTable { get; set; }
    public List<ColoringInfo> Colorings { get; set; } = new List<ColoringInfo>();

    private DataColumn column;
    public ColumnInfo(DataColumn column) {
      this.column = column;
      DataType = column.DataType.ToString().Substring(Aibe.DH.SharedPrefixDataType.Length);
    }
  }
}
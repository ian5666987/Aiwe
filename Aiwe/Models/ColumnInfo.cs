using System.Data;
using System.Collections.Generic;
using Aibe;
using Aibe.Models;

namespace Aiwe.Models {
  public class ColumnInfo {
    public string Name { get; set; }
    public string DataType { get; private set; }
    public string DisplayName { get; set; }
    public bool IsIndexIncluded { get; set; }
    public bool IsFilterIncluded { get; set; }
    public bool IsIndexShowImage { get; set; }
    public bool IsListColumn { get; set; }
    public int ImageWidth { get; set; } = 100; //default value
    public List<ColoringInfo> Colorings { get; set; } = new List<ColoringInfo>();

    private DataColumn column;
    public ColumnInfo(DataColumn column) {
      this.column = column;
      DataType = column.DataType.ToString().Substring(DH.SharedPrefixDataType.Length);
    }

    //Not need to have this anymore, since in the Colorings, everything needed is there
    //public List<ColoringItem> CreateColorItems(int id) {
    //  if (Colorings == null || Colorings.Count <= 0)
    //    return null;
    //  return Colorings.Select(x => ColoringHelper.CreateColoringItem(column, x, id)).ToList();
    //}
  }
}
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Aibe.Models {
  public partial class ListColumnItem : BaseInfo {
    public string Name { get; set; }
    public string Type { get; set; } = "default";
    public string Value { get; set; }
    public string Ending { get; set; }
    public string Dropdown { get; set; }
    public List<string> DropdownList { get; set; }
    public bool HasDropdownList { get { return DropdownList != null && DropdownList.Count > 0; } }
    public string Remarks { get; set; }
    public List<string> EndingList { get; set; }
    public bool HasEndingList { get { return EndingList != null && EndingList.Count > 0; } }
    public string CurrentDesc { get {
        StringBuilder sb = new StringBuilder(Name);

        sb.Append("=");
        if(!string.IsNullOrWhiteSpace(Value))
          sb.Append(Value);

        if (Type == "check")
          return sb.ToString();
        
        if (Type == "remarks") {
          sb.Append("|" + (string.IsNullOrWhiteSpace(Remarks) ? "" : Remarks));
        } else if (Type == "dropdown") {
          if (HasDropdownList) {
            sb.Append("|");
            sb.Append(string.Join(",", DropdownList));
          }
          sb.Append("|" + (string.IsNullOrWhiteSpace(Remarks) ? "" : Remarks));
        } else {
          sb.Append("|" + (string.IsNullOrWhiteSpace(Remarks) ? "" : Ending) + "|");
          if (HasEndingList)
            sb.Append(string.Join(",", EndingList));
        }
        
        return sb.ToString();
      } }

    //Called for creation, such as when add button is used in the javascript
    public ListColumnItem (string desc, string lcType) : base(desc) { //TipLength=10|mm|mm,cm,m  OR  HasTipLength=True OR Ian=17|He is a good officer OR Name=val|ddc1,ddc2|passed!
      if (string.IsNullOrWhiteSpace(desc))
        return;
      IsValid = true; //this point onwards is OK
      Type = lcType;
      int index = desc.IndexOf('=');
      if (index < 0 || desc.Length <= index + 1) {
        Name = desc.Trim(); //TipLength        
        return;
      }
      
      Name = desc.Substring(0, index).Trim(); //TipLength
      string itemDesc = desc.Substring(index + 1).Trim(); //10|mm|mm,cm,m  OR  10||  OR  False OR 17|He is a good officer OR val|ddc1,ddc2|passed!

      var itemDescParts = itemDesc.Split('|').Select(x => x.Trim()).ToList(); //[10  mm  mm,cm,m] OR [17  "He is a good officer"] OR [val ddc1,ddc2 passed!]
      if (itemDescParts == null || itemDescParts.Count <= 0)
        return;
      Value = itemDescParts[0];
      if (itemDescParts.Count <= 1 || Type == "check")
        return;

      if (lcType == "remarks") {
        Remarks = itemDescParts[1];
        return;
      } else if(lcType == "dropdown") {
        Dropdown = itemDescParts[1];
        DropdownList = itemDescParts[1].Split(',').Select(x => x.Trim()).ToList();
        if (itemDescParts.Count <= 2)
          return;
        Remarks = itemDescParts[2];
        return;
      }

      Ending = itemDescParts[1];
      if (itemDescParts.Count <= 2)
        return;
      EndingList = itemDescParts[2].Split(',').Select(x => x.Trim()).ToList();
    }
  }
}
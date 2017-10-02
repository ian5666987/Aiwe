using Aibe.Models;
using Aibe.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Aiwe.Extensions {
  public static class ListColumnInfoExtension {
    private static void insertCommonHTMLAttributes(StringBuilder sb, string columnName, int rowNo, char subItemType, int columnNo, bool isAdd) {
      sb.Append(isAdd ? " class=\"common-subcolumn-input-add\"" : " class=\"common-subcolumn-input\"");
      sb.Append(" commoncolumnname=\"" + columnName + "\"");
      sb.Append(" commonrowno=\"" + rowNo + "\"");
      sb.Append(" commonsubitemtype=\"" + subItemType + "\"");
      sb.Append(" commoncolumnno=\"" + columnNo + "\"");
      sb.Append(" commoninputisadd=\"" + isAdd + "\"");
      sb.Append(" id=\"common-subcolumn-input-" + columnName + "-" + subItemType + "-" + rowNo + "-" + columnNo + "-" + isAdd + "\"");
    }

    //Called to create HTML for the list column
    public static string GetHTML(this ListColumnInfo info, string dataValue, bool isReadOnly = false) {      
      //Initialization
      List<ListColumnItem> listColumnItems = new List<ListColumnItem>();
      int textCol = 20; //TODO currently hardcoded
      string readOnlyBackgroundColor = "ececec";
      if (!string.IsNullOrWhiteSpace(dataValue))
        listColumnItems = dataValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
          ?.Select(x => new ListColumnItem(x.Trim(), info.ListType)).ToList();
      StringBuilder sb = new StringBuilder();
      sb.Append("<table style=\"border-collapse:separate;border-spacing:10px 5px;border:1px solid black\">");

      //Create headers
      List<string> usedHeaders = new List<string>();
      string header = string.Empty;
      info.ResetHeaderCount();
      sb.Append("<tr>");
      foreach (char c in info.ListType) { //for every item known, one header
        sb.Append("<td><b>");
        header = info.GetNextHeader();
        usedHeaders.Add(header);
        sb.Append(header);
        sb.Append("</b></td>");
      }
      if (!isReadOnly)
        sb.Append("<td><b>Actions</b></td>"); //last header would be action, if there is any
      sb.Append("</tr>");

      //Create items
      int count = 1;
      foreach (var item in listColumnItems) {
        sb.Append("<tr>");
        for (int i = 0; i < item.SubItems.Count; ++i) {
          ListColumnSubItem subItem = item.SubItems[i];
          int columnNo = i + 1;
          sb.Append("<td>");
          if (isReadOnly) {
            if (subItem.SubItemType == 'L') { //The simplest of all, just print it
              sb.Append(subItem.Value);
            } else { //the second easiest, read-only
              sb.Append("<input");
              sb.Append(" readonly=\"readonly\"");
              sb.Append(" style=\"background-color:#" + readOnlyBackgroundColor + "\"");
              sb.Append(" type=\"text\" size=\"" + textCol + "\" value=\"");
              sb.Append(subItem.Value);
              sb.Append("\" />");
            }
          } else { //If not read only, do individual printing of HTML
            switch (subItem.SubItemType) {
              case 'L': //the simplest case, just print it
                sb.Append(subItem.Value); 
                break;
              case 'V': //Value type, print it...
                sb.Append("<input");
                insertCommonHTMLAttributes(sb, info.Name, count, subItem.SubItemType, columnNo, false);
                sb.Append(" type=\"text\" size=\"" + textCol + "\" value=\"");
                sb.Append(subItem.Value);
                sb.Append("\" />");
                break;
              case 'O':
                sb.Append("<select");
                insertCommonHTMLAttributes(sb, info.Name, count, subItem.SubItemType, columnNo, false);
                sb.Append(">");
                if (subItem.HasOptions) {
                  if (string.IsNullOrWhiteSpace(subItem.Value)) {
                    sb.Append("<option selected=\"selected\"></option>");
                  } else {
                    sb.Append("<option value=\"\"></option>");
                  }
                  foreach (var option in subItem.Options) {
                    sb.Append("<option value=\"");
                    sb.Append(option);
                    if (!string.IsNullOrWhiteSpace(subItem.Value) && option == subItem.Value)
                      sb.Append("\" selected=\"selected");
                    sb.Append("\">");
                    sb.Append(option);
                    sb.Append("</option>\n");
                  }
                }
                sb.Append("</select>");
                break;
              case 'C':
                sb.Append("<select");
                insertCommonHTMLAttributes(sb, info.Name, count, subItem.SubItemType, columnNo, false);
                sb.Append(">");
                string def = subItem.Value?.ToLower()?.Trim();
                string selectedStr = " selected=\"selected\"";
                string addStr = string.IsNullOrWhiteSpace(def) ? selectedStr : string.Empty;
                sb.Append(string.Concat("<option value=\"\"", addStr, "></option>"));
                addStr = def == "yes" ? selectedStr : string.Empty;
                sb.Append(string.Concat("<option value=\"Yes\"", addStr, ">Yes</option>"));
                addStr = def == "no" ? selectedStr : string.Empty;
                sb.Append(string.Concat("<option value=\"No\"", addStr, ">No</option>"));
                sb.Append("</select>");
                break;
              default: break;
            }
          }
          sb.Append("</td>");
        }

        //Delete button
        if (!isReadOnly) {
          sb.Append("<td>");
          sb.Append("<button");
          sb.Append(" class=\"common-subcolumn-button\"");
          sb.Append(" commonbuttontype=\"delete\"");
          sb.Append(" id=\"common-subcolumn-button-delete-" + info.Name + "-" + count + "\"");
          sb.Append(" commondeleteno=\"" + count + "\"");
          sb.Append(" commoncolumnname=\"" + info.Name + "\"");
          sb.Append(">Delete</button>");
          sb.Append("</td>");
        }

        sb.Append("</tr>");
        count++;
      }

      //last row
      if (!isReadOnly) {
        sb.Append("<tr>");

        //All subcolumns
        for (int i = 0; i < info.ListType.Length; ++i) {
          string usedHeader = usedHeaders[i];
          char subItemType = info.ListType[i];
          int columnNo = i + 1;
          sb.Append("<td>");
          sb.Append("<input");
          insertCommonHTMLAttributes(sb, info.Name, count, subItemType, columnNo, true);
          sb.Append(" type=\"text\" size=\"" + textCol + "\" value=\"\"");
          sb.Append(" placeholder=\"");
          switch (subItemType) {
            case 'L': //L an V share the same placeholder
            case 'V': sb.Append(usedHeader); break;
            case 'O': sb.Append(usedHeader + " Or " + usedHeader + " | Option 1, Option 2, ..., Option N"); break;
            case 'C': sb.Append("Yes or No"); break; //very special for the check
            default: break;
          }
          sb.Append("\""); //end of placeholder
          sb.Append("/>"); //end of input
          sb.Append("</td>");
        }

        //Add button
        sb.Append("<td>");
        sb.Append("<button");
        sb.Append(" class=\"common-subcolumn-button\"");
        sb.Append(" commonbuttontype=\"add\"");
        sb.Append(" id=\"common-subcolumn-button-add-" + info.Name + "\"");
        sb.Append(" commoncolumnname=\"" + info.Name + "\"");
        sb.Append(">Add</button>");
        sb.Append("</td>");

        sb.Append("</tr>");
      }

      //Ending
      sb.Append("</table>");
      return sb.ToString();
    }
    
    public static string GetDetailsHTML(this ListColumnInfo info, string dataValue) {
      //Checking
      var listColumnItems = dataValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
        ?.Select(x => new ListColumnItem(x.Trim(), info.ListType)).ToList();

      if (listColumnItems.Count <= 0)
        return null;

      //Initialization
      StringBuilder sb = new StringBuilder();
      sb.Append("<table style=\"border-collapse:separate;border-spacing:10px 5px;border:1px solid black\">");

      //Create headers
      List<string> usedHeaders = new List<string>();
      string header = string.Empty;
      info.ResetHeaderCount();
      sb.Append("<tr>");
      foreach (char c in info.ListType) { //for every item known, one header
        sb.Append("<td><b>");
        header = info.GetNextHeader();
        usedHeaders.Add(header);
        sb.Append(header);
        sb.Append("</b></td>");
      }
      sb.Append("</tr>");

      //Create items
      foreach (var item in listColumnItems) {
        sb.Append("<tr>");
        for (int i = 0; i < item.SubItems.Count; ++i)
          sb.Append("<td>" + item.SubItems[i].Value + "</td>");
        sb.Append("</tr>");
      }

      //Ending
      sb.Append("</table>");
      return sb.ToString();
    }
  }
}

//TextArea
//if (!info.ListType.EqualsIgnoreCase("list")) //if it is a list, don't need to add empty item here...
//  sb.Append("<td></td>");
//sb.Append("<td>");
//sb.Append("<textarea");
//sb.Append(" rows=\"" + textAreaRow + "\" cols=\"" + textAreaCol + "\"");
//sb.Append(" id=\"common-subcolumn-addtext-" + info.Name + "\"");
//sb.Append(" placeholder=\"");
//sb.Append("\"");

//if (info.ListType.EqualsIgnoreCase("list")) {
//  sb.Append(" placeholder=\"");
//  sb.Append(usedHeaders[0] + " 1; ");
//  sb.Append(usedHeaders[0] + " 2; ...; ");
//  sb.Append(usedHeaders[0] + " N");
//  sb.Append("\"");
//} else if (info.ListType.EqualsIgnoreCase("check")) {
//  sb.Append(string.Concat(" placeholder=\"", usedHeaders[0], " = True Or ", usedHeaders[0], " = False\""));
//} else if (info.ListType.EqualsIgnoreCase("remarks")) {
//  sb.Append(string.Concat(" placeholder=\"", usedHeaders[0], " = ", usedHeaders[1], " | ", usedHeaders[2], "\""));
//} else if (info.ListType.EqualsIgnoreCase("dropdown")) {
//  sb.Append(string.Concat(" placeholder=\"", usedHeaders[0], " = ", usedHeaders[1], " | Choice 1, Choice 2, ..., Choice N | ", usedHeaders[2], "\""));
//} else {
//  sb.Append(string.Concat(" placeholder=\"", usedHeaders[0], " = ", usedHeaders[1], " | ", usedHeaders[2], " | Choice 1, Choice 2, ..., Choice N\""));
//}
//sb.Append("></textarea>");
//sb.Append("</td>");

//        sb.Append("<td>" + (info.ListType.EqualsIgnoreCase("list") ? string.Empty : "<b>")); //if it is a list, then don't need to bold at this point
//        sb.Append(item.Name?.ToCamelBrokenString());
//        sb.Append("</td>" + (info.ListType.EqualsIgnoreCase("list") ? string.Empty : "</b>"));

//        if (item.Type.EqualsIgnoreCase("list")) {
//          //nothing is there
//        } else if (item.Type.EqualsIgnoreCase("check")) {
//          if (isReadOnly) {
//            sb.Append("<td><input");
//            sb.Append(" class=\"" + roStrName + "-input\"");
//            sb.Append(" " + roAttrPrefix + "columnname=\"" + info.Name + "\"");
//            sb.Append(" " + roAttrPrefix + "inputno=\"" + count + "\"");
//            sb.Append(" " + roAttrPrefix + "inputtype=\"select\"");
//            sb.Append(" " + roAttrPrefix + "inputpart=\"check\"");
//            sb.Append(" id=\"" + roStrName + "-input-checklist-" + info.Name + "-" + count + "\"");
//            sb.Append(" readonly=\"readonly\"");
//            sb.Append(" style=\"background-color:#" + readOnlyBackgroundColor + "\"");
//            sb.Append(" type=\"text\" size=\"" + textAreaCol + "\" value=\"");
//            sb.Append(item.Value);
//            sb.Append("\" /></td>");
//          } 
//          else {
//            sb.Append("<td><select");
//            sb.Append(" class=\"" + roStrName + "-input\"");
//            sb.Append(" " + roAttrPrefix + "columnname=\"" + info.Name + "\"");
//            sb.Append(" " + roAttrPrefix + "inputno=\"" + count + "\"");
//            sb.Append(" " + roAttrPrefix + "inputtype=\"select\"");
//            sb.Append(" " + roAttrPrefix + "inputpart=\"check\"");
//            sb.Append(" id=\"" + roStrName + "-input-checklist-" + info.Name + "-" + count + "\"");
//            sb.Append(">");

//            string def = item.Value?.ToLower()?.Trim();
//string selectedStr = " selected=\"selected\"";
//string addStr = string.IsNullOrWhiteSpace(def) ? selectedStr : string.Empty;
//sb.Append(string.Concat("<option value=\"\"", addStr, "></option>"));
//            addStr = def == "true" ? selectedStr : string.Empty;
//            sb.Append(string.Concat("<option value=\"True\"", addStr, ">True</option>"));
//            addStr = def == "false" ? selectedStr : string.Empty;
//            sb.Append(string.Concat("<option value=\"False\"", addStr, ">False</option>"));

//            sb.Append("</select></td>");
//          }
//        } else if (item.Type.EqualsIgnoreCase("remarks")) {

//          sb.Append("<td><input");
//          sb.Append(" class=\"" + roStrName + "-input\"");
//          sb.Append(" " + roAttrPrefix + "columnname=\"" + info.Name + "\"");
//          sb.Append(" " + roAttrPrefix + "inputno=\"" + count + "\"");
//          sb.Append(" " + roAttrPrefix + "inputtype=\"text\"");
//          sb.Append(" " + roAttrPrefix + "inputpart=\"text\"");
//          sb.Append(" id=\"" + roStrName + "-input-text-" + info.Name + "-" + count + "\"");
//          if (isReadOnly) {
//            sb.Append(" readonly=\"readonly\"");
//            sb.Append(" style=\"background-color:#" + readOnlyBackgroundColor + "\"");
//          }
//          sb.Append(" type=\"text\" size=\"" + textAreaCol + "\" value=\"");
//          sb.Append(item.Value);
//          sb.Append("\" /></td>");

//          sb.Append("<td><input");
//          sb.Append(" class=\"" + roStrName + "-input\"");
//          sb.Append(" " + roAttrPrefix + "columnname=\"" + info.Name + "\"");
//          sb.Append(" " + roAttrPrefix + "inputno=\"" + count + "\"");
//          sb.Append(" " + roAttrPrefix + "inputtype=\"text\"");
//          sb.Append(" " + roAttrPrefix + "inputpart=\"remarks\"");
//          sb.Append(" id=\"" + roStrName + "-input-remarks-" + info.Name + "-" + count + "\"");
//          if (isReadOnly) {
//            sb.Append(" readonly=\"readonly\"");
//            sb.Append(" style=\"background-color:#" + readOnlyBackgroundColor + "\"");
//          }
//          sb.Append(" type=\"text\" size=\"" + textAreaCol* 2 + "\" value=\"");
//          sb.Append(item.Remarks);
//          sb.Append("\" /></td>");
//        } 
//        else if (item.Type.EqualsIgnoreCase("dropdown")) {
//          if (isReadOnly) {
//            sb.Append("<td><input");
//            sb.Append(" class=\"" + roStrName + "-input\"");
//            sb.Append(" " + roAttrPrefix + "columnname=\"" + info.Name + "\"");
//            sb.Append(" " + roAttrPrefix + "inputno=\"" + count + "\"");
//            sb.Append(" " + roAttrPrefix + "inputtype=\"select\"");
//            sb.Append(" " + roAttrPrefix + "inputpart=\"dropdown\"");
//            sb.Append(" id=\"" + roStrName + "-input-dropdown-" + info.Name + "-" + count + "\"");
//            sb.Append(" readonly=\"readonly\"");
//            sb.Append(" style=\"background-color:#" + readOnlyBackgroundColor + "\"");
//            sb.Append(" type=\"text\" size=\"" + textAreaCol + "\" value=\"");
//            sb.Append(item.Value);
//            sb.Append("\" /></td>");
//          } else {
//            sb.Append("<td><select");
//            sb.Append(" class=\"" + roStrName + "-input " + roStrName + "-input-unit\"");
//            sb.Append(" " + roAttrPrefix + "columnname=\"" + info.Name + "\"");
//            sb.Append(" " + roAttrPrefix + "inputno=\"" + count + "\"");
//            sb.Append(" " + roAttrPrefix + "inputtype=\"select\"");
//            sb.Append(" " + roAttrPrefix + "inputpart=\"dropdown\"");
//            sb.Append(" id=\"" + roStrName + "-input-dropdown-" + info.Name + "-" + count + "\"");
//            sb.Append(">");
//            if (item.HasDropdownList) {
//              if (string.IsNullOrWhiteSpace(item.Value)) {
//                sb.Append("<option selected=\"selected\"></option>");
//              } else {
//                sb.Append("<option value=\"\"></option>");
//              }
//              foreach (var subitem in item.DropdownList) {
//                sb.Append("<option value=\"");
//                sb.Append(subitem);
//                if (!string.IsNullOrWhiteSpace(item.Value) && subitem == item.Value)
//                  sb.Append("\" selected=\"selected");
//                sb.Append("\">");
//                sb.Append(subitem);
//                sb.Append("</option>\n");
//              }
//            }
//            sb.Append("</select></td>");
//        }

//        sb.Append("<td><input");
//          sb.Append(" class=\"" + roStrName + "-input\"");
//          sb.Append(" " + roAttrPrefix + "columnname=\"" + info.Name + "\"");
//          sb.Append(" " + roAttrPrefix + "inputno=\"" + count + "\"");
//          sb.Append(" " + roAttrPrefix + "inputtype=\"text\"");
//          sb.Append(" " + roAttrPrefix + "inputpart=\"remarks\"");
//          sb.Append(" id=\"" + roStrName + "-input-remarks-" + info.Name + "-" + count + "\"");
//          if (isReadOnly) {
//            sb.Append(" readonly=\"readonly\"");
//            sb.Append(" style=\"background-color:#" + readOnlyBackgroundColor + "\"");
//          }
//          sb.Append(" type=\"text\" size=\"" + textAreaCol* 2 + "\" value=\"");
//          sb.Append(item.Remarks);
//          sb.Append("\" /></td>");          
//        } 
//        else {
//          sb.Append("<td><input");
//          sb.Append(" class=\"" + roStrName + "-input\"");
//          sb.Append(" " + roAttrPrefix + "columnname=\"" + info.Name + "\"");
//          sb.Append(" " + roAttrPrefix + "inputno=\"" + count + "\"");
//          sb.Append(" " + roAttrPrefix + "inputtype=\"text\"");
//          sb.Append(" " + roAttrPrefix + "inputpart=\"text\"");
//          sb.Append(" id=\"" + roStrName + "-input-text-" + info.Name + "-" + count + "\"");
//          if (isReadOnly) {
//            sb.Append(" readonly=\"readonly\"");
//            sb.Append(" style=\"background-color:#" + readOnlyBackgroundColor + "\"");
//          }
//          sb.Append(" type=\"text\" size=\"" + textAreaCol + "\" value=\"");
//          sb.Append(item.Value);
//          sb.Append("\" /></td>");

//          if (isReadOnly) {
//            sb.Append("<td><input");
//            sb.Append(" class=\"" + roStrName + "-input " + roStrName + "-input-unit\"");
//            sb.Append(" " + roAttrPrefix + "columnname=\"" + info.Name + "\"");
//            sb.Append(" " + roAttrPrefix + "inputno=\"" + count + "\"");
//            sb.Append(" " + roAttrPrefix + "inputtype=\"select\"");
//            sb.Append(" " + roAttrPrefix + "inputpart=\"unit\"");
//            sb.Append(" id=\"" + roStrName + "-input-unit-" + info.Name + "-" + count + "\"");
//            sb.Append(" readonly=\"readonly\"");
//            sb.Append(" style=\"background-color:#" + readOnlyBackgroundColor + "\"");
//            sb.Append(" type=\"text\" size=\"" + textAreaCol + "\" value=\"");
//            sb.Append(item.Value);
//            sb.Append("\" /></td>");
//          } else {
//            sb.Append("<td><select");
//            sb.Append(" class=\"" + roStrName + "-input " + roStrName + "-input-unit\"");
//            sb.Append(" " + roAttrPrefix + "columnname=\"" + info.Name + "\"");
//            sb.Append(" " + roAttrPrefix + "inputno=\"" + count + "\"");
//            sb.Append(" " + roAttrPrefix + "inputtype=\"select\"");
//            sb.Append(" " + roAttrPrefix + "inputpart=\"unit\"");
//            sb.Append(" id=\"" + roStrName + "-input-unit-" + info.Name + "-" + count + "\"");
//            sb.Append(">");
//            if (item.HasEndingList) {
//              if (string.IsNullOrWhiteSpace(item.Ending)) {
//                sb.Append("<option selected=\"selected\"></option>");
//              } else {
//                sb.Append("<option value=\"\"></option>");
//              }
//              foreach (var subitem in item.EndingList) {
//                sb.Append("<option value=\"");
//                sb.Append(subitem);
//                if (!string.IsNullOrWhiteSpace(item.Ending) && subitem == item.Ending)
//                  sb.Append("\" selected=\"selected");
//                sb.Append("\">");
//                sb.Append(subitem);
//                sb.Append("</option>\n");
//              }
//            }

//            sb.Append("</select></td>");
//          }
//        }


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
      sb.Append(isAdd ? " class=\"common-subcolumn-input-add-" + columnName +"\"" : " class=\"common-subcolumn-input\"");
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
      string readOnlyBackgroundColor = "ececec";
      if (!string.IsNullOrWhiteSpace(dataValue)) {
        var descs = dataValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
        foreach(var subdesc in descs) {
          ListColumnItem lcItem = new ListColumnItem(subdesc.Trim(), info.ListType, info.Widths);
          listColumnItems.Add(lcItem);
        }
        //listColumnItems = descs?.Select(x => new ListColumnItem(x.Trim(), info.ListType, info.Widths)).ToList();
      }
      StringBuilder sb = new StringBuilder();
      sb.Append("<table style=\"border-collapse:separate;border-spacing:10px 5px;border:1px solid black\">");

      //Create headers
      List<string> usedHeaders = new List<string>();
      string header = string.Empty;
      int width = ListColumnInfo.DefaultWidth;
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
              sb.Append(" type=\"text\" size=\"" + subItem.Width + "\" value=\"");
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
                sb.Append(" type=\"text\" size=\"" + subItem.Width + "\" value=\"");
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
          sb.Append(" type=\"text\" size=\"" + info.Widths[i] + "\" value=\"\"");
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
        ?.Select(x => new ListColumnItem(x.Trim(), info.ListType, info.Widths)).ToList();

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

using System;
using System.Collections.Generic;
using System.Linq;
using Extension.String;
using System.Text;
using System.Data;
using Aibe.Models.Core;

namespace Aiwe.Extensions {
  public static class ListColumnInfoExtension {
    //Called to create HTML for the list column
    public static string GetHTML(this ListColumnInfo info, string dataValue) {      
      List<ListColumnItem> listColumnItems = new List<ListColumnItem>();
      List<string> usedHeaders = new List<string>();
      string header = string.Empty;
      int textAreaRow = 3;
      int textAreaCol = 30;
      if (!string.IsNullOrWhiteSpace(dataValue))
        listColumnItems = dataValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
          ?.Select(x => new ListColumnItem(x.Trim(), info.ListType)).ToList();

      int usedHeaderIndex = 0;
      StringBuilder sb = new StringBuilder();
      sb.Append("<table style=\"border-collapse:separate;border-spacing:10px 5px;border:1px solid black\">");
      sb.Append("<tr>");

      sb.Append("<td><b>");
      header = info.HeaderNames != null && info.HeaderNames.Count > usedHeaderIndex ?
        info.HeaderNames[usedHeaderIndex++] : "Sub-Item";
      usedHeaders.Add(header);
      sb.Append(header);
      sb.Append("</b></td>");

      if (info.ListType != "list") {
        sb.Append("<td><b>");
        header = info.HeaderNames != null && info.HeaderNames.Count > usedHeaderIndex ?
          info.HeaderNames[usedHeaderIndex++] : "Value";
        usedHeaders.Add(header);
        sb.Append(header);
        sb.Append("</b></td>");

        if (info.ListType != "check") {
          sb.Append("<td><b>");
          string replacementString = info.ListType == "remarks" || info.ListType == "dropdown" ? "Remarks" : "Ending";
          header = info.HeaderNames != null && info.HeaderNames.Count > usedHeaderIndex ?
            info.HeaderNames[usedHeaderIndex++] : replacementString;
          usedHeaders.Add(header);
          sb.Append(header);
          sb.Append("</b></td>");
        }
      }

      sb.Append("<td><b>Actions</b></td>");
      sb.Append("</tr>");
      int count = 1;
      foreach (var item in listColumnItems) {
        sb.Append("<tr>");

        sb.Append("<td>" + (info.ListType == "list" ? string.Empty : "<b>")); //if it is a list, then don't need to bold at this point
        sb.Append(item.Name?.ToCamelBrokenString());
        sb.Append("</td>" + (info.ListType == "list" ? string.Empty : "</b>"));

        if (item.Type == "list") {
          //nothing is there
        } else if (item.Type == "check") {
          sb.Append("<td><select");
          sb.Append(" class=\"common-subcolumn-input\"");
          sb.Append(" commoncolumnname=\"" + info.Name + "\"");
          sb.Append(" commoninputno=\"" + count + "\"");
          sb.Append(" commoninputtype=\"select\"");
          sb.Append(" commoninputpart=\"check\"");
          sb.Append(" id=\"common-subcolumn-input-checklist-" + info.Name + "-" + count + "\"");
          sb.Append(">");

          string def = item.Value?.ToLower()?.Trim();
          string selectedStr = " selected=\"selected\"";
          string addStr = string.IsNullOrWhiteSpace(def) ? selectedStr : string.Empty;
          sb.Append(string.Concat("<option value=\"\"", addStr, "></option>"));
          addStr = def == "true" ? selectedStr : string.Empty;
          sb.Append(string.Concat("<option value=\"True\"", addStr, ">True</option>"));
          addStr = def == "false" ? selectedStr : string.Empty;
          sb.Append(string.Concat("<option value=\"False\"", addStr, ">False</option>"));

          sb.Append("</select></td>");
        } else if (item.Type == "remarks") {

          sb.Append("<td><input");
          sb.Append(" class=\"common-subcolumn-input\"");
          sb.Append(" commoncolumnname=\"" + info.Name + "\"");
          sb.Append(" commoninputno=\"" + count + "\"");
          sb.Append(" commoninputtype=\"text\"");
          sb.Append(" commoninputpart=\"text\"");
          sb.Append(" id=\"common-subcolumn-input-text-" + info.Name + "-" + count + "\"");
          sb.Append(" type=\"text\" size=\"" + textAreaCol + "\" value=\"");
          sb.Append(item.Value);
          sb.Append("\" /></td>");

          sb.Append("<td><input");
          sb.Append(" class=\"common-subcolumn-input\"");
          sb.Append(" commoncolumnname=\"" + info.Name + "\"");
          sb.Append(" commoninputno=\"" + count + "\"");
          sb.Append(" commoninputtype=\"text\"");
          sb.Append(" commoninputpart=\"remarks\"");
          sb.Append(" id=\"common-subcolumn-input-remarks-" + info.Name + "-" + count + "\"");
          sb.Append(" type=\"text\" size=\"" + textAreaCol * 2 + "\" value=\"");
          sb.Append(item.Remarks);
          sb.Append("\" /></td>");
        } else if (item.Type == "dropdown") {

          sb.Append("<td><select");
          sb.Append(" class=\"common-subcolumn-input common-subcolumn-input-unit\"");
          sb.Append(" commoncolumnname=\"" + info.Name + "\"");
          sb.Append(" commoninputno=\"" + count + "\"");
          sb.Append(" commoninputtype=\"select\"");
          sb.Append(" commoninputpart=\"dropdown\"");
          sb.Append(" id=\"common-subcolumn-input-dropdown-" + info.Name + "-" + count + "\"");
          sb.Append(">");
          if (item.HasDropdownList) {
            if (string.IsNullOrWhiteSpace(item.Value)) {
              sb.Append("<option selected=\"selected\"></option>");
            } else {
              sb.Append("<option value=\"\"></option>");
            }
            foreach (var subitem in item.DropdownList) {
              sb.Append("<option value=\"");
              sb.Append(subitem);
              if (!string.IsNullOrWhiteSpace(item.Value) && subitem == item.Value)
                sb.Append("\" selected=\"selected");
              sb.Append("\">");
              sb.Append(subitem);
              sb.Append("</option>\n");
            }
          }

          sb.Append("</select></td>");
          sb.Append("<td><input");
          sb.Append(" class=\"common-subcolumn-input\"");
          sb.Append(" commoncolumnname=\"" + info.Name + "\"");
          sb.Append(" commoninputno=\"" + count + "\"");
          sb.Append(" commoninputtype=\"text\"");
          sb.Append(" commoninputpart=\"remarks\"");
          sb.Append(" id=\"common-subcolumn-input-remarks-" + info.Name + "-" + count + "\"");
          sb.Append(" type=\"text\" size=\"" + textAreaCol * 2 + "\" value=\"");
          sb.Append(item.Remarks);
          sb.Append("\" /></td>");
        } else {
          sb.Append("<td><input");
          sb.Append(" class=\"common-subcolumn-input\"");
          sb.Append(" commoncolumnname=\"" + info.Name + "\"");
          sb.Append(" commoninputno=\"" + count + "\"");
          sb.Append(" commoninputtype=\"text\"");
          sb.Append(" commoninputpart=\"text\"");
          sb.Append(" id=\"common-subcolumn-input-text-" + info.Name + "-" + count + "\"");
          sb.Append(" type=\"text\" size=\"" + textAreaCol + "\" value=\"");
          sb.Append(item.Value);
          sb.Append("\" /></td>");

          sb.Append("<td><select");
          sb.Append(" class=\"common-subcolumn-input common-subcolumn-input-unit\"");
          sb.Append(" commoncolumnname=\"" + info.Name + "\"");
          sb.Append(" commoninputno=\"" + count + "\"");
          sb.Append(" commoninputtype=\"select\"");
          sb.Append(" commoninputpart=\"unit\"");
          sb.Append(" id=\"common-subcolumn-input-unit-" + info.Name + "-" + count + "\"");
          sb.Append(">");
          if (item.HasEndingList) {
            if (string.IsNullOrWhiteSpace(item.Ending)) {
              sb.Append("<option selected=\"selected\"></option>");
            } else {
              sb.Append("<option value=\"\"></option>");
            }
            foreach (var subitem in item.EndingList) {
              sb.Append("<option value=\"");
              sb.Append(subitem);
              if (!string.IsNullOrWhiteSpace(item.Ending) && subitem == item.Ending)
                sb.Append("\" selected=\"selected");
              sb.Append("\">");
              sb.Append(subitem);
              sb.Append("</option>\n");
            }
          }

          sb.Append("</select></td>");
        }

        //Delete button
        sb.Append("<td><button");
        sb.Append(" class=\"common-subcolumn-button\"");
        sb.Append(" commonbuttontype=\"delete\"");
        sb.Append(" id=\"common-subcolumn-button-delete-" + info.Name + "-" + count + "\"");
        sb.Append(" commondeleteno=\"" + count + "\"");
        sb.Append(" commoncolumnname=\"" + info.Name + "\"");
        sb.Append(">Delete</button></td>");

        sb.Append("</tr>");

        count++;
      }

      //Add button
      sb.Append("<tr>");
      if (info.ListType != "list") //if it is a list, don't need to add empty item here...
        sb.Append("<td></td>");
      sb.Append("<td><textarea");
      sb.Append(" rows=\"" + textAreaRow + "\" cols=\"" + textAreaCol + "\"");
      sb.Append(" id=\"common-subcolumn-addtext-" + info.Name + "\"");
      if (info.ListType == "list") {
        sb.Append(" placeholder=\"");
        sb.Append(usedHeaders[0] + " 1; ");
        sb.Append(usedHeaders[0] + " 2; ...; ");
        sb.Append(usedHeaders[0] + " N");
        sb.Append("\"");
      } else if (info.ListType == "check") {
        sb.Append(string.Concat(" placeholder=\"", usedHeaders[0], " = True Or ", usedHeaders[0], " = False\""));
      } else if (info.ListType == "remarks") {
        sb.Append(string.Concat(" placeholder=\"", usedHeaders[0], " = ", usedHeaders[1], " | ", usedHeaders[2], "\""));
      } else if (info.ListType == "dropdown") {
        sb.Append(string.Concat(" placeholder=\"", usedHeaders[0], " = ", usedHeaders[1], " | Choice 1, Choice 2, ..., Choice N | ", usedHeaders[2], "\""));
      } else {
        sb.Append(string.Concat(" placeholder=\"", usedHeaders[0], " = ", usedHeaders[1], " | ", usedHeaders[2], " | Choice 1, Choice 2, ..., Choice N\""));
      }
      
      //TextArea
      sb.Append("></textarea></td>");
      sb.Append("<td><button");
      sb.Append(" class=\"common-subcolumn-button\"");
      sb.Append(" commonbuttontype=\"add\"");
      sb.Append(" id=\"common-subcolumn-button-add-" + info.Name + "\"");
      sb.Append(" commoncolumnname=\"" + info.Name + "\"");
      sb.Append(">Add</button></td>");
      sb.Append("</tr>");

      sb.Append("</table>");
      return sb.ToString();
    }
    
    public static string GetDetailsHTML(this ListColumnInfo info, string dataValue) {
      var listColumnItems = dataValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
        ?.Select(x => new ListColumnItem(x.Trim(), info.ListType)).ToList();

      if (listColumnItems.Count <= 0)
        return null;

      StringBuilder sb = new StringBuilder();

      int usedHeaderIndex = 0;
      sb.Append("<table style=\"border-collapse:separate;border-spacing:10px 5px;border:1px solid black\">");
      sb.Append("<tr>");
      sb.Append("<td><b>"); //+ (firstSample.Type == "remarks" ? "Name" : "Sub-Item") + "</b></td>");
      sb.Append(info.HeaderNames != null && info.HeaderNames.Count > usedHeaderIndex ?
        info.HeaderNames[usedHeaderIndex++] : "Sub-Item");
      sb.Append("</b></td>");

      if (info.ListType != "list") { //a list only have name
        sb.Append("<td><b>");
        sb.Append(info.HeaderNames != null && info.HeaderNames.Count > usedHeaderIndex ?
          info.HeaderNames[usedHeaderIndex++] : "Value");
        sb.Append("</b></td>");
        if (info.ListType == "remarks" || info.ListType == "dropdown") {
          sb.Append("<td><b>");
          sb.Append(info.HeaderNames != null && info.HeaderNames.Count > usedHeaderIndex ?
            info.HeaderNames[usedHeaderIndex++] : "Remarks");
          sb.Append("</b></td>");
        }
      }

      sb.Append("</tr>");

      foreach (var item in listColumnItems) {
        sb.Append("<tr>");
        sb.Append("<td>");
        sb.Append(item.Name?.ToCamelBrokenString());
        sb.Append("</td>");
        if (info.ListType != "list") { //list only have one item
          sb.Append("<td><i>");
          sb.Append(item.Value + " " + item.Ending);
          sb.Append("</i></td>");
          if (item.Type == "remarks" || item.Type == "dropdown") {
            sb.Append("<td><i>");
            sb.Append(item.Remarks);
            sb.Append("</i></td>");
          }
        }
        sb.Append("</tr>");
      }

      sb.Append("</table>");

      return sb.ToString();
    }
  }
}
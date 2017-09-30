using Aibe.Models.Core;
using System.Text;

namespace Aiwe.Extensions {
  public static class LiveDropDownResultExtension {
    public static string BuildDropdownString(this LiveDropDownResult result) {
      StringBuilder sb = new StringBuilder();
      sb.Append("<select class=\"form-control form-control-common-plus common-column-dropdown\" id=\"");
      sb.Append("common-column-dropdown-" + result.ColumnName);
      sb.Append("\" name=\"");
      sb.Append(result.ColumnName);
      string prevValue = result.Arg == null || result.Arg == null || string.IsNullOrWhiteSpace(result.Arg.Value.ToString()) ?
        string.Empty : result.Arg.Value.ToString();
      if (string.IsNullOrWhiteSpace(prevValue))
        sb.Append("\"><option selected=\"selected\"></option>\n");
      else
        sb.Append("\"><option value=\"\"></option>\n");
      if (result.Values != null) { //if the null is put for the second time, then you cannot choose as freely TODO probably should return to everything?
        if (!string.IsNullOrWhiteSpace(result.ArgOriginalValue) && !result.Values.Contains(result.ArgOriginalValue))
          result.Values.Insert(0, result.ArgOriginalValue);
        foreach (var val in result.Values) {
          sb.Append("<option value=\"");
          sb.Append(val);
          if (!string.IsNullOrWhiteSpace(prevValue) && val == prevValue)
            sb.Append("\" selected=\"selected");
          sb.Append("\">");
          sb.Append(val);
          sb.Append("</option>\n");
        }
      }
      sb.Append("</select>");
      return sb.ToString();
    }
  }
}

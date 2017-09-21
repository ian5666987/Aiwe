using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aibe.Helpers {
  public partial class StringHelper { //TODO can be made string extension
    public static string ProcessAsSqlStringValue(string input) {
      return "'" + GetSqlSafeStringValue(input) + "'";
    }

    public static string GetSqlSafeStringValue(string input) {
      return string.IsNullOrWhiteSpace(input) ? "" : input.Replace("'", "''");
    }

    public static List<string> CreateSqlValueList(string input) {
      List<string> components = new List<string>();
      StringBuilder sb = new StringBuilder();
      bool openAposthropeFound = false;
      string str = string.Empty;
      char[] chArr = input.ToCharArray();
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
                openAposthropeFound = false;
              }
            }
          } else
            sb.Append(ch);
          continue;
        } else {
          if (ch == ',') { //comma is found when not in the open aposthrope, means this is one element
            components.Add(sb.ToString());
            sb = new StringBuilder();
          } else if(ch == '\'') { //the open aposthrope is found here
            openAposthropeFound = true;
            sb.Append(ch); //add this aposthrope character
          } else
            sb.Append(ch); //if not comma, just add the element
        }
      }
      str = sb.ToString();
      if (!string.IsNullOrWhiteSpace(str))
        components.Add(str);
      return components.Select(x => x.Trim()).ToList();
    }
  }
}

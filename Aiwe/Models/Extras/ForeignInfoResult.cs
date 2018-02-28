using System.Collections.Generic;

namespace Aiwe.Models.Extras {
  public class ForeignInfoResult {
    public string Name { get; private set; }
    public string ViewString { get; set; }
    public ForeignInfoResult(KeyValuePair<string, object> item) {
      Name = item.Key;
      ViewString = "<p class=\"common-foreign-info-value " + Aiwe.DH.ControlClasses + 
        "\" value=\"" + "\" readonly=\"readonly\">" + (item.Value ?? string.Empty) + "</p>";
    }
  }
}

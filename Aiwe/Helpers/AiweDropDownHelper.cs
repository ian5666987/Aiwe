using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace Aiwe.Helpers {
  public class AiweDropDownHelper {
    public static List<SelectListItem> GetBooleanOptions() {
      return Aibe.LCZ.GetLocalizedBooleanDropDownOptions()
        .Select(x => new SelectListItem { Text = x.Key, Value = x.Value })
        .ToList();
    }
  }
}

using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Aiwe.Helpers {
  public class AiweDropDownHelper {
    public static List<SelectListItem> GetBooleanOptions() {
      return new List<SelectListItem> {
        new SelectListItem { Text = null, Value = null },
        new SelectListItem { Text = Aibe.LCZ.W_BoTrue, Value = Aibe.DH.BvTrue },
        new SelectListItem { Text = Aibe.LCZ.W_BoFalse, Value = Aibe.DH.BvFalse },
      };
    }
  }
}

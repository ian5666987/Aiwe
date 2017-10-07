using System;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNet.Identity;

namespace Aiwe.Extensions {
  public static class IdentityExtension {
    public static string GetDisplayName(this IIdentity identity) {
      if (identity == null) {
        throw new ArgumentNullException("identity");
      }
      var ci = identity as ClaimsIdentity;
      if (ci != null) {
        return ci.FindFirstValue(Aiwe.DH.UserDisplayName);
      }
      return null;
    }
  }
}

using System.Collections.Generic;
using System.Linq;
using Extension.String;

namespace Aibe.Models {
  public partial class ExclusionInfo : CommonBaseInfo {
    public List<string> Roles { get; private set; } = new List<string>();
    public ExclusionInfo(string desc) : base(desc) {
      if (HasRightSide)
        Roles = RightSide.GetTrimmedNonEmptyParts(',');
    }

    public bool IsExcluded(string role) { //if roles are empty means everybody is alloweds
      if (DH.MainAdminRoles.Any(x => x.EqualsIgnoreCase(role))) //role in the main admin roles cannot be excluded
        return false;
      return Roles != null && Roles.Any(x => x.EqualsIgnoreCase(role));
    }
  }
}
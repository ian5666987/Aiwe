using System.Linq;
using Aiwe.Helpers;
using Aibe.Models;
using System.Security.Principal;
using Extension.String;
using System.Collections.Generic;

namespace Aiwe.Models {
  public class DetailsInfo : BaseTableInfo {
    public int Cid { get; private set; }

    public DetailsInfo(MetaInfo metaInput, int id, Dictionary<string, string> stringDictionary) : base(metaInput, stringDictionary) {
      Cid = id;
    }

    public bool IsColumnIncludedInDetails(string columnName, IPrincipal user) {
      if (AiweUserHelper.UserHasMainAdminRight(user)) //if user is in main admin rights, it is always true
        return true;
      return IsColumnIncluded(Meta.DetailsExclusions, columnName, user);
    }
  }
}
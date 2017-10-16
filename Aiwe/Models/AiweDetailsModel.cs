using System.Linq;
using Aiwe.Helpers;
using Aibe.Models;
using System.Security.Principal;
using System.Collections.Generic;

namespace Aiwe.Models {
  public class AiweDetailsModel : AiweBaseTableModel {
    public int Cid { get; private set; }

    public AiweDetailsModel(MetaInfo metaInput, int id, Dictionary<string, string> stringDictionary) : base(metaInput, stringDictionary) {
      Cid = id;
    }

    public bool IsColumnIncludedInDetails(string columnName, IPrincipal user) {
      if (AiweUserHelper.UserHasMainAdminRight(user)) //if user is in main admin rights, it is always true
        return true;
      return IsColumnIncluded(Meta.DetailsExclusions, columnName, user) && !Meta.IsPrefilledColumn(columnName);
    }
  }
}
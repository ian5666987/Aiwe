using System.Linq;
using Aibe.Helpers;
using Aibe.Models;
using System.Security.Principal;
using Extension.String;

namespace Aiwe.Models {
  public class DetailsInfo : BaseTableInfo {
    public int Cid { get; private set; }

    public DetailsInfo(MetaInfo metaInput, int id) : base(metaInput) {
      Cid = id;
    }

    //Use IsColumnIncludedInDetails
    //public bool IsIncluded(IPrincipal user, string columnName) {
    //  return IsColumnIncludedInDetails(columnName, user);
    //}

    //Use IsPictureColumn
    //public bool ShowImage(string columnName) {
    //  return pictureLinkColumns != null && pictureLinkColumns.Contains(columnName.ToLower());
    //}

    //Provided in the BaseTableInfo
    //public int GetImageWidth(string columnName) {
    //  return ActionFilterHelper.GetSize(pictureLinkColumnsDescription, columnName);
    //}

    public bool IsColumnIncludedInDetails(string columnName, IPrincipal user, bool isWebApi = false) {
      if (UserHelper.UserHasMainAdminRight(user)) //if user is in main admin rights, it is always true
        return true;
      if (Meta.DetailsExclusions == null || Meta.DetailsExclusions.Count <= 0) //if there is no column exclusion, then the column is definitely included
        return true;
      ExclusionInfo exInfo = Meta.DetailsExclusions.FirstOrDefault(x => x.Name.EqualsIgnoreCaseTrim(columnName));
      if (exInfo == null) //non specified column exclusion is allowed
        return true;
      bool isExplicitlyExcluded = exInfo.Roles.Any(x => user.IsInRole(x));
      return !isExplicitlyExcluded; //if user is not explicitly excluded, then he is definitely included.
    }
  }
}
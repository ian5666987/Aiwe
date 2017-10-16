using Aibe.Models;
using Aibe.Models.Core;
using Extension.String;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;

namespace Aiwe.Models {
  public class AiweCreateEditModel : AiweBaseTableModel {
    public string ActionType { get; set; } = Aibe.DH.CreateActionName; //create OR edit, default is create
    public int CreateEditLabelPortion { get; private set; }
    public List<DropDownInfo> DropDowns { get { return Meta.CreateEditDropDowns; } }

    public AiweCreateEditModel(MetaInfo metaInput, string actionType, Dictionary<string, string> stringDictionary) : base(metaInput, stringDictionary) {
      ActionType = actionType;
      //TODO, currently all these are hardcoded, not put in DHs
      List<string> columnNames = Meta.ArrangedDataColumns.Select(x => Meta.GetColumnDisplayName(x.ColumnName)).ToList();
      CreateEditLabelPortion = columnNames.Any(x => x.Length >= 30) ? 3 : 2;
    }

    public virtual bool IsColumnIncludedInCreateEdit(DataColumn column, IPrincipal user) {
      if (column.Unique || column.ColumnName.EqualsIgnoreCase(Aibe.DH.Cid)) //Cid and unique column always not included in the create and edit
        return false;
      return IsColumnIncluded(Meta.CreateEditExclusions, column.ColumnName, user) && !Meta.IsPrefilledColumn(column.ColumnName);
    }
  }
}
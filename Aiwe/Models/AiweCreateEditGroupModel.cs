using Aibe.Models;
using Aibe.Models.Core;
using Extension.String;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Aiwe.Models {
  public class AiweCreateEditGroupModel : AiweBaseTableModel {
    public string ActionType { get; set; } = Aibe.DH.CreateGroupActionName; //create OR edit, default is create
    public int CreateEditLabelPortion { get; private set; }
    public List<DropDownInfo> DropDowns { get { return Meta.CreateEditDropDowns; } }
    public List<string> IdentifierColumns { get; private set; }
    public List<KeyValuePair<string, object>> IdentifierResults { get; private set; } = new List<KeyValuePair<string, object>>();

    public AiweCreateEditGroupModel(MetaInfo metaInput, string actionType, Dictionary<string, string> stringDictionary, 
      List<string> identifierColumns, List<KeyValuePair<string, object>> identifierResults = null) : base(metaInput, stringDictionary) {
      ActionType = actionType;
      IdentifierColumns = identifierColumns;
      if (identifierResults != null && actionType.EqualsIgnoreCase(Aibe.DH.EditGroupActionName)) //only EditGroup has identifier results in the first place
        IdentifierResults = identifierResults;      
      //TODO, currently all these are hardcoded, not put in DHs
      List<string> columnNames = Meta.ArrangedDataColumns.Select(x => Meta.GetColumnDisplayName(x.ColumnName)).ToList();
      CreateEditLabelPortion = columnNames.Any(x => x.Length >= 30) ? 3 : 2;
    }
  }
}
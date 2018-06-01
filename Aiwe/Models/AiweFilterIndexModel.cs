using System.Collections.Generic;
using System.Security.Principal;
using Aibe.Models;
using System.Data;

namespace Aiwe.Models {
  //To be used to display filter and index
  public class AiweFilterIndexModel : AiweBaseFilterIndexModel {
    public FilterIndexModel FiModel { get { return (FilterIndexModel)BaseModel; } }
    public List<DataRow> IdentifierIndexRows { get; private set; } = new List<DataRow>(); //v1.4.1.0 used to make rows for identifiers purpose

    public AiweFilterIndexModel(MetaInfo meta, IPrincipal userInput, FilterIndexModel model,
      Dictionary<string, string> stringDictionary) : base(meta, userInput, model, stringDictionary) {

      //v1.4.1.0 to handle rows for identifiers
      IdentifierIndexRows.Clear();
      foreach (DataRow row in model.ForeignIdentifiersData.Rows)
        IdentifierIndexRows.Add(row);
    }
  }
}
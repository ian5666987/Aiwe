using System.Collections.Generic;
using System.Security.Principal;
using Aibe.Models;

namespace Aiwe.Models {
  //To be used to display filter and index
  public class AiweFilterGroupDetailsModel : AiweBaseFilterIndexModel {
    public FilterGroupDetailsModel GdModel { get { return (FilterGroupDetailsModel)BaseModel; } }
    public bool IsGroupDeletion { get; private set; }
    public AiweFilterGroupDetailsModel(MetaInfo meta, IPrincipal userInput, FilterGroupDetailsModel model,
      bool isGroupDeletion, Dictionary<string, string> stringDictionary) : base(meta, userInput, model, stringDictionary) {
      IsGroupDeletion = isGroupDeletion;
    }
  }
}
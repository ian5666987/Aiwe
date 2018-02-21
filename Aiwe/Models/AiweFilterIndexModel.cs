using System.Collections.Generic;
using System.Security.Principal;
using Aibe.Models;

namespace Aiwe.Models {
  //To be used to display filter and index
  public class AiweFilterIndexModel : AiweBaseFilterIndexModel {
    public FilterIndexModel FiModel { get { return (FilterIndexModel)BaseModel; } }

    public AiweFilterIndexModel(MetaInfo meta, IPrincipal userInput, FilterIndexModel model,
      Dictionary<string, string> stringDictionary) : base(meta, userInput, model, stringDictionary) {
    }
  }
}
using Aibe.Helpers;
using System;

namespace Aiwe.Helpers {
  public class AiweCheckerHelper : CheckerHelper { //This, unfortunately, is bound to the running assembly and thus must be declared in its own assembly everytime
    protected override Type getTableType(string prefix, string tableSource) { //this can also be done pretty nicely if we have provided the delegate for the GetTableType in the CheckerHelper
      return Type.GetType(string.Concat(prefix, tableSource));
    }
  }
}

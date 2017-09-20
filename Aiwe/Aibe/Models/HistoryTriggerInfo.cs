using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aibe.Models {
  public partial class HistoryTriggerInfo : BaseInfo { //likely to be unique, not common
    public HistoryTriggerInfo(string desc) : base(desc) {
      if (string.IsNullOrWhiteSpace(desc))
        return;
      //TODO implement this
    }
  }
}
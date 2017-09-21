using System.Collections.Generic;
using System.Linq;
using Extension.String;

namespace Aibe.Models.Core {
  public partial class OrderByInfo : CommonBaseInfo {
    public string OrderDirection { get; private set; }
    private static List<string> validOrderDirections = new List<string>() { "ASC", "DESC" };
    public OrderByInfo(string desc) : base(desc) {
      if (HasRightSide && validOrderDirections.Any(x => x.EqualsIgnoreCaseTrim(RightSide)))
        OrderDirection = validOrderDirections.FirstOrDefault(x => x.EqualsIgnoreCaseTrim(RightSide));
    }

    public string GetOrderDirection () { return string.IsNullOrWhiteSpace(OrderDirection) ? string.Empty : OrderDirection; }
  }
}
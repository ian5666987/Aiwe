using System.Collections.Generic;
using System.Linq;
using Extension.String;

namespace Aibe.Models.Core {
  public partial class DropDownInfo : CommonBaseInfo {
    private static List<string> validOrderByDirectives = new List<string>() { "ASC", "DESC" };
    public string OrderByDirective { get; private set; } //by default this is null, unless this is specified as asc or desc
    public List<DropDownItemInfo> Items { get; private set; } = new List<DropDownItemInfo>();
    
    public DropDownInfo(string desc) : base(desc) { //Each Info should be like Info1=1,2,3,[RInfo1],[RInfo2],...
      if (!HasRightSide) { //must have right side
        IsValid = false;
        return;
      }

      var parts = RightSide.GetTrimmedNonEmptyParts(',');

      string possibleDirectiveContent = parts.FirstOrDefault(x => x.GetNonEmptyTrimmedInBetween("{", "}") != null);
      if (!string.IsNullOrWhiteSpace(possibleDirectiveContent) && validOrderByDirectives.Any(x => x.Equals(possibleDirectiveContent.ToUpper()))) //has something and valid
        OrderByDirective = possibleDirectiveContent.ToUpper();        

      Items = parts.Where(x => !x.StartsWith("{") && !x.EndsWith("}")) //select whatever is not order by directive
        .Select(x => new DropDownItemInfo(x.Trim())).Where(x => x.IsValid).ToList();

      IsValid = Items != null && Items.Any();
    }
  }
}
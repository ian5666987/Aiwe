namespace Aibe.Models.Core {
  public partial class AffixColumnInfo : CommonBaseInfo {
    public string AffixValue { get; private set; }
    public AffixColumnInfo(string desc) : base(desc) {
      if (!HasRightSide) {
        IsValid = false;
        return;
      }
      AffixValue = RightSide;
    }
  }
}
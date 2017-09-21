namespace Aibe.Models.Core {
  public partial class PictureColumnInfo : CommonBaseInfo {
    public const int DefaultWidth = 100;
    public int Width { get; private set; } = DefaultWidth;
    public PictureColumnInfo(string desc) : base(desc) {
      if (HasRightSide) {
        int value;
        bool result = int.TryParse(RightSide, out value);
        if (result && value > 0) //if the parsing is successful and the value is positive, only then we can take it
          Width = value;
      }
    }
  }
}
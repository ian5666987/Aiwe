using Extension.Cryptography;

namespace Aibe.Helpers {
  public partial class CryptographyHelper {
    public static void Init() {
      Cryptography.Extension = "astriocfile";
      Cryptography.Password = "astrioencryptor";
      Cryptography.AesKey = new byte[] {
        0x03, 0x12, 0x19, 0x65,
        0x25, 0x12, 0x00, 0x01,
        0x03, 0x05, 0x19, 0x87,
        0x01, 0x07, 0x20, 0x13
      };
    }
  }
}

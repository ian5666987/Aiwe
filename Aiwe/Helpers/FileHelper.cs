using System.IO;
using System.Web;

namespace Aiwe.Helpers {
  public partial class FileHelper {
    public static void SaveAttachments(HttpRequestBase request, string folderPath) {
      if (request != null && request.Files != null && request.Files.Count > 0)
        for (int i = 0; i < request.Files.Count; ++i) {
          var file = request.Files[i];
          if (file != null && file.ContentLength > 0) {
            var fileName = Path.GetFileName(file.FileName);
            Directory.CreateDirectory(folderPath);
            var path = Path.Combine(folderPath, fileName);
            file.SaveAs(path);
          }
        }
    }
  }
}

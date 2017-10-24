using Aibe.Helpers;
using System;
using System.IO;
using System.Web;

namespace Aiwe.Helpers {
  public class AiweFileHelper {
    public static bool SaveAttachments(HttpRequestBase request, string folderPath) {
      try {
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
        return true;
      } catch (Exception ex) {
        LogHelper.Error("NA", "SaveAttachments", "NA", "AiweFileHelper", "NA", "Edit", folderPath, ex.ToString());
        return false;
      }
    }
  }
}

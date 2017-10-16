using Aiwe.Helpers;
using Aiwe.Models.DB;
using Extension.Cryptography;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Aiwe.Controllers {
  [Authorize(Roles = Aibe.DH.DevRole)]
  public class MetaItemController : Controller {
    CoreDataModel db = new CoreDataModel();

    private RedirectToRouteResult redirectToError(string error) {
      return RedirectToAction(Aiwe.DH.ErrorLocalActionName, new { error = error });
    }

    public ActionResult ErrorLocal(string error) {
      ViewBag.Error = error;
      return View(Aiwe.DH.ErrorViewName);
    }

    public ActionResult ApplyUpdates(int id) {
      MetaItem meta = db.MetaItems.FirstOrDefault(x => x.Cid == id);
      if (meta == null)
        return redirectToError(Aibe.LCZ.NFE_IdNotFound);
      AiweTableHelper.UpdateMeta(meta);
      return RedirectToAction(Aiwe.DH.SuccessActionName, new { msg = string.Format(Aibe.LCZ.M_ItemIsUpdated, id) });
    }

    public ActionResult ApplyAllUpdates() {
      List<MetaItem> metas = db.MetaItems.ToList();
      foreach (var meta in metas)
        AiweTableHelper.UpdateMeta(meta);
      return RedirectToAction(Aiwe.DH.SuccessActionName, new { msg = string.Format(Aibe.LCZ.M_MetaItemsAreUpdated, metas.Count) });
    }

    public ActionResult CryptoSerialize(int id) {
      MetaItem meta = db.MetaItems.FirstOrDefault(x => x.Cid == id);
      if (meta == null)
        return redirectToError(Aibe.LCZ.NFE_IdNotFound);
      return View(meta);
    }

    [HttpPost]
    [ActionName(Aibe.DH.CryptoSerializeAction)]
    public ActionResult CryptoSerializePost(int id) {
      MetaItem meta = db.MetaItems.FirstOrDefault(x => x.Cid == id);
      if (meta == null)
        return redirectToError(Aibe.LCZ.NFE_IdNotFound);
      string folderPath = Server.MapPath("~/" + Aibe.DH.DefaultSettingFolderName);
      Cryptography.CryptoSerialize(meta, folderPath, meta.TableName);
      return RedirectToAction(Aiwe.DH.SuccessActionName, new { msg = string.Format(Aibe.LCZ.M_MetaItemIsCryptoSerialized, id) });
    }

    public ActionResult CryptoSerializeAll() {
      var all = db.MetaItems.ToList();
      var fileNames = all.Select(x => x.TableName).ToList();
      string folderPath = Server.MapPath("~/" + Aibe.DH.DefaultSettingFolderName);
      Cryptography.CryptoSerializeAll(all, folderPath, fileNames);
      return RedirectToAction(Aiwe.DH.SuccessActionName, new { msg = string.Format(Aibe.LCZ.M_CryptoSerializeAllSuccess, all.Count) });
    }

    public ActionResult Success(string msg) {
      ViewBag.SuccessMessage = msg;
      return View();
    }

    public ActionResult DecryptoSerializeAll() {
      string folderPath = Server.MapPath("~/" + Aibe.DH.DefaultSettingFolderName);
      int count = AiweTableHelper.DecryptMetaItems(folderPath);
      return RedirectToAction(Aiwe.DH.SuccessActionName, new { msg = string.Format(Aibe.LCZ.M_DecryptoSerializeAllSuccess, count, Aibe.LCZ.W_CryptExtension.ToUpper()) });
    }
  }
}
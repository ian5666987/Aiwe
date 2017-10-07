using System.Web.Mvc;

namespace Aiwe.Controllers {
  public class HomeController : Controller {
    public ActionResult Index() {
      return View();
    }

    public ActionResult About() {
      ViewBag.Message = Aibe.LCZ.D_About;

      return View();
    }

    public ActionResult Contact() {
      ViewBag.Message = Aibe.LCZ.D_Contact;

      return View();
    }

    public ActionResult TableNotReady() {
      return View();
    }

    public ActionResult ActionNotFound(int? errorNo) {
      return View();
    }

    public ActionResult TableDescriptionNotFound(int? errorNo) {
      return View();
    }

    public ActionResult InsufficientAccessRightPage(int? errorNo) {
      return View();
    }

    public ActionResult InsufficientAccessRightAction(int? errorNo) {
      return View();
    }

    public ActionResult Error() {
      return View();
    }
  }
}
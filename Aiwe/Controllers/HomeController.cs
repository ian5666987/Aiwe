using System.Web.Mvc;

namespace Aiwe.Controllers {
  public class HomeController : Controller {
    public ActionResult Index() {
      return View();
    }

    public ActionResult About() {
      ViewBag.Message = "Your application description page.";

      return View();
    }

    public ActionResult Contact() {
      ViewBag.Message = "Your contact page.";

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
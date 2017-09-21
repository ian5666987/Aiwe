using System.Web.Mvc;

namespace Aiwe.Controllers {
  [Authorize(Roles = Aiwe.DH.AdminAuthorizedRoles)]
  public class AdminController : Controller {

    public ActionResult Index() { //Where all common tables are returned as list
      return View();
    }
  }
}


using System.Web.Mvc;
using Aibe;

namespace Aiwe.Controllers {
  [Authorize(Roles = DH.AdminAuthorizedRoles)]
  public class AdminController : Controller {

    public ActionResult Index() { //Where all common tables are returned as list
      return View();
    }
  }
}


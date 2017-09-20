using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Aiwe.Models;
using System.Data.Entity.Migrations;
using Aiwe.Extensions;
using System.Security.Claims;
using Aibe.Models.DB;
using Aibe.Helpers;

namespace Aiwe.Controllers {
  [Authorize]
  public class ManageController : Controller {
    private ApplicationSignInManager _signInManager;
    private ApplicationUserManager _userManager;

    public ManageController() {
    }

    public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager) {
      UserManager = userManager;
      SignInManager = signInManager;
    }

    public ApplicationSignInManager SignInManager {
      get {
        return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
      }
      private set {
        _signInManager = value;
      }
    }

    public ApplicationUserManager UserManager {
      get {
        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
      }
      private set {
        _userManager = value;
      }
    }

    ApplicationDbContext context = new ApplicationDbContext();
    CoreDataModel db = new CoreDataModel();
    //
    // GET: /Manage/Index
    public async Task<ActionResult> Index(ManageMessageId? message) {
      ViewBag.StatusMessage =
          message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
          : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
          : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
          : message == ManageMessageId.Error ? "An error has occurred."
          : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
          : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
          : message == ManageMessageId.ChangeDisplayNameSuccess ? "Your display name has been changed."
          : message == ManageMessageId.UserNotFound ? "User not found."
          : message == ManageMessageId.UserNotFound ? "User claim not found."
          : "";

      var userId = User.Identity.GetUserId();
      ApplicationUser user = context.Users.SingleOrDefault(x => x.Id == userId);
      var model = new IndexViewModel {
        HasPassword = HasPassword(),
        PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
        TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
        Logins = await UserManager.GetLoginsAsync(userId),
        BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
        FullName = user.FullName,
        DisplayName = user.DisplayName,
        Team = user.Team,
        WorkingRole = user.WorkingRole,
        AdminRole = user.AdminRole,
        RegistrationDate = user.RegistrationDate,
        LastLogin = user.LastLogin,        
      };
      return View(model);
    }

    //
    // GET: /Manage/ChangePassword
    public ActionResult ChangePassword() {
      return View();
    }

    //
    // POST: /Manage/ChangePassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model) {
      if (!ModelState.IsValid) {
        return View(model);
      }
      var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
      if (result.Succeeded) {
        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        if (user != null) {
          await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        }
        UserHelper.SetUserMapPassword(db, user.UserName, model.NewPassword);
        return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
      }
      AddErrors(result);
      return View(model);
    }


    public ActionResult ChangeDisplayName() {
      ChangeDisplayNameViewModel model = new ChangeDisplayNameViewModel {
        OldDisplayName = User.Identity.GetDisplayName(),        
      };      
      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ChangeDisplayName(ChangeDisplayNameViewModel model) {
      if (!ModelState.IsValid)
        return View(model);
      
      var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
      if(user == null)
        return RedirectToAction("Index", new { Message = ManageMessageId.UserNotFound }); //not found

      var userIdentity = await user.GenerateUserIdentityAsync(UserManager);
      var claim = userIdentity.Claims
        .FirstOrDefault(x => x.Type == "DisplayName" && x.Value == model.OldDisplayName);
      if (claim == null)
        return RedirectToAction("Index", new { Message = ManageMessageId.UserClaimNotFound }); //claim not found

      userIdentity.RemoveClaim(claim);
      userIdentity.AddClaim(new Claim("DisplayName", model.NewDisplayName));
      AuthenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant
        (new ClaimsPrincipal(userIdentity), new AuthenticationProperties { IsPersistent = true });

      user.DisplayName = model.NewDisplayName;
      context.Users.AddOrUpdate(user);
      context.SaveChanges();

      return RedirectToAction("Index", new { Message = ManageMessageId.ChangeDisplayNameSuccess });
    }

    //
    // GET: /Manage/SetPassword
    public ActionResult SetPassword() {
      return View();
    }

    //
    // POST: /Manage/SetPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> SetPassword(SetPasswordViewModel model) {
      if (ModelState.IsValid) {
        var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
        if (result.Succeeded) {
          var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
          if (user != null) {
            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
          }
          UserHelper.SetUserMapPassword(db, user.UserName, model.NewPassword);
          return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
        }
        AddErrors(result);
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    protected override void Dispose(bool disposing) {
      if (disposing && _userManager != null) {
        _userManager.Dispose();
        _userManager = null;
      }

      base.Dispose(disposing);
    }

    #region Helpers
    // Used for XSRF protection when adding external logins
    private const string XsrfKey = "XsrfId";

    private IAuthenticationManager AuthenticationManager {
      get {
        return HttpContext.GetOwinContext().Authentication;
      }
    }

    private void AddErrors(IdentityResult result) {
      foreach (var error in result.Errors) {
        ModelState.AddModelError("", error);
      }
    }

    private bool HasPassword() {
      var user = UserManager.FindById(User.Identity.GetUserId());
      if (user != null) {
        return user.PasswordHash != null;
      }
      return false;
    }

    private bool HasPhoneNumber() {
      var user = UserManager.FindById(User.Identity.GetUserId());
      if (user != null) {
        return user.PhoneNumber != null;
      }
      return false;
    }

    public enum ManageMessageId {
      AddPhoneSuccess,
      ChangePasswordSuccess,
      SetTwoFactorSuccess,
      SetPasswordSuccess,
      RemoveLoginSuccess,
      RemovePhoneSuccess,
      ChangeDisplayNameSuccess,
      UserNotFound,
      UserClaimNotFound,
      Error
    }

    #endregion
  }
}
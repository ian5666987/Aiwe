using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Aiwe.Extensions;
using Aiwe.Models;
using Aiwe.Models.DB;
using System.Data.Entity.Migrations;
using System.Security.Claims;
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
          message == ManageMessageId.ChangePasswordSuccess ? Aibe.LCZ.NFM_ChangePasswordSuccess
          : message == ManageMessageId.SetPasswordSuccess ? Aibe.LCZ.NFM_SetPasswordSuccess
          : message == ManageMessageId.SetTwoFactorSuccess ? Aibe.LCZ.NFM_SetTwoFactorSuccess
          : message == ManageMessageId.Error ? Aibe.LCZ.NFE_GeneralError
          : message == ManageMessageId.AddPhoneSuccess ? Aibe.LCZ.NFM_AddPhoneSuccess
          : message == ManageMessageId.RemovePhoneSuccess ? Aibe.LCZ.NFM_RemovePhoneSuccess
          : message == ManageMessageId.ChangeDisplayNameSuccess ? Aibe.LCZ.NFM_ChangeDisplayNameSuccess
          : message == ManageMessageId.UserNotFound ? Aibe.LCZ.NFE_UserNotFound
          : message == ManageMessageId.UserClaimNotFound ? Aibe.LCZ.NFE_UserClaimNotFound
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
        UserHelper.SetUserMapPassword(user.UserName, model.NewPassword);
        return RedirectToAction(Aibe.DH.IndexActionName, new { Message = ManageMessageId.ChangePasswordSuccess });
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
        return RedirectToAction(Aibe.DH.IndexActionName, new { Message = ManageMessageId.UserNotFound }); //not found

      var userIdentity = await user.GenerateUserIdentityAsync(UserManager);
      var claim = userIdentity.Claims
        .FirstOrDefault(x => x.Type == Aiwe.DH.UserDisplayName && x.Value == model.OldDisplayName);
      if (claim == null)
        return RedirectToAction(Aibe.DH.IndexActionName, new { Message = ManageMessageId.UserClaimNotFound }); //claim not found

      userIdentity.RemoveClaim(claim);
      userIdentity.AddClaim(new Claim(Aiwe.DH.UserDisplayName, model.NewDisplayName));
      AuthenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant
        (new ClaimsPrincipal(userIdentity), new AuthenticationProperties { IsPersistent = true });

      user.DisplayName = model.NewDisplayName;
      context.Users.AddOrUpdate(user);
      context.SaveChanges();

      return RedirectToAction(Aibe.DH.IndexActionName, new { Message = ManageMessageId.ChangeDisplayNameSuccess });
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
          UserHelper.SetUserMapPassword(user.UserName, model.NewPassword);
          return RedirectToAction(Aibe.DH.IndexActionName, new { Message = ManageMessageId.SetPasswordSuccess });
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
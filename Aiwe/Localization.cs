using System.Collections.Generic;
using Aibe;

namespace Aiwe {
  public class LCZ { //can be extended as wanted
    public static void Init() {
      Aibe.LCZ.NFM_FootNotesRequired = "Notes: * required field, ** at least <b>one</b> must be filled";
      //Localize Aibe items whenever necessary (best is using xml file)
    }

    //Errors
    public static string E_RolesCannotBeBothEmpty = "[{0}] and [{1}] cannot be both empty";

    //Non-formatted error
    public static string NFE_NotWellFormedRequest = "Not-well-formed request";
    public static string NFE_QueryScriptCheckingFails = "Query script checking fails";
    public static string NFE_QueryCreationFails = "Query creation fails";
    public static string NFE_BadRequestOnResultMaking = "Bad request on result making";
    public static string NFE_DataRequestedNotFound = "Data requested not found";

    //Items
    public static string I_WorkingRole = "WorkingRole";
    public static string I_AdminRole = "AdminRole";

    //Words
    public static string W_AdditionalMessage = "Additional Message";
    public static string W_RoleViewModel = "Role View Model";
    public static string W_ApplicationUserModel = "Application User Model";
    public static string W_HomeTeam = "Home";
    public static string W_WorkingRole = "Working Role";
    public static string W_AdminRole = "Admin Role";
    public static string W_OrderCheck = "Order Check";
    public static string W_FilterCheck = "Filter Check";

    //Descriptions
    public static string D_DoNotBother = "DO NOT BOTHER the 'No file chosen' phrase. If you see a picture, it means THERE IS a picture linked to the data.";
    public static string D_DoNotBotherAttachment = "DO NOT BOTHER the 'No file chosen' phrase. Check if the attachment name is correct.";

    //Final localization, cannot be further derived from
    public class F {
      public class ApplicationUserFilter {
        public const string FullName = "Full Name";
        public const string DisplayName = "Display Name";
        public const string Email = "Email";
        public const string Team = "Team";
        public const string WorkingRole = "Working Role";
        public const string AdminRole = "Admin Role";
      }

      public class LoginViewModel {
        public const string Email = "Email";
        public const string Password = "Password";
        public const string RememberMe = "Remember me?";
      }

      public class Team {
        public const string Id = "Id";
        public const string Name = "Name";
      }

      public class IndexViewModel {
        //Below part is exact duplicate of the IdentityModels.ApplicationUser
        public const string HasPassword = "Has Password";
        public const string FullName = "Full Name";
        public const string DisplayName = "Display Name";
        public const string Team = "Team";
        public const string WorkingRole = "Working Role";
        public const string AdminRole = "Admin Role";
        public const string RegistrationDate = "Registration Date";
        public const string LastLogin = "Last Login"; //To check when was the last login of the user
      }


      public class SetPasswordViewModel {
        public const string NewPassword_ErrorMessage = "The {0} must be at least {2} characters long.";
        public const string ConfirmPassword_ErrorMessage = "The new password and confirmation password do not match.";
        public const string NewPassword = "New password";
        public const string ConfirmPassword = "Confirm new password";
      }

      public class ChangePasswordViewModel {
        public const string NewPassword_ErrorMessage = "The {0} must be at least {2} characters long.";
        public const string ConfirmPassword_ErrorMessage = "The new password and confirmation password do not match.";
        public const string OldPassword = "Current password";
        public const string NewPassword = "New password";
        public const string ConfirmPassword = "Confirm new password";
      }

      public class ChangeDisplayNameViewModel {
        public const string OldDisplayName = "Current display name";
        public const string NewDisplayName = "New display name";
      }

      public class ApplicationUserCreateViewModel {
        public const string FullName = "Full Name *";
        public const string DisplayName = "Display Name *";
        public const string Email = "Email *";
        public const string Team = "Team *";
        public const string Password_ErrorMessage = "The {0} must be at least {2} characters long.";
        public const string Password = "Password *";
        public const string ConfirmPassword_ErrorMessage = "The password and confirmation password do not match.";
        public const string ConfirmPassword = "Confirm password";
        public const string WorkingRole = "Working Role **";
        public const string AdminRole = "Admin Role **";
        public const string Id = "Id";
      }

      public class ApplicationUserEditViewModel {
        public const string FullName = "Full Name *";
        public const string DisplayName = "Display Name *";
        public const string Email = "Email *";
        public const string Team = "Team *";
        public const string WorkingRole = "Working Role **";
        public const string AdminRole = "Admin Role **";
        public const string Id = "Id";
      }

      public class ApplicationUserViewModel {
        public const string FullName = "Full Name";
        public const string DisplayName = "Display Name";
        public const string Email = "Email";
        public const string Team = "Team";
        public const string WorkingRole = "Working Role";
        public const string AdminRole = "Admin Role";
        public const string RegistrationDate = "Registration Date";
        public const string LastLogin = "Last Login"; //To check when was the last login of the user
        public const string Id = "Id";
      }

      public class RoleViewModel {
        public const string Id = "Id";
        public const string Name = "Name";
      }
    }
  }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using System;

namespace Aiwe.Models {
  public class IndexViewModel {
    public bool HasPassword { get; set; }
    public IList<UserLoginInfo> Logins { get; set; }
    public string PhoneNumber { get; set; }
    public bool TwoFactor { get; set; }
    public bool BrowserRemembered { get; set; }

    //Below part is exact duplicate of the IdentityModels.ApplicationUser
    public string FullName { get; set; }
    public string DisplayName { get; set; }
    public string WorkingRole { get; set; }
    public string AdminRole { get; set; }
    public string Team { get; set; }
    public DateTime RegistrationDate { get; set; } = new DateTime(1999, 12, 31, 23, 59, 59); //when the user is registered, then it has its registration date
    public DateTime? LastLogin { get; set; } = new DateTime(1999, 12, 30, 23, 59, 59); //To check when was the last login of the user

  }

  public class SetPasswordViewModel {
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
  }

  public class ChangePasswordViewModel {
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string OldPassword { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
  }

  public class ChangeDisplayNameViewModel {
    [Required]
    [Display(Name = "Current display name")]
    public string OldDisplayName { get; set; }

    [Required]
    [Display(Name = "New display name")]
    public string NewDisplayName { get; set; }
  }
}
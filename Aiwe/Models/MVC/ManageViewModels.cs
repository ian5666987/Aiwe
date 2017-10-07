using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using System;

namespace Aiwe.Models {
  public class IndexViewModel {
    [Display(Name = Aiwe.LCZ.F.IndexViewModel.HasPassword)]
    public bool HasPassword { get; set; }
    public IList<UserLoginInfo> Logins { get; set; }
    public string PhoneNumber { get; set; }
    public bool TwoFactor { get; set; }
    public bool BrowserRemembered { get; set; }

    //Below part is exact duplicate of the IdentityModels.ApplicationUser
    [Display(Name = Aiwe.LCZ.F.IndexViewModel.FullName)]
    public string FullName { get; set; }
    [Display(Name = Aiwe.LCZ.F.IndexViewModel.DisplayName)]
    public string DisplayName { get; set; }
    [Display(Name = Aiwe.LCZ.F.IndexViewModel.WorkingRole)]
    public string WorkingRole { get; set; }
    [Display(Name = Aiwe.LCZ.F.IndexViewModel.AdminRole)]
    public string AdminRole { get; set; }
    [Display(Name = Aiwe.LCZ.F.IndexViewModel.Team)]
    public string Team { get; set; }
    [Display(Name = Aiwe.LCZ.F.IndexViewModel.RegistrationDate)]
    public DateTime RegistrationDate { get; set; } = new DateTime(1999, 12, 31, 23, 59, 59); //when the user is registered, then it has its registration date
    [Display(Name = Aiwe.LCZ.F.IndexViewModel.LastLogin)]
    public DateTime? LastLogin { get; set; } = new DateTime(1999, 12, 30, 23, 59, 59); //To check when was the last login of the user

  }

  public class SetPasswordViewModel {
    [Required]
    [StringLength(100, ErrorMessage = Aiwe.LCZ.F.SetPasswordViewModel.NewPassword_ErrorMessage, MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = Aiwe.LCZ.F.SetPasswordViewModel.NewPassword)]
    public string NewPassword { get; set; }

    //"NewPassword" is ok to be hardcoded since it is compared to the item above whose field's name is "NewPassword"
    [DataType(DataType.Password)]
    [Display(Name = Aiwe.LCZ.F.SetPasswordViewModel.ConfirmPassword)]
    [Compare("NewPassword", ErrorMessage = Aiwe.LCZ.F.SetPasswordViewModel.ConfirmPassword_ErrorMessage)]
    public string ConfirmPassword { get; set; }
  }

  public class ChangePasswordViewModel {
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = Aiwe.LCZ.F.ChangePasswordViewModel.OldPassword)]
    public string OldPassword { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = Aiwe.LCZ.F.ChangePasswordViewModel.NewPassword_ErrorMessage, MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = Aiwe.LCZ.F.ChangePasswordViewModel.NewPassword)]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = Aiwe.LCZ.F.ChangePasswordViewModel.ConfirmPassword)]
    [Compare("NewPassword", ErrorMessage = Aiwe.LCZ.F.ChangePasswordViewModel.ConfirmPassword_ErrorMessage)]
    public string ConfirmPassword { get; set; }
  }

  public class ChangeDisplayNameViewModel {
    [Required]
    [Display(Name = Aiwe.LCZ.F.ChangeDisplayNameViewModel.OldDisplayName)]
    public string OldDisplayName { get; set; }

    [Required]
    [Display(Name = Aiwe.LCZ.F.ChangeDisplayNameViewModel.NewDisplayName)]
    public string NewDisplayName { get; set; }
  }
}
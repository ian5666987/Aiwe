using System.ComponentModel.DataAnnotations;

namespace Aiwe.Models.ViewModels {
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class ApplicationUserCreateViewModel {
    [Required]
    [Display(Name = Aiwe.LCZ.F.ApplicationUserCreateViewModel.FullName)]
    public string FullName { get; set; }

    [Required]
    [Display(Name = Aiwe.LCZ.F.ApplicationUserCreateViewModel.DisplayName)]
    public string DisplayName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = Aiwe.LCZ.F.ApplicationUserCreateViewModel.Email)]
    public string Email { get; set; }

    [Required]
    [Display(Name = Aiwe.LCZ.F.ApplicationUserCreateViewModel.Team)]
    public string Team { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = Aiwe.LCZ.F.ApplicationUserCreateViewModel.Password_ErrorMessage, MinimumLength = 4)]
    [DataType(DataType.Password)]
    [Display(Name = Aiwe.LCZ.F.ApplicationUserCreateViewModel.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = Aiwe.LCZ.F.ApplicationUserCreateViewModel.ConfirmPassword)]
    [Compare("Password", ErrorMessage = Aiwe.LCZ.F.ApplicationUserCreateViewModel.ConfirmPassword_ErrorMessage)]
    public string ConfirmPassword { get; set; }

    [Display(Name = Aiwe.LCZ.F.ApplicationUserCreateViewModel.WorkingRole)]
    public string WorkingRole { get; set; }

    [Display(Name = Aiwe.LCZ.F.ApplicationUserCreateViewModel.AdminRole)]
    public string AdminRole { get; set; }

    [Display(Name = Aiwe.LCZ.F.ApplicationUserCreateViewModel.Id)]
    public string Id { get; set; } //remains hidden, not shown only used in edit

    public ApplicationUserCreateViewModel() { }

    public ApplicationUserCreateViewModel(ApplicationUser appUser) {
      FullName = appUser.FullName;
      DisplayName = appUser.DisplayName;
      Email = appUser.Email;
      Team = appUser.Team;
      WorkingRole = appUser.WorkingRole;
      AdminRole = appUser.AdminRole;
      Id = appUser.Id;
    }
  }
}
using System.ComponentModel.DataAnnotations;

namespace Aiwe.Models.ViewModels {
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class ApplicationUserEditViewModel {
    [Required]
    [Display(Name = Aiwe.LCZ.F.ApplicationUserEditViewModel.FullName)]
    public string FullName { get; set; }

    [Required]
    [Display(Name = Aiwe.LCZ.F.ApplicationUserEditViewModel.DisplayName)]
    public string DisplayName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = Aiwe.LCZ.F.ApplicationUserEditViewModel.Email)]
    public string Email { get; set; }

    [Required]
    [Display(Name = Aiwe.LCZ.F.ApplicationUserEditViewModel.Team)]
    public string Team { get; set; }

    [Display(Name = Aiwe.LCZ.F.ApplicationUserEditViewModel.WorkingRole)]
    public string WorkingRole { get; set; }

    [Display(Name = Aiwe.LCZ.F.ApplicationUserEditViewModel.AdminRole)]
    public string AdminRole { get; set; }

    [Display(Name = Aiwe.LCZ.F.ApplicationUserEditViewModel.Id)]
    public string Id { get; set; }

    public ApplicationUserEditViewModel() { }

    public ApplicationUserEditViewModel(ApplicationUser appUser) {
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
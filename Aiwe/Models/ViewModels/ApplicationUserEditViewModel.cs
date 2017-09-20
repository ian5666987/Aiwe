using System.ComponentModel.DataAnnotations;

namespace Aiwe.Models.ViewModels {
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class ApplicationUserEditViewModel {
    [Required]
    [Display(Name = "Full Name *")]
    public string FullName { get; set; }

    [Required]
    [Display(Name = "Display Name *")]
    public string DisplayName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email *")]
    public string Email { get; set; }

    [Required]
    [Display(Name = "Team *")]
    public string Team { get; set; }

    [Display(Name = "Working Role **")]
    public string WorkingRole { get; set; }

    [Display(Name = "Admin Role **")]
    public string AdminRole { get; set; }

    public string Id { get; set; } //remains hidden, not shown only used in edit

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
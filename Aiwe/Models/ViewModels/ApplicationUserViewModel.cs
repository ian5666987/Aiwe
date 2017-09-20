using System;
using System.ComponentModel;

namespace Aiwe.Models.ViewModels {
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class ApplicationUserViewModel {
    public static DateTime DefaultDateTime = new DateTime(1999, 12, 31, 23, 59, 59);

    [DisplayName("Full Name")]
    public string FullName { get; set; }

    [DisplayName("Display Name")]
    public string DisplayName { get; set; }

    [DisplayName("Email")]
    public string Email { get; set; }

    [DisplayName("Team")]
    public string Team { get; set; }

    [DisplayName("Working Role")]
    public string WorkingRole { get; set; }

    [DisplayName("Admin Role")]
    public string AdminRole { get; set; }

    [DisplayName("Registration Date")]
    public DateTime RegistrationDate { get; set; } = DefaultDateTime; //when the user is registered, then it has its registration date

    [DisplayName("Last Login")]
    public DateTime LastLogin { get; set; } = DefaultDateTime; //To check when was the last login of the user
    public string Id { get; set; } //hidden but needed to do the actions

    public ApplicationUserViewModel(ApplicationUser appUser) {
      FullName = appUser.FullName;
      DisplayName = appUser.DisplayName;
      Email = appUser.Email;
      Team = appUser.Team;
      WorkingRole = appUser.WorkingRole;
      AdminRole = appUser.AdminRole;
      RegistrationDate = appUser.RegistrationDate;
      LastLogin = appUser.LastLogin.HasValue ? appUser.LastLogin.Value : DefaultDateTime;
      Id = appUser.Id;
    }
  }
}
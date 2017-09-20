using System.ComponentModel.DataAnnotations;
using Aibe.Models.Filters;

namespace Aiwe.Models.Filters {
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class ApplicationUserFilter : BaseFilter {
    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    [Display(Name = "Display Name")]
    public string DisplayName { get; set; }

    [Display(Name = "Email")]
    public string Email { get; set; }

    [Display(Name = "Team")]
    public string Team { get; set; }

    [Display(Name = "Working Role")]
    public string WorkingRole { get; set; }

    [Display(Name = "Admin Role")]
    public string AdminRole { get; set; }
  }
}
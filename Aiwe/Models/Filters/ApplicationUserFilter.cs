using System.ComponentModel.DataAnnotations;
using Aibe.Models.Filters;

namespace Aiwe.Models.Filters {
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class ApplicationUserFilter : BaseFilter {
    [Display(Name = Aiwe.LCZ.F.ApplicationUserFilter.UserName)]
    public string UserName { get; set; }

    [Display(Name = Aiwe.LCZ.F.ApplicationUserFilter.FullName)]
    public string FullName { get; set; }

    [Display(Name = Aiwe.LCZ.F.ApplicationUserFilter.DisplayName)]
    public string DisplayName { get; set; }

    [Display(Name = Aiwe.LCZ.F.ApplicationUserFilter.Email)]
    public string Email { get; set; }

    [Display(Name = Aiwe.LCZ.F.ApplicationUserFilter.Team)]
    public string Team { get; set; }

    [Display(Name = Aiwe.LCZ.F.ApplicationUserFilter.WorkingRole)]
    public string WorkingRole { get; set; }

    [Display(Name = Aiwe.LCZ.F.ApplicationUserFilter.AdminRole)]
    public string AdminRole { get; set; }
  }
}
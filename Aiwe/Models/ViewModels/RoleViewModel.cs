using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Aiwe.Models.ViewModels {
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class RoleViewModel {
    [DisplayName(Aiwe.LCZ.F.RoleViewModel.Id)]
    public string Id { get; set; }
    [Required]
    [DisplayName(Aiwe.LCZ.F.RoleViewModel.Name)]
    public string Name { get; set; }

    public RoleViewModel() { }
    public RoleViewModel(IdentityRole role) {
      Id = role.Id;
      Name = role.Name;
    }
  }
}
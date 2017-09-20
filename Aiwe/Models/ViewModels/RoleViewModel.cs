using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace Aiwe.Models.ViewModels {
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class RoleViewModel {
    public string Id { get; set; }
    [Required]
    public string Name { get; set; }

    public RoleViewModel() { }
    public RoleViewModel(IdentityRole role) {
      Id = role.Id;
      Name = role.Name;
    }
  }
}
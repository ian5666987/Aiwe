using Aibe.Models.Filters;

namespace Aiwe.Models.Filters {
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class TeamFilter : BaseFilter {
    public string Name { get; set; }
  }
}
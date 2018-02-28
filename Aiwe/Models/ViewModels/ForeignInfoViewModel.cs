using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Aiwe.Models.ViewModels {
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class ForeignInfoViewModel {
    public string ColumnName { get; set; }
    public string Value { get; set; }
    public string PageName { get; set; }
    public string DefaultDateTimeFormat { get; set; }
    public AiweBaseTableModel BaseModel { get; set; }

    //Web elements only
    public string LabelClasses { get; set; }
    public string EditorClasses { get; set; }

    public ForeignInfoViewModel() { }
  }
}
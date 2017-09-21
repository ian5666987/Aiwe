namespace Aiwe.Models.DB {
  using System.ComponentModel.DataAnnotations;

  public partial class CFG_TEM_DRP {
    [Key]
    public int Cid { get; set; }

    [StringLength(100)]
    public string TemplateName { get; set; }

    [StringLength(2000)]
    public string TemplateValue { get; set; }
  }
}

namespace Aiwe.Models.DB {
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("TestTableCommonH")]
  public partial class TestTableCommonH {
    [Key]
    public int Cid { get; set; }

    [StringLength(100)]
    public string TemplateName { get; set; }

    [StringLength(2000)]
    public string TemplateDefaultValue { get; set; }

    [StringLength(2000)]
    public string TemplateCheckValue { get; set; }
  }
}

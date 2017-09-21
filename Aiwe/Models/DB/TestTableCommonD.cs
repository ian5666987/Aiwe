namespace Aiwe.Models.DB {
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("TestTableCommonD")]
  public partial class TestTableCommonD {
    [Key]
    public int Cid { get; set; }

    [Required]
    [StringLength(100)]
    public string SimpleName { get; set; }

    public bool? SimpleBoolean { get; set; }

    [StringLength(1000)]
    public string Picture { get; set; }

    [StringLength(1000)]
    public string AnotherPicture { get; set; }
  }
}

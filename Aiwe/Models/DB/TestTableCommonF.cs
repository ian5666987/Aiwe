namespace Aiwe.Models.DB {
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("TestTableCommonF")]
  public partial class TestTableCommonF {
    [Key]
    public int Cid { get; set; }

    [StringLength(100)]
    public string SimpleString { get; set; }

    public int? IncrementId { get; set; }
  }
}

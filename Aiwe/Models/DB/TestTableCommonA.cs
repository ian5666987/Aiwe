namespace Aiwe.Models.DB {
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("TestTableCommonA")]
  public partial class TestTableCommonA {
    [Key]
    public int Cid { get; set; }

    [StringLength(100)]
    public string OfficerName { get; set; }

    [StringLength(100)]
    public string Email { get; set; }

    [StringLength(100)]
    public string AddressPath { get; set; }

    [StringLength(300)]
    public string Remarks { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime? StartTime { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime? EndTime { get; set; }
  }
}

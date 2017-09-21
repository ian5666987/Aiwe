namespace Aiwe.Models.DB {
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("TestTableCommonC")]
  public partial class TestTableCommonC {
    [Key]
    public int Cid { get; set; }

    [StringLength(100)]
    public string ServiceType { get; set; }

    [StringLength(100)]
    public string ServiceName { get; set; }

    [StringLength(100)]
    public string PersonInCharge { get; set; }

    [StringLength(100)]
    public string PhoneNumber { get; set; }

    [StringLength(100)]
    public string ServiceStatus { get; set; }
  }
}

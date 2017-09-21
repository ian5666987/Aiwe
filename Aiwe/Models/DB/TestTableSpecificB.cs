namespace Aiwe.Models.DB {
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("TestTableSpecificB")]
  public partial class TestTableSpecificB {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int VendorId { get; set; }

    [StringLength(100)]
    public string VendorName { get; set; }

    [StringLength(100)]
    public string AddressLocation { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime? ContractStartOn { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime? ContractEndOn { get; set; }
  }
}

namespace Aiwe.Models.DB {
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("VServiceInfo")]
  public partial class VServiceInfo {
    [Key]
    [Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Cid { get; set; }

    public int? RequestNo { get; set; }

    [StringLength(100)]
    public string RequestPerson { get; set; }

    public DateTime? RequestDate { get; set; }

    [Key]
    [Column(Order = 1)]
    [StringLength(50)]
    public string CustomerId { get; set; }

    [StringLength(20)]
    public string CustomerSiteId { get; set; }

    [StringLength(100)]
    public string DeviceName { get; set; }

    [StringLength(100)]
    public string ProductCode { get; set; }

    [Key]
    [Column(Order = 2)]
    [StringLength(100)]
    public string ProductTypeId { get; set; }

    [Key]
    [Column(Order = 3)]
    [StringLength(100)]
    public string CustomerName { get; set; }

    [StringLength(50)]
    public string TeamId { get; set; }

    [StringLength(300)]
    public string ServicePeople { get; set; }

    public DateTime? ScheduleDate { get; set; }

    public DateTime? ServiceStart { get; set; }

    public DateTime? ServiceEnd { get; set; }

    public DateTime? ServiceClose { get; set; }
  }
}

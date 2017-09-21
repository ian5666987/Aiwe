namespace Aiwe.Models.DB {
  using System;
  using System.ComponentModel.DataAnnotations;

  public partial class ORD_CUS_REQ {
    [Key]
    public int Cid { get; set; }

    public int? RequestNo { get; set; }

    [StringLength(100)]
    public string RequestPerson { get; set; }

    public DateTime? RequestDate { get; set; }

    [Required]
    [StringLength(50)]
    public string CustomerId { get; set; }

    [StringLength(20)]
    public string CustomerSiteId { get; set; }

    [StringLength(100)]
    public string SubconId { get; set; }

    [StringLength(600)]
    public string ContactList { get; set; }

    [StringLength(100)]
    public string DeviceName { get; set; }

    [StringLength(100)]
    public string ProductCode { get; set; }

    [StringLength(500)]
    public string ProductSeriesNo { get; set; }

    [StringLength(500)]
    public string ProductFault { get; set; }

    [StringLength(10)]
    public string RequestStatus { get; set; }
  }
}

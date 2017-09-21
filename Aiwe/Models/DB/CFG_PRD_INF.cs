namespace Aiwe.Models.DB {
  using System.ComponentModel.DataAnnotations;

  public partial class CFG_PRD_INF {
    [Key]
    public int Cid { get; set; }

    [Required]
    [StringLength(100)]
    public string ProductTypeId { get; set; }

    [Required]
    [StringLength(100)]
    public string DeviceName { get; set; }

    [StringLength(100)]
    public string ProductCode { get; set; }

    [StringLength(500)]
    public string ProductSeriesNo { get; set; }
  }
}

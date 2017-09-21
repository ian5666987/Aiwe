namespace Aiwe.Models.DB {
  using System;
  using System.ComponentModel.DataAnnotations;

  public partial class CFG_TRN_ACT {
    [Key]
    public int Cid { get; set; }

    [Required]
    [StringLength(50)]
    public string Category { get; set; }

    [Required]
    [StringLength(150)]
    public string Topic { get; set; }

    public DateTime? LastUpdate { get; set; }
  }
}

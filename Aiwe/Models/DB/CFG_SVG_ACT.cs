namespace Aiwe.Models.DB {
  using System.ComponentModel.DataAnnotations;

  public partial class CFG_SVG_ACT {
    [Key]
    public int Cid { get; set; }

    [Required]
    [StringLength(50)]
    public string ActionName { get; set; }

    [StringLength(1000)]
    public string SPList { get; set; }
  }
}

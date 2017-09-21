namespace Aiwe.Models.DB {
  using System.ComponentModel.DataAnnotations;

  public partial class CFG_SVG_TYP {
    [Key]
    public int Cid { get; set; }

    [Required]
    [StringLength(30)]
    public string ServiceTypeId { get; set; }

    [StringLength(100)]
    public string TypeDescription { get; set; }
  }
}

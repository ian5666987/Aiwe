namespace Aiwe.Models.DB {
  using System.ComponentModel.DataAnnotations;

  public partial class CFG_PRD_TYP {
    [Key]
    public int Cid { get; set; }

    [Required]
    [StringLength(50)]
    public string ProductTypeId { get; set; }

    [StringLength(200)]
    public string TypeDescription { get; set; }
  }
}

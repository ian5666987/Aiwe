namespace Aiwe.Models.DB {
  using System.ComponentModel.DataAnnotations;

  public partial class CFG_CUS_SUB {
    [Key]
    public int Cid { get; set; }

    [Required]
    [StringLength(50)]
    public string CustomerId { get; set; }

    [Required]
    [StringLength(100)]
    public string SubconId { get; set; }
  }
}

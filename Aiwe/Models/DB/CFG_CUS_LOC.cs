namespace Aiwe.Models.DB {
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  public partial class CFG_CUS_LOC {
    [Key]
    [Column(Order = 0)]
    public int Cid { get; set; }

    [Key]
    [Column(Order = 1)]
    [StringLength(3)]
    public string CustomerSiteId { get; set; }

    [StringLength(100)]
    public string SiteDescription { get; set; }

    [Key]
    [Column(Order = 2)]
    [StringLength(100)]
    public string Country { get; set; }
  }
}

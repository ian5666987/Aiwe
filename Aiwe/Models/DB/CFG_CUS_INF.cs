namespace Aiwe.Models.DB {
  using System.ComponentModel.DataAnnotations;

  public partial class CFG_CUS_INF {
    [Key]
    public int Cid { get; set; }

    [Required]
    [StringLength(50)]
    public string CustomerId { get; set; }

    [Required]
    [StringLength(3)]
    public string CustomerSiteId { get; set; }

    [StringLength(100)]
    public string CustomerName { get; set; }

    [StringLength(200)]
    public string CompanyAddress { get; set; }

    [StringLength(600)]
    public string ContactList { get; set; }
  }
}

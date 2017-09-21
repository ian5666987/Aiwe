namespace Aiwe.Models.DB {
  using System.ComponentModel.DataAnnotations;

  public partial class CFG_JOB_STS {
    [Key]
    public int Cid { get; set; }

    [Required]
    [StringLength(3)]
    public string StatusId { get; set; }

    [Required]
    [StringLength(20)]
    public string JobStatus { get; set; }
  }
}

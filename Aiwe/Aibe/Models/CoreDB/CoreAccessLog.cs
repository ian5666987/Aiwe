namespace Aibe.Models.DB {
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table(DH.AccessLogTableName)]
  public partial class CoreAccessLog {
    [Key]
    public int Cid { get; set; }

    [Required]
    [StringLength(300)]
    public string UserName { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime LogTimeStamp { get; set; }

    [Required]
    [StringLength(100)]
    public string LogType { get; set; }

    [StringLength(100)]
    public string Result { get; set; }

    [StringLength(3000)]
    public string LogMessage { get; set; }
  }
}

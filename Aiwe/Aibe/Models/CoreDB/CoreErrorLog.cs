namespace Aibe.Models.DB {
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table(DH.ErrorLogTableName)]
  public partial class CoreErrorLog {
    [Key]
    public int Cid { get; set; }

    [StringLength(300)]
    public string UserName { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime LogTimeStamp { get; set; }

    [StringLength(100)]
    public string ErrorCode { get; set; }

    [StringLength(100)]
    public string ControllerType { get; set; }

    [StringLength(100)]
    public string ControllerName { get; set; }

    [StringLength(100)]
    public string TableSource { get; set; }

    [StringLength(100)]
    public string UserAction { get; set; }

    [StringLength(3000)]
    public string LogMessage { get; set; }

    public string ErrorMessage { get; set; }
  }
}

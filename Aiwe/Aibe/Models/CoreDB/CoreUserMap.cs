namespace Aibe.Models.DB {
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table(DH.UserMapTableName)]
  public partial class CoreUserMap {
    [Key]
    public int Cid { get; set; }

    [StringLength(500)]
    public string UserName { get; set; }

    [StringLength(2000)]
    public string EncryptedPassword { get; set; }
  }
}

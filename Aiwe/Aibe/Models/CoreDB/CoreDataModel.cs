namespace Aibe.Models.DB {
  using System.Data.Entity;

  public partial class CoreDataModel : DbContext {
    public CoreDataModel()
        : base("name=" + DH.DataDBConnectionStringName) {
    }

    public virtual DbSet<CoreAccessLog> CoreAccessLogs { get; set; }
    public virtual DbSet<CoreActionLog> CoreActionLogs { get; set; }
    public virtual DbSet<CoreErrorLog> CoreErrorLogs { get; set; }
    public virtual DbSet<CoreUserMap> CoreUserMaps { get; set; }
    public virtual DbSet<MetaItem> MetaItems { get; set; }
  }
}

namespace Aiwe.Models.DB {
  using System;
  using System.Data.Entity;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Linq;

  public partial class CoreDataModel : DbContext {
    public CoreDataModel()
        : base("name=CoreDataModel") {
    }

    public virtual DbSet<CoreAccessLog> CoreAccessLogs { get; set; }
    public virtual DbSet<CoreActionLog> CoreActionLogs { get; set; }
    public virtual DbSet<CoreEmailInfo> CoreEmailInfoes { get; set; }
    public virtual DbSet<CoreEmailTemplate> CoreEmailTemplates { get; set; }
    public virtual DbSet<CoreErrorLog> CoreErrorLogs { get; set; }
    public virtual DbSet<CoreUserMap> CoreUserMaps { get; set; }
    public virtual DbSet<MetaItem> MetaItems { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder) {
    }
  }
}

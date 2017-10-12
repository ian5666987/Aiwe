namespace Aiwe.Models.DB {
  using System;
  using System.Data.Entity;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Linq;

  public partial class CoreDataModel : DbContext {
    public CoreDataModel()
        : base("name=CoreDataModel") {
    }

    public virtual DbSet<CFG_CUS_INF> CFG_CUS_INF { get; set; }
    public virtual DbSet<CFG_CUS_SUB> CFG_CUS_SUB { get; set; }
    public virtual DbSet<CFG_JOB_INT> CFG_JOB_INT { get; set; }
    public virtual DbSet<CFG_JOB_STS> CFG_JOB_STS { get; set; }
    public virtual DbSet<CFG_PRD_INF> CFG_PRD_INF { get; set; }
    public virtual DbSet<CFG_PRD_TYP> CFG_PRD_TYP { get; set; }
    public virtual DbSet<CFG_PUB_HOL> CFG_PUB_HOL { get; set; }
    public virtual DbSet<CFG_SVG_ACT> CFG_SVG_ACT { get; set; }
    public virtual DbSet<CFG_SVG_TYP> CFG_SVG_TYP { get; set; }
    public virtual DbSet<CFG_TEM_DRP> CFG_TEM_DRP { get; set; }
    public virtual DbSet<CFG_TEM_LST> CFG_TEM_LST { get; set; }
    public virtual DbSet<CFG_TRN_ACT> CFG_TRN_ACT { get; set; }
    public virtual DbSet<CoreAccessLog> CoreAccessLogs { get; set; }
    public virtual DbSet<CoreActionLog> CoreActionLogs { get; set; }
    public virtual DbSet<CoreErrorLog> CoreErrorLogs { get; set; }
    public virtual DbSet<CoreUserMap> CoreUserMaps { get; set; }
    public virtual DbSet<EML_EMAILINFO> EML_EMAILINFO { get; set; }
    public virtual DbSet<EML_TEMPLATE> EML_TEMPLATE { get; set; }
    public virtual DbSet<JOB_CAL_INF> JOB_CAL_INF { get; set; }
    public virtual DbSet<JOB_ORD_INF> JOB_ORD_INF { get; set; }
    public virtual DbSet<JOB_TRN_INF> JOB_TRN_INF { get; set; }
    public virtual DbSet<LOG_JOB_HST> LOG_JOB_HST { get; set; }
    public virtual DbSet<LOG_TRN_HST> LOG_TRN_HST { get; set; }
    public virtual DbSet<LOG_USR_ACT> LOG_USR_ACT { get; set; }
    public virtual DbSet<MetaItem> MetaItems { get; set; }
    public virtual DbSet<ORD_CUS_CON> ORD_CUS_CON { get; set; }
    public virtual DbSet<ORD_CUS_PRD> ORD_CUS_PRD { get; set; }
    public virtual DbSet<ORD_CUS_REQ> ORD_CUS_REQ { get; set; }
    public virtual DbSet<TestTableCommonA> TestTableCommonAs { get; set; }
    public virtual DbSet<TestTableCommonB> TestTableCommonBs { get; set; }
    public virtual DbSet<TestTableCommonC> TestTableCommonCs { get; set; }
    public virtual DbSet<TestTableCommonD> TestTableCommonDs { get; set; }
    public virtual DbSet<TestTableCommonE> TestTableCommonEs { get; set; }
    public virtual DbSet<TestTableCommonF> TestTableCommonFs { get; set; }
    public virtual DbSet<TestTableCommonG> TestTableCommonGs { get; set; }
    public virtual DbSet<TestTableCommonH> TestTableCommonHs { get; set; }
    public virtual DbSet<TestTableSpecificA> TestTableSpecificAs { get; set; }
    public virtual DbSet<TestTableSpecificB> TestTableSpecificBs { get; set; }
    public virtual DbSet<CFG_CUS_LOC> CFG_CUS_LOC { get; set; }
    public virtual DbSet<VServiceInfo> VServiceInfoes { get; set; }
    public virtual DbSet<VServiceLog> VServiceLogs { get; set; }
    public virtual DbSet<VServiceTeam> VServiceTeams { get; set; }
    public virtual DbSet<VServiceTest> VServiceTests { get; set; }
    public virtual DbSet<VTrainingLog> VTrainingLogs { get; set; }
    public virtual DbSet<VTrainingTest> VTrainingTests { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder) {
      modelBuilder.Entity<LOG_JOB_HST>()
          .Property(e => e.MailSent)
          .IsFixedLength();

      modelBuilder.Entity<LOG_TRN_HST>()
          .Property(e => e.MailSent)
          .IsFixedLength();

      modelBuilder.Entity<TestTableCommonE>()
          .Property(e => e.NullableDecimal)
          .HasPrecision(18, 7);

      modelBuilder.Entity<TestTableCommonE>()
          .Property(e => e.NullableChar)
          .IsFixedLength()
          .IsUnicode(false);

      modelBuilder.Entity<TestTableCommonE>()
          .Property(e => e.DecimalType)
          .HasPrecision(18, 7);

      modelBuilder.Entity<VServiceLog>()
          .Property(e => e.MailSent)
          .IsFixedLength();

      modelBuilder.Entity<VServiceLog>()
          .Property(e => e.ContactName)
          .IsUnicode(false);

      modelBuilder.Entity<VServiceLog>()
          .Property(e => e.ContactNo)
          .IsUnicode(false);

      modelBuilder.Entity<VServiceLog>()
          .Property(e => e.ConnectorNo)
          .IsUnicode(false);

      modelBuilder.Entity<VServiceLog>()
          .Property(e => e.HeadNo)
          .IsUnicode(false);

      modelBuilder.Entity<VServiceLog>()
          .Property(e => e.CardNo)
          .IsUnicode(false);

      modelBuilder.Entity<VServiceLog>()
          .Property(e => e.CheckList2)
          .IsUnicode(false);

      modelBuilder.Entity<VServiceTest>()
          .Property(e => e.ContactName)
          .IsUnicode(false);

      modelBuilder.Entity<VServiceTest>()
          .Property(e => e.ContactNo)
          .IsUnicode(false);

      modelBuilder.Entity<VServiceTest>()
          .Property(e => e.ConnectorNo)
          .IsUnicode(false);

      modelBuilder.Entity<VServiceTest>()
          .Property(e => e.HeadNo)
          .IsUnicode(false);

      modelBuilder.Entity<VServiceTest>()
          .Property(e => e.CardNo)
          .IsUnicode(false);

      modelBuilder.Entity<VServiceTest>()
          .Property(e => e.CheckList2)
          .IsUnicode(false);

      modelBuilder.Entity<VTrainingLog>()
          .Property(e => e.MailSent)
          .IsFixedLength();

      modelBuilder.Entity<VTrainingLog>()
          .Property(e => e.ContactName)
          .IsUnicode(false);

      modelBuilder.Entity<VTrainingLog>()
          .Property(e => e.ContactNo)
          .IsUnicode(false);

      modelBuilder.Entity<VTrainingTest>()
          .Property(e => e.ContactName)
          .IsUnicode(false);

      modelBuilder.Entity<VTrainingTest>()
          .Property(e => e.ContactNo)
          .IsUnicode(false);
    }
  }
}

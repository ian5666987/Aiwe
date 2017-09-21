namespace Aiwe.Models.DB {
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("VTrainingTest")]
  public partial class VTrainingTest {
    [Key]
    [Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Cid { get; set; }

    public int? TrainingNo { get; set; }

    [Key]
    [Column(Order = 1)]
    [StringLength(100)]
    public string CustomerName { get; set; }

    [Key]
    [Column(Order = 2)]
    [StringLength(30)]
    public string TrnCategory { get; set; }

    [Key]
    [Column(Order = 3)]
    [StringLength(100)]
    public string TrnTopic { get; set; }

    [Key]
    [Column(Order = 4)]
    public DateTime TrnScheduleTime { get; set; }

    [StringLength(500)]
    public string TrnScheduleHist { get; set; }

    public DateTime? TrnDateTime { get; set; }

    public int? DaysTaken { get; set; }

    [StringLength(50)]
    public string CustomerId { get; set; }

    [StringLength(20)]
    public string CustomerSiteId { get; set; }

    [StringLength(50)]
    public string CompanyDept { get; set; }

    [StringLength(1000)]
    public string AttendeeList { get; set; }

    [StringLength(600)]
    public string ContactList { get; set; }

    [StringLength(100)]
    public string TrainerId { get; set; }

    [StringLength(2000)]
    public string Remarks { get; set; }

    [StringLength(500)]
    public string CustomerSignature { get; set; }

    [StringLength(500)]
    public string TrainerSignature { get; set; }

    [StringLength(100)]
    public string CustomerSignatureName { get; set; }

    [StringLength(100)]
    public string TrainerSignatureName { get; set; }

    public DateTime? TrainingStart { get; set; }

    public DateTime? TrainingEnd { get; set; }

    [StringLength(10)]
    public string TrainingStatus { get; set; }

    [StringLength(200)]
    public string CompanyAddress { get; set; }

    [StringLength(200)]
    public string ContactName { get; set; }

    [StringLength(200)]
    public string ContactNo { get; set; }
  }
}

namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VServiceTest")]
    public partial class VServiceTest
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Cid { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestNo { get; set; }

        [StringLength(50)]
        public string CustomerId { get; set; }

        [StringLength(3)]
        public string CustomerSiteId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string CustomerName { get; set; }

        [StringLength(600)]
        public string ContactList { get; set; }

        [StringLength(50)]
        public string CompanyDept { get; set; }

        [StringLength(50)]
        public string TeamId { get; set; }

        [StringLength(300)]
        public string ServicePeople { get; set; }

        public DateTime? ScheduleDate { get; set; }

        [StringLength(10)]
        public string ServiceStatus { get; set; }

        [StringLength(100)]
        public string DeviceName { get; set; }

        [StringLength(100)]
        public string ProductCode { get; set; }

        [StringLength(500)]
        public string ProductSeriesNo { get; set; }

        [StringLength(200)]
        public string SPTemplate { get; set; }

        [StringLength(2000)]
        public string ServicePerformed { get; set; }

        [StringLength(200)]
        public string CLTemplate { get; set; }

        [StringLength(2000)]
        public string CheckList { get; set; }

        [StringLength(2000)]
        public string RemarkAndAction { get; set; }

        [StringLength(500)]
        public string DeviceImage { get; set; }

        [StringLength(500)]
        public string UserSignature { get; set; }

        [StringLength(500)]
        public string EngineerSignature { get; set; }

        public DateTime? ServiceStart { get; set; }

        public DateTime? ServiceEnd { get; set; }

        [StringLength(100)]
        public string UserSignatureName { get; set; }

        [StringLength(100)]
        public string EngineerSignatureName { get; set; }

        [StringLength(1000)]
        public string JobTrack { get; set; }

        [StringLength(1000)]
        public string FollowUp { get; set; }

        [StringLength(200)]
        public string CompanyAddress { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(100)]
        public string ProductTypeId { get; set; }

        [StringLength(200)]
        public string ContactName { get; set; }

        [StringLength(200)]
        public string ContactNo { get; set; }

        [StringLength(200)]
        public string ConnectorNo { get; set; }

        [StringLength(200)]
        public string HeadNo { get; set; }

        [StringLength(200)]
        public string CardNo { get; set; }

        [StringLength(500)]
        public string ProductFault { get; set; }

        [StringLength(200)]
        public string CheckList2 { get; set; }
    }
}

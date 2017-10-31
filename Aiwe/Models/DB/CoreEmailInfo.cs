namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CoreEmailInfo")]
    public partial class CoreEmailInfo
    {
        [Key]
        public int Cid { get; set; }

        [StringLength(200)]
        public string TemplateName { get; set; }

        [StringLength(2000)]
        public string EmailFrom { get; set; }

        [StringLength(2000)]
        public string EmailTo { get; set; }

        [StringLength(500)]
        public string EmailSubject { get; set; }

        [StringLength(2000)]
        public string EmailCc { get; set; }

        [StringLength(2000)]
        public string EmailBcc { get; set; }

        [StringLength(4000)]
        public string EmailBody { get; set; }

        [StringLength(4000)]
        public string EmailParameters { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? CreatedOn { get; set; }

        public bool? IsSent { get; set; }

        public DateTime? SendDateTime { get; set; }

        public int? SendCount { get; set; }

        [StringLength(4000)]
        public string AttachmentFilePaths { get; set; }

        public int? JobNo { get; set; }

        [StringLength(10)]
        public string JobStatus { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ScheduleDateTime { get; set; }

        public int? Send1DaysBefore { get; set; }

        public int? Send3DaysBefore { get; set; }

        public int? Send7DaysBefore { get; set; }
    }
}

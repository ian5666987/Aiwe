namespace Aibe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EML_EMAILINFO
    {
        [Key]
        public int Cid { get; set; }

        [StringLength(50)]
        public string TemplateName { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? SendStatus { get; set; }

        public DateTime? SendDateTime { get; set; }

        public int? SendCount { get; set; }

        [StringLength(50)]
        public string EmailFrom { get; set; }

        [StringLength(500)]
        public string EmailTo { get; set; }

        [StringLength(500)]
        public string EmailCc { get; set; }

        [StringLength(200)]
        public string EmailSubject { get; set; }

        [StringLength(4000)]
        public string EmailBody { get; set; }

        [StringLength(200)]
        public string AttachedFilesPath { get; set; }

        public int? JobNo { get; set; }

        [StringLength(10)]
        public string JobStatus { get; set; }

        public DateTime? ScheduleDateTime { get; set; }

        [StringLength(1000)]
        public string ParamValues { get; set; }

        public int? Send1DaysBefore { get; set; }

        public int? Send3DaysBefore { get; set; }

        public int? Send7DaysBefore { get; set; }
    }
}

namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CoreEmailTemplate")]
    public partial class CoreEmailTemplate
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

        [StringLength(200)]
        public string CreatedOn { get; set; }

        public bool? IsSent { get; set; }

        [StringLength(4000)]
        public string AttachmentFilePaths { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}

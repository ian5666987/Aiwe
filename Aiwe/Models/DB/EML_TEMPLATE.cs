namespace Aibe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EML_TEMPLATE
    {
        [Key]
        public int Cid { get; set; }

        [StringLength(50)]
        public string TemplateName { get; set; }

        [StringLength(50)]
        public string DefaultFrom { get; set; }

        [StringLength(500)]
        public string DefaultTo { get; set; }

        [StringLength(500)]
        public string DefaultCC { get; set; }

        [StringLength(200)]
        public string DefaultSubject { get; set; }

        [StringLength(4000)]
        public string DefaultBody { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}

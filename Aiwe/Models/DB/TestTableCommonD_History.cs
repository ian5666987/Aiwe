namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TestTableCommonD_History
    {
        [Key]
        public int Cid { get; set; }

        [Required]
        [StringLength(100)]
        public string SimpleName { get; set; }

        public bool? SimpleBoolean { get; set; }

        [StringLength(1000)]
        public string Picture { get; set; }

        [StringLength(1000)]
        public string AnotherPicture { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? HRTS { get; set; }
    }
}

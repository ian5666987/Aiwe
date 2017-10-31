namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TestTableCommonJ_History
    {
        [Key]
        public int Cid { get; set; }

        [StringLength(200)]
        public string Element1 { get; set; }

        [StringLength(200)]
        public string ChangedNameElement2 { get; set; }

        public int? IntItem1 { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DateTimeItem1 { get; set; }

        [StringLength(200)]
        public string AddElement1 { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? CustomHRTS { get; set; }
    }
}

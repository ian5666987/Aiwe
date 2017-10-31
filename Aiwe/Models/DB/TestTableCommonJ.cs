namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TestTableCommonJ")]
    public partial class TestTableCommonJ
    {
        [Key]
        public int Cid { get; set; }

        [StringLength(200)]
        public string Element1 { get; set; }

        [StringLength(200)]
        public string Element2 { get; set; }

        public int? IntItem1 { get; set; }

        public int? IntItem2 { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DateTimeItem1 { get; set; }
    }
}

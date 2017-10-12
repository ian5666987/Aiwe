namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TestTableCommonG")]
    public partial class TestTableCommonG
    {
        [Key]
        public int Cid { get; set; }

        public int? SimpleNumber { get; set; }

        [StringLength(100)]
        public string SimpleString { get; set; }

        [StringLength(100)]
        public string SimpleName { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? SimpleDateTime { get; set; }

        public bool? SimpleBit { get; set; }

        public int SimpleAnotherNumber { get; set; }
    }
}

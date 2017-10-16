namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TestTableCommonI")]
    public partial class TestTableCommonI
    {
        [Key]
        public int Cid { get; set; }

        [StringLength(100)]
        public string StringVal { get; set; }

        [StringLength(100)]
        public string AnotherStringVal { get; set; }

        public int? IntVal { get; set; }

        public int? AnotherIntVal { get; set; }

        public float? DecimalVal { get; set; }

        public float? AnotherDecimalVal { get; set; }
    }
}

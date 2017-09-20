namespace Aibe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TestTableCommonF")]
    public partial class TestTableCommonF
    {
        [Key]
        public int Cid { get; set; }

        [StringLength(100)]
        public string SimpleString { get; set; }

        public int? IncrementId { get; set; }
    }
}

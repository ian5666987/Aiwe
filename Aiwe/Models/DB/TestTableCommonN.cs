namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TestTableCommonN")]
    public partial class TestTableCommonN
    {
        [Key]
        public int Cid { get; set; }

        [StringLength(500)]
        public string DdId { get; set; }

        [StringLength(4000)]
        public string DdDesc { get; set; }

        [StringLength(500)]
        public string TbId { get; set; }

        [StringLength(4000)]
        public string TbDesc { get; set; }

        public int NuId { get; set; }

        [StringLength(4000)]
        public string NuDesc { get; set; }
    }
}

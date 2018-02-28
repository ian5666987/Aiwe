namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TestTableCommonM")]
    public partial class TestTableCommonM
    {
        [Key]
        public int Cid { get; set; }

        [StringLength(500)]
        public string DdId { get; set; }

        [StringLength(500)]
        public string TbId { get; set; }

        public int NuId { get; set; }
    }
}

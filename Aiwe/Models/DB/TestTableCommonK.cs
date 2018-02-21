namespace Aiwe.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TestTableCommonK")]
    public partial class TestTableCommonK
    {
        [Key]
        public int Cid { get; set; }

        public string ItemName { get; set; }
    }
}
